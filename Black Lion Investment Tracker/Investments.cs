using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLIT.Extensions;
using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT;

public partial class Investments : ScrollContainer
{
    CancellationTokenSource cancellationTokenSource;
    [Export]
    PackedScene itemScene;
    [Export]
    VBoxContainer investmentHolder;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        cancellationTokenSource = new CancellationTokenSource();

        Continue();
    }

    public void Continue()
    {
        CancellationToken token = cancellationTokenSource.Token;
        Task.Run(async () =>
        {
            do
            {
                await Task.Delay(500);
                if (token.IsCancellationRequested)
                {
                    break;
                }
                else
                    await ShowInvestments();
                await Task.Delay(30000);

            }
            while (false); // Only Run Through Once For Now

        }, token);
    }
    public void Pause()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
    }

    // Do Calcumations On History For Investments
    Task ShowInvestments()
    {
        return Task.Run(async () =>
        {
            try
            {
                GD.Print("Starting Investments");

                // Get All Transactions
                List<CommerceTransactionHistory> buys = new();
                List<CommerceTransactionHistory> sells = new();

                // Get all the buy and sell orders from the API
                await GetBuyAndSellHistory(buys, sells);

                // Create the Investment database from those buy and sell orders
                await CreateInvestmentsFromOrders(buys, sells);

                Main.Database.GenerateCollapsed();

                GetNode<Label>("VBoxContainer/VBoxContainer/Label").Hide();

                // Remove Old Investment Items From UI
                foreach (var child in investmentHolder.GetChildren())
                    child.QueueFree();

                // Add New Investment Items To UI
                foreach (var investment in Main.Database.CollapsedInvestments)
                {
                    try
                    {
                        var item = Main.MyClient.WebApi.V2.Items.GetAsync(investment.ItemId).Result;
                        var iconBytes = Main.MyClient.WebApi.Render.DownloadToByteArrayAsync(item.Icon.Url).Result;

                        var image = new Image();
                        image.LoadPngFromBuffer(iconBytes);
                        var icon = ImageTexture.CreateFromImage(image);

                        var instance = itemScene.Instantiate<CollapsedTransactionItem>();
                        instance.Init(item.Name, icon, investment);
                        investmentHolder.AddChildSafe(instance, 0);
                    }
                    catch (Exception e)
                    {
                        GD.PrintErr(e);
                    }
                }

                GD.Print($"Buy Listings:{buys.Count}, Sell Listings:{sells.Count}, Similar Items:{buys.Select(b => b.ItemId).Where(i => sells.Select(s => s.ItemId).Contains(i)).Count()}");

                // Calculate Profit
                string totalInvested = Main.Database.TotalInvested.ToCurrencyString(false);
                string totalReturn = Main.Database.TotalReturn.ToCurrencyString(false);
                string totalProfit = Main.Database.TotalProfit.ToCurrencyString(false);
                GD.Print($"Total Invested: {totalInvested}, Total Return: {totalReturn},  Total Profit Inculing Tax: {totalProfit}, ROI: {Main.Database.ROI}");
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
        });
    }

    Task GetBuyAndSellHistory(List<CommerceTransactionHistory> buys, List<CommerceTransactionHistory> sells)
    {
        return Task.Run(async () =>
        {
            // loop through all pages to get all the transactions then continue
            int i = 0;
            while (true)
            {
                bool @continue = false;

                var pageBuys = Main.MyClient.WebApi.V2.Commerce.Transactions.History.Buys.PageAsync(i);
                while (true)
                {
                    await Task.Delay(250);
                    if (pageBuys.Status == TaskStatus.RanToCompletion)
                    {
                        buys.AddRange(pageBuys.Result);

                        // Break out of inner loop When we have the items we need
                        // And continue
                        @continue = true;
                        break;
                    }

                    // Break out of inner loop if it fails
                    // And don't continue
                    if (pageBuys.Status == TaskStatus.Faulted)
                        break;
                }

                var pageSells = Main.MyClient.WebApi.V2.Commerce.Transactions.History.Sells.PageAsync(i);
                while (true)
                {
                    await Task.Delay(250);
                    if (pageSells.Status == TaskStatus.RanToCompletion)
                    {
                        sells.AddRange(pageSells.Result);

                        // Break out of inner loop When we have the items we need
                        // And continue
                        @continue = true;
                        break;
                    }

                    // Break out of inner loop if it fails
                    // And don't continue
                    if (pageSells.Status == TaskStatus.Faulted)
                        break;
                }

                // Break out if we did not get any new elements
                if (@continue == false)
                    break;

                //Increment Loop
                i++;
            }
        });
    }

    Task CreateInvestmentsFromOrders(List<CommerceTransactionHistory> buys, List<CommerceTransactionHistory> sells)
    {
        return Task.Run(() =>
        {
            var sBuys = buys.OrderBy(b => b.Purchased);
            var sSells = sells.OrderBy(s => s.Purchased);

            // Go through all buys and check if any are investments
            foreach (var buy in sBuys)
            {
                int ammountLeft = buy.Quantity;
                var investment = new InvestmentData(buy);

                Item btem = null;
                try
                {
                    btem = Main.MyClient.WebApi.V2.Items.GetAsync(buy.ItemId).Result;
                }
                catch (Exception)
                {
                    // Most likely a new item that Gw2Sharp doesn't understand so we'll just skip it
                    GD.PushWarning($"Failed to retreive info on item {buy.ItemId}, most likely Gw2Sharp has not been updated yet to handle the item");
                    continue;
                }

                try
                {
                    GD.Print("");
                    GD.Print($"Checking Buy Order For {btem?.Name}");

                    // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                    foreach (var sell in sSells.Where(s => s.ItemId == buy.ItemId && s.Purchased > buy.Purchased))
                    {
                        var sellData = new SellData(sell);
                        if (ammountLeft > 0)
                        {
                            // Get all the other partial uses of this sell order
                            var databasePartials = Main.Database.Investments.SelectMany(i => i.SellDatas).Where(s => s.TransactionId == sell.Id);

                            // If is in the database
                            if (databasePartials.Any())
                            {
                                // Get the remaining quantity we can use in the sell order
                                var remaining = sell.Quantity - databasePartials.Sum(p => p.Quantity);

                                // If there is anything left in the sell order to use
                                if (remaining > 0)
                                {
                                    // If we need as much as is remaining or less, take what we need and continue
                                    if (ammountLeft <= remaining)
                                    {
                                        sellData.Quantity = ammountLeft;
                                        ammountLeft = 0;
                                    }
                                    // If we need more than there is left, take what is left and continue
                                    else if (ammountLeft > remaining)
                                    {
                                        sellData.Quantity = remaining;
                                        ammountLeft -= remaining;
                                    }
                                }
                            }
                            // Was not in the database
                            else
                            {
                                // If we need all of or less than the sell order, use it and continue
                                if (ammountLeft <= sell.Quantity)
                                {
                                    sellData.Quantity = ammountLeft;
                                    ammountLeft = 0;
                                }
                                // If we need more than there is in the sell order, take what we need and continue
                                else if (ammountLeft > sell.Quantity)
                                {
                                    ammountLeft -= sell.Quantity;
                                }
                            }
                            investment.SellDatas.Add(sellData);
                        }
                        // We made sure all the buys are accounted for so we don't need to check any more sells
                        else break;
                    }

                    // This is how we determine if an actual investment was created
                    if (investment.SellDatas.Count > 0)
                    {
                        // If there are left over bought items, remove them from this transaction because they did not sell
                        if (ammountLeft > 0)
                        {
                            investment.Quantity -= ammountLeft;
                        }

                        var item = Main.MyClient.WebApi.V2.Items.GetAsync(investment.ItemId).Result;
                        GD.Print($"New Investment -> {item.Name}, Bought {investment.Quantity} for {investment.TotalBuyPrice}, Sold {investment.SellQuantity}/{investment.SellDatas.Count} for {investment.TotalSellPrice}, for a Profit of {investment.Profit}");
                        Main.Database.Investments.Add(investment);
                    }
                }
                catch (Exception e)
                {
                    GD.PrintErr(e);
                }
            }
        });
    }
}
