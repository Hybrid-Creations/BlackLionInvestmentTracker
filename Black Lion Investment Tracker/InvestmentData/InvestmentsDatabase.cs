using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLIT.Saving;
using BLIT.UI;
using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

public partial class InvestmentsDatabase
{
    CompletedInvestmentsData completedInvestmentsData = new();
    PendingInvestmentsData pendingInvestmentsData = new();

    public List<InvestmentData> Investments => completedInvestmentsData.Investments;
    public List<long> NotInvestments => completedInvestmentsData.NotInvestments;
    public List<InvestmentData> PendingTransactions => pendingInvestmentsData.PendingTransactions;
    public List<CollapsedInvestmentData> CollapsedInvestments { get; private set; } = new List<CollapsedInvestmentData>();
    public long TotalInvested => completedInvestmentsData.Investments.Sum(i => i.TotalBuyPrice);
    public long TotalReturn => completedInvestmentsData.Investments.Sum(i => i.TotalSellPrice);
    public long TotalProfit => completedInvestmentsData.Investments.Sum(i => i.Profit);
    public double ROI => TotalProfit / (double)TotalInvested * 100;

    bool updating;

    public void GenerateCollapsed()
    {
        List<CollapsedInvestmentData> groups = new();

        foreach (var investment in Investments)
        {
            var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.ItemId && ci.IndividualPrice == investment.IndividualPrice && ci.Quantity + investment.Quantity <= Constants.MaxItemStack);
            if (readyGroup is not null)
            {
                readyGroup.SubInvestments.Add(investment);
            }
            else
            {
                var newCollapsedInvestment = new CollapsedInvestmentData(investment);
                groups.Add(newCollapsedInvestment);
            }
        }

