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
    public List<PendingInvestmentData> PendingInvestments => pendingInvestmentsData.PendingInvestments;
    public List<CollapsedInvestmentData> CollapsedInvestments { get; private set; } = new List<CollapsedInvestmentData>();
    public List<CollapsedPendingInvestmentData> CollapsedPendingInvestments { get; private set; } = new List<CollapsedPendingInvestmentData>();
    public long TotalInvested => completedInvestmentsData.Investments.Sum(i => i.TotalBuyPrice);
    public long TotalReturn => completedInvestmentsData.Investments.Sum(i => i.TotalSellPrice);
    public long TotalProfit => completedInvestmentsData.Investments.Sum(i => i.Profit);
    public double ROI => TotalProfit / (double)TotalInvested * 100;

    bool updating;

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
                List<CommerceTransactionHistory> buyOrders = new();
                List<CommerceTransactionHistory> sellOrders = new();
                List<CommerceTransactionCurrent> postedSellOrders = new();

                // Get all the buy and sell orders from the API
                await GetBuyAndSellHistory(buyOrders, sellOrders, postedSellOrders);

                // Create the Investment database from those buy and sell orders
                await CreateInvestmentsFromOrders(buyOrders, sellOrders, postedSellOrders);

                Main.Database.CollapsedInvestments.Clear();
                Main.Database.GenerateCollapsed();

                Main.Database.CollapsedPendingInvestments.Clear();
                Main.Database.GenerateCollapsedPending();

                AppStatusIndicator.ClearStatus();

                //GD.Print($"Buy Listings:{buyOrders.Count}, Sell Listings:{sellOrders.Count}, Similar Items:{buyOrders.Select(b => b.ItemId).Where(i => sellOrders.Select(s => s.ItemId).Contains(i)).Count()}");
            }
            catch (System.Exception e)
            {
                GD.PrintErr(e);
            }
        });

        Task GetBuyAndSellHistory(List<CommerceTransactionHistory> buyOrders, List<CommerceTransactionHistory> sellOrders, List<CommerceTransactionCurrent> postedSellOrders)
        {
            return Task.Run(async () =>
            {
                try
                {
                    AppStatusIndicator.ShowStatus("Downloading transactions from GW2 server...");
                    // loop through all pages to get all the transactions then continue

                    int pageIndex = 0;
                    while (true)
                    {
                        var pageBuys = Main.MyClient.WebApi.V2.Commerce.Transactions.History.Buys.PageAsync(pageIndex);
                        await Task.Delay(250);
                        if (pageBuys.Status == TaskStatus.RanToCompletion)
                        {
                            AppStatusIndicator.ShowStatus("Downloading buy order history from GW2 server...");
                            buyOrders.AddRange(pageBuys.Result);

                            //Increment Loop
                            pageIndex++;
                            // Keep looping through pages till we have all the items
                            continue;
                        }

                        // Break out of inner loop if it fails
                        // And don't continue
                        if (pageBuys.Status == TaskStatus.Faulted)
                            break;
                    }

                    pageIndex = 0;
                    while (true)
                    {
                        var pageSells = Main.MyClient.WebApi.V2.Commerce.Transactions.History.Sells.PageAsync(pageIndex);
                        await Task.Delay(250);
                        if (pageSells.Status == TaskStatus.RanToCompletion)
                        {
                            AppStatusIndicator.ShowStatus("Downloading sell order history from GW2 server...");
                            sellOrders.AddRange(pageSells.Result);

                            //Increment Loop
                            pageIndex++;
                            // Keep looping through pages till we have all the items
                            continue;
                        }

                        // Break out of inner loop if it fails
                        // And don't continue
                        if (pageSells.Status == TaskStatus.Faulted)
                            break;
                    }

                    pageIndex = 0;
                    while (true)
                    {
                        var postedSells = Main.MyClient.WebApi.V2.Commerce.Transactions.Current.Sells.PageAsync(pageIndex);
                        await Task.Delay(250);
                        if (postedSells.Status == TaskStatus.RanToCompletion)
                        {
                            AppStatusIndicator.ShowStatus("Downloading current sell orders from GW2 server...");
                            postedSellOrders.AddRange(postedSells.Result);

                            //Increment Loop
                            pageIndex++;
                            // Keep looping through pages till we have all the items
                            continue;
                        }

                        // Break out of inner loop if it fails
                        // And don't continue
                        if (postedSells.Status == TaskStatus.Faulted)
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    GD.PrintErr(e);
                    throw e;
                }
            });
        }

        Task CreateInvestmentsFromOrders(List<CommerceTransactionHistory> buys, List<CommerceTransactionHistory> sells, List<CommerceTransactionCurrent> postedSellOrders)
        {
            return Task.Run(() =>
            {
                string status = $"Checking for investments in transactions...";
                AppStatusIndicator.ShowStatus($"{status} (0/{buys.Count})");
                var sortedBuys = buys.OrderBy(b => b.Purchased);
                var sortedSells = sells.OrderBy(s => s.Purchased);
                var sortedPostedSellOrders = postedSellOrders.OrderBy(p => p.Created);

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
                        // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                        CheckHistorySellOrdersForMatches(buy, ref buyAmmountLeft, investment, sortedSells);

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
                            var pendingInvestment = new PendingInvestmentData(buy, new Lazy<int>(() => Cache.Prices.GetPrice(buy.ItemId)));

                            // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                            CheckPostedSellOrdersForMatches(buy, ref buyAmmountLeft, pendingInvestment, sortedPostedSellOrders);

                            PendingInvestments.Add(pendingInvestment);
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
            void CheckHistorySellOrdersForMatches(CommerceTransactionHistory buyOrder, ref int buyAmmountLeft, InvestmentData investment, IOrderedEnumerable<CommerceTransactionHistory> sortedSellOrders)
            {
                foreach (var sellOrder in sortedSellOrders.Where(s => s.ItemId == buyOrder.ItemId && s.Purchased > buyOrder.Purchased))
                {
                    var sellData = new SellData(sellOrder);
                    if (buyAmmountLeft > 0)
                    {
                        // Get all the other partial uses of this sell order
                        var databasePartials = Investments.SelectMany(i => i.SellDatas).Where(s => s.TransactionId == sellOrder.Id);

                        // If is in the database
                        if (databasePartials.Any())
                        {
                            // Get the remaining quantity we can use in the sell order
                            var remaining = sellOrder.Quantity - databasePartials.Sum(p => p.Quantity);

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
                                GD.Print($"Skipped sell order \"{sellOrder.Id}\" as it was already in the database and fully used.");
                                continue;
                            }
                        }
                        // Was not in the database
                        else
                        {
                            // If we need all of or less than the sell order, use it and continue
                            if (buyAmmountLeft <= sellOrder.Quantity)
                            {
                                sellData.Quantity = buyAmmountLeft;
                                buyAmmountLeft = 0;
                            }
                            // If we need more than there is in the sell order, take what we need and continue
                            else if (buyAmmountLeft > sellOrder.Quantity)
                            {
                                buyAmmountLeft -= sellOrder.Quantity;
                            }
                        }
                        investment.SellDatas.Add(sellData);
                    }
                    // We made sure all the buys are accounted for so we don't need to check any more sells
                    else break;
                }
            }

            //---------- ---------- ---------- ---------- ----------
            void CheckPostedSellOrdersForMatches(CommerceTransactionHistory buyOrder, ref int buyAmmountLeft, PendingInvestmentData pendingInvestment, IOrderedEnumerable<CommerceTransactionCurrent> sortedPostedSellOrders)
            {
                foreach (var postedSellOrder in sortedPostedSellOrders.Where(s => s.ItemId == buyOrder.ItemId && s.Created > buyOrder.Purchased))
                {
                    var pendingSellData = new PendingSellData(postedSellOrder);
                    if (buyAmmountLeft > 0)
                    {
                        // Get all the other partial uses of this sell order
                        var databasePartials = PendingInvestments.SelectMany(i => i.PostedSellDatas).Where(s => s.TransactionId == postedSellOrder.Id);

                        // If is in the database
                        if (databasePartials.Any())
                        {
                            // Get the remaining quantity we can use in the sell order
                            var remaining = postedSellOrder.Quantity - databasePartials.Sum(p => p.Quantity);

                            // If there is anything left in the sell order to use
                            if (remaining > 0)
                            {
                                // If we need as much as is remaining or less, take what we need and continue
                                if (buyAmmountLeft <= remaining)
                                {
                                    pendingSellData.Quantity = buyAmmountLeft;
                                    buyAmmountLeft = 0;
                                }
                                // If we need more than there is left, take what is left and continue
                                else if (buyAmmountLeft > remaining)
                                {
                                    pendingSellData.Quantity = remaining;
                                    buyAmmountLeft -= remaining;
                                }
                            }
                            // If there are no items left to use in this sell order, skip it
                            else
                            {
                                GD.Print($"Skipped sell posting \"{postedSellOrder.Id}\" as it was already in the database and fully used.");
                                continue;
                            }
                        }
                        // Was not in the database
                        else
                        {
                            // If we need all of or less than the sell order, use it and continue
                            if (buyAmmountLeft <= postedSellOrder.Quantity)
                            {
                                pendingSellData.Quantity = buyAmmountLeft;
                                buyAmmountLeft = 0;
                            }
                            // If we need more than there is in the sell order, take what we need and continue
                            else if (buyAmmountLeft > postedSellOrder.Quantity)
                            {
                                buyAmmountLeft -= postedSellOrder.Quantity;
                            }
                        }
                        pendingInvestment.PostedSellDatas.Add(pendingSellData);
                    }
                    // We made sure all the buys are accounted for so we don't need to check any more sells
                    else break;
                }
            }
        }
    }

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

    public void GenerateCollapsedPending()
    {
        List<CollapsedPendingInvestmentData> groups = new();

        foreach (var investment in PendingInvestments.Where(p => p.PostedSellDatas.Count == 0))
        {
            var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.ItemId && ci.IndividualPrice == investment.IndividualPrice && ci.Quantity + investment.Quantity <= Constants.MaxItemStack);
            if (readyGroup is not null)
            {
                readyGroup.SubInvestments.Add(investment);
            }
            else
            {
                var newCollapsedInvestment = new CollapsedPendingInvestmentData(investment, new Lazy<int>(() => Cache.Prices.GetPrice(investment.ItemId)));
                groups.Add(newCollapsedInvestment);
            }
        }

        groups.ForEach(c => CollapsedPendingInvestments.Add(c));
    }

    public class CompletedInvestmentsData
    {
        public List<InvestmentData> Investments { get; private set; } = new();
        public List<long> NotInvestments { get; private set; } = new();
    }
    public class PendingInvestmentsData
{
        public List<PendingInvestmentData> PendingInvestments { get; private set; } = new();
    }

    // ---------- Saving
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
