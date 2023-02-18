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
    VBoxContainer listingsContainer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        cancellationTokenSource = new CancellationTokenSource();
        listingsContainer = GetChild<VBoxContainer>(0);

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

                // Remove Old Investment Items From UI
                foreach (var child in listingsContainer.GetChildren())
                {
                    if (child.Name == "ColumnTitles" || child.Name == "TitlesBorder")
                        continue;
                    child.QueueFree();
                }

                // Add New Investment Items To UI
                foreach (var investment in Main.Database.CollapsedInvestments)
                {
                    var item = Main.MyClient.WebApi.V2.Items.GetAsync(investment.ItemId).Result;
                    var iconBytes = Main.MyClient.WebApi.Render.DownloadToByteArrayAsync(item.Icon.Url).Result;

                    var image = new Image();
                    image.LoadPngFromBuffer(iconBytes);
                    var icon = ImageTexture.CreateFromImage(image);

                    var instance = itemScene.Instantiate<TransactionItem>();
                    instance.Init(item.Name, icon, investment);
                    listingsContainer.AddChild(instance);
                    listingsContainer.MoveChild(instance, 2);
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

                Gw2Sharp.WebApi.V2.Models.Item btem = null;
                try
                {
                    btem = Main.MyClient.WebApi.V2.Items.GetAsync(buy.ItemId).Result;
                }
                catch (Exception)
                {
                    // Most likely a new item that Gw2Sharp doesn't understand so we'll just skip it
                    continue;
                }

                GD.Print("");
                GD.Print($"Checking Buy Order For {btem?.Name}");

                // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                foreach (var sell in sSells.Where(s => s.ItemId == buy.ItemId && s.Purchased > buy.Purchased))
                {
                    var sellData = new SellData(sell);
                    if (ammountLeft > 0)
                    {
                        if (sell.Quantity > ammountLeft)
                        {
                            // Check through database of investments for a partial sell, and use the rest of that
                            bool inDatabase = Main.Database.Investments.SelectMany(i => i.SellDatas).Select(s => s.TransactionId).Contains(sell.Id);

                            if (inDatabase)
                            {
                                var dataBaseSell = Main.Database.Investments.SelectMany(i => i.SellDatas).First(s => s.TransactionId == sell.Id);

                                // If the database had this sell, and it used up all its quantity, don't check it
                                // Should Never Happen
                                if (dataBaseSell.Quantity == sell.Quantity)
                                {
                                    GD.PushWarning("The Unlikely Happend!");
                                    continue;
                                }

                                // It was partially used so we can use the rest now
                                if (dataBaseSell.Quantity < sell.Quantity)
                                {
                                    // The amount left for another partial
                                    var leftOver = sell.Quantity - dataBaseSell.Quantity;

                                    // If there is more left in the sell order take what we need
                                    if (leftOver > ammountLeft)
                                    {
                                        sellData.Quantity = ammountLeft;
                                        ammountLeft = 0;
                                    }
                                    else
                                    {
                                        sellData.Quantity = leftOver;
                                        ammountLeft -= leftOver;
                                    }
                                }
                            }
                            // If its not in the database then start a new partial
                            else
                            {
                                sellData.Quantity = ammountLeft;
                                ammountLeft = 0;
                            }
                        }
                        else if (sell.Quantity == ammountLeft)
                        {
                            ammountLeft -= sell.Quantity;
                        }
                        else // if (sell.Quantity < ammountLeft)
                        {
                            ammountLeft -= sell.Quantity;
                        }
                        // Make sure to just take what we need when we add the thing to the DB
                        investment.SellDatas.Add(sellData);
                    }
                    else break;
                }

                // This is how we determine if an actual investment was created
                if (investment.SellDatas.Count > 0)
                {
                    if (ammountLeft > 0)
                    {
                        investment.Quantity -= ammountLeft;
                    }
                    var item = Main.MyClient.WebApi.V2.Items.GetAsync(investment.ItemId).Result;
                    GD.Print($"New Investment -> {item.Name}, Bought {investment.Quantity} for {investment.TotalBuyPrice}, Sold {investment.SellQuantity} for {investment.TotalSellPrice}, for a Profit of {investment.Profit}");
                    Main.Database.Investments.Add(investment);
                }
            }
        });
    }
}