        groups.ForEach(c => CollapsedInvestments.Add(c));
    }

    public void Update(Action OnAfterUpdate)
    {
        if (updating == true) return;
        updating = true;
        Task.Run(async () =>
        {
            AppStatusIndicator.ShowStatus("Updating Database...");
            await CalculateAndUpdateInvestments();
            OnAfterUpdate?.Invoke();
            updating = false;
        });
    }

    // Do Calculations On History For Investments
    Task CalculateAndUpdateInvestments()
    {
        return Task.Run(async () =>
        {
            try
            {
                GD.Print("Starting Database Update");

                // Get All Transactions
                List<CommerceTransactionHistory> buys = new();
                List<CommerceTransactionHistory> sells = new();

                // Get all the buy and sell orders from the API
                await GetBuyAndSellHistory(buys, sells);

                // Create the Investment database from those buy and sell orders
                await CreateInvestmentsFromOrders(buys, sells);

                Main.Database.CollapsedInvestments.Clear();
                Main.Database.GenerateCollapsed();

                AppStatusIndicator.ClearStatus();

                GD.Print($"Buy Listings:{buys.Count}, Sell Listings:{sells.Count}, Similar Items:{buys.Select(b => b.ItemId).Where(i => sells.Select(s => s.ItemId).Contains(i)).Count()}");

            }
            catch (System.Exception e)
            {
                GD.PrintErr(e);
            }
        });

        Task GetBuyAndSellHistory(List<CommerceTransactionHistory> buys, List<CommerceTransactionHistory> sells)
        {
            return Task.Run(async () =>
            {
                try
                {
                    // loop through all pages to get all the transactions then continue
                    int pageIndex = 0;
                    while (true)
                    {
                        string status = "Downloading transactions from GW2 server...";
                        AppStatusIndicator.ShowStatus(status);

                        bool @continue = false;

                        var pageBuys = Main.MyClient.WebApi.V2.Commerce.Transactions.History.Buys.PageAsync(pageIndex);
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

                        var pageSells = Main.MyClient.WebApi.V2.Commerce.Transactions.History.Sells.PageAsync(pageIndex);
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
                        pageIndex++;
                    }
                }
                catch (System.Exception e)
                {
                    GD.PrintErr(e);
                    throw e;
                }
            });
        }

        Task CreateInvestmentsFromOrders(List<CommerceTransactionHistory> buys, List<CommerceTransactionHistory> sells)
        {
            return Task.Run(() =>
            {
                string status = $"Checking for investments in transactions...";
                AppStatusIndicator.ShowStatus($"{status} (0/{buys.Count})");
                var sortedBuys = buys.OrderBy(b => b.Purchased);
                var sortedSells = sells.OrderBy(s => s.Purchased);

                // Go through all buys and check if any are investments
                int i = 0;
                foreach (var buy in sortedBuys)
                {
                    // Skip if we already have the transaction in the database, that means we have used it already
                    if (Main.Database.NotInvestments.Any(l => l == buy.Id))
                    {
                        GD.Print($"Skipped buy order \"{buy.Id}\" as it was marked as not an investment.");
                        SetStatusAndPrintDuration(status, ref i, buys.Count);
                        continue;
                    }
                    if (Main.Database.Investments.Any(i => i.TransactionId == buy.Id))
                    {
                        GD.Print($"Skipped buy order \"{buy.Id}\" as it was already in the database.");
                        SetStatusAndPrintDuration(status, ref i, buys.Count);
                        continue;
                    }

                    int buyAmmountLeft = buy.Quantity;
                    var investment = new InvestmentData(buy);

                    try
                    {
                        GD.Print("");
                        GD.Print($"Checking Buy Order For {buy.ItemId}");

                        // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                        CheckSellOrdersForMatches(buy, ref buyAmmountLeft, investment, sortedSells);

                        // This is how we determine if an actual investment was created
                        if (investment.SellDatas.Count > 0)
                        {
                            // If there are left over bought items, remove them from this transaction because they did not sell
                            if (buyAmmountLeft > 0)
                            {
                                investment.Quantity -= buyAmmountLeft;
                            }

                            GD.Print($"New Investment -> {buy.ItemId}, Bought {investment.Quantity} for {investment.TotalBuyPrice}, Sold {investment.SellQuantity}/{investment.SellDatas.Count} for {investment.TotalSellPrice}, for a Profit of {investment.Profit}");
                            Investments.Add(investment);
                        }
                        // This means we did buy the item, but we have not sold any yet, so it is pending
                        else
                        {
                            //TODO: Here is where we have the "Pending" Investments, as the buy orders have not been sold back
                            PendingTransactions.Add(investment);
                        }
                    }
                    catch (System.Exception e)
                    {
                        GD.PrintErr(e);
                    }

                    SetStatusAndPrintDuration(status, ref i, buys.Count);
                }
                AppStatusIndicator.ShowStatus($"{status} ({buys.Count}/{buys.Count})");
            });

            //---------- ---------- ---------- ---------- ----------
            void SetStatusAndPrintDuration(string status, ref int i, int buysCount)
            {
                AppStatusIndicator.ShowStatus($"{status} ({i}/{buysCount})");
                i++;
            }

            //---------- ---------- ---------- ---------- ----------
            void CheckSellOrdersForMatches(CommerceTransactionHistory buy, ref int buyAmmountLeft, InvestmentData investment, IOrderedEnumerable<CommerceTransactionHistory> sortedSells)
            {
                foreach (var sell in sortedSells.Where(s => s.ItemId == buy.ItemId && s.Purchased > buy.Purchased))
                {
                    var sellData = new SellData(sell);
                    if (buyAmmountLeft > 0)
                    {
                        // Get all the other partial uses of this sell order
                        var databasePartials = Investments.SelectMany(i => i.SellDatas).Where(s => s.TransactionId == sell.Id);

                        // If is in the database
                        if (databasePartials.Any())
                        {
                            // Get the remaining quantity we can use in the sell order
                            var remaining = sell.Quantity - databasePartials.Sum(p => p.Quantity);

                            // If there is anything left in the sell order to use
                            if (remaining > 0)
                            {
                                // If we need as much as is remaining or less, take what we need and continue
                                if (buyAmmountLeft <= remaining)
                                {
                                    sellData.Quantity = buyAmmountLeft;
                                    buyAmmountLeft = 0;
                                }
                                // If we need more than there is left, take what is left and continue
                                else if (buyAmmountLeft > remaining)
                                {
                                    sellData.Quantity = remaining;
                                    buyAmmountLeft -= remaining;
                                }
                            }
                            // If there are no items left to use in this sell order, skip it
                            else
                            {
                                GD.Print($"Skipped sell order \"{sell.Id}\" as it was already in the database and fully used.");
                                continue;
                            }
                        }
                        // Was not in the database
                        else
                        {
                            // If we need all of or less than the sell order, use it and continue
                            if (buyAmmountLeft <= sell.Quantity)
                            {
                                sellData.Quantity = buyAmmountLeft;
                                buyAmmountLeft = 0;
                            }
                            // If we need more than there is in the sell order, take what we need and continue
                            else if (buyAmmountLeft > sell.Quantity)
                            {
                                buyAmmountLeft -= sell.Quantity;
                            }
                        }
                        investment.SellDatas.Add(sellData);
                    }
                    // We made sure all the buys are accounted for so we don't need to check any more sells
                    else break;
                }
            }
        }
    }

    public class CompletedInvestmentsData
    {
        public List<InvestmentData> Investments { get; private set; } = new();
        public List<long> NotInvestments { get; private set; } = new();
    }
    public class PendingInvestmentsData
    {
        public List<InvestmentData> PendingTransactions { get; private set; } = new();
    }

    const string databasePath = "user://database.completed";
    public void Save()
    {
        SaveSystem.SaveToFile(databasePath, completedInvestmentsData);
    }
    public void Load()
    {
        if (SaveSystem.TryLoadFromFile(databasePath, out CompletedInvestmentsData newData))
            completedInvestmentsData = newData;
        GD.Print($"Loaded Database; {Investments.Count}, {Investments.Count}, {CollapsedInvestments.Count}, {NotInvestments.Count}");
    }
}
