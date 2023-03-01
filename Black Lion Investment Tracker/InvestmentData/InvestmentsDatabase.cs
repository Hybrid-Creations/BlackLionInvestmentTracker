using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using BLIT.ConstantVariables;
using BLIT.Saving;
using BLIT.UI;
using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

public partial class InvestmentsDatabase
{
    public List<long> NotInvestments { get; private set; } = new();
    public List<CompletedInvestment> CompletedInvestments { get; private set; } = new();
    public List<PendingInvestment> PendingInvestments { get; private set; } = new();
    public List<PotentialInvestment> PotentialInvestments { get; private set; } = new();

    public List<CollapsedCompletedInvestment> CollapsedCompletedInvestments { get; private set; } = new();
    public List<CollapsedPendingInvestment> CollapsedPendingInvestments { get; private set; } = new();
    public List<CollapsedPotentialInvestment> CollapsedPotentialInvestments { get; private set; } = new();

    public long TotalInvested => CompletedInvestments.Sum(i => i.TotalBuyPrice);
    public long TotalReturn => CompletedInvestments.Sum(i => i.TotalSellPrice);
    public long TotalProfit => CompletedInvestments.Sum(i => i.Profit);
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
            var deffered = Callable.From(() => OnAfterUpdate?.Invoke());
            deffered.CallDeferred();
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

                Main.Database.CollapsedCompletedInvestments.Clear();
                Main.Database.GenerateCollapsedCompleted();

                //Main.Database.CollapsedPendingInvestments.Clear();
                //Main.Database.GenerateCollapsedPending();

                AppStatusIndicator.ClearStatus();

                //GD.Print($"Buy Listings:{buyOrders.Count}, Sell Listings:{sellOrders.Count}, Similar Items:{buyOrders.Select(b => b.ItemId).Where(i => sellOrders.Select(s => s.ItemId).Contains(i)).Count()}");
            }
            catch (System.Exception e)
            {
                GD.PushError(e);
            }
        });
    }

    private static Task GetBuyAndSellHistory(List<CommerceTransactionHistory> buyOrders, List<CommerceTransactionHistory> sellOrders, List<CommerceTransactionCurrent> postedSellOrders)
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
                GD.PushError(e);
                throw e;
            }
        });
    }

    private Task CreateInvestmentsFromOrders(List<CommerceTransactionHistory> buys, List<CommerceTransactionHistory> sells, List<CommerceTransactionCurrent> postedSellOrders)
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
            foreach (var buyOrder in sortedBuys)
            {
                //Skip if we already have the transaction in the database, that means we have used it already
                if (Main.Database.NotInvestments.Any(l => l == buyOrder.Id))
                {
                    //GD.Print($"Skipped buy order \"{buy.Id}\" as it was marked as not an investment.");
                    SetStatusAndPrintDuration(status, ref i, buys.Count);
                    continue;
                }
                if (Main.Database.CompletedInvestments.Any(i => i.Data.BuyData.TransactionId == buyOrder.Id))
                {
                    //GD.Print($"Skipped buy order \"{buy.Id}\" as it was already in the database.");
                    SetStatusAndPrintDuration(status, ref i, buys.Count);
                    continue;
                }
                if (Main.Database.PendingInvestments.Any(i => i.Data.BuyData.TransactionId == buyOrder.Id))
                {
                    //GD.Print($"Skipped buy order \"{buy.Id}\" as it was already in the database.");
                    SetStatusAndPrintDuration(status, ref i, buys.Count);
                    continue;
                }

                int buyAmmountLeft = buyOrder.Quantity;
                var buyData = new BuyData(buyOrder);

                try
                {
                    // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                    var sellDatas = CheckHistorySellOrdersForCompleteInvestmentMatches(buyOrder, ref buyAmmountLeft, sortedSells);

                    // This is how we determine if an actual investment was created
                    if (sellDatas.Count > 0)
                    {
                        // If there are left over bought items, remove them from this transaction because they did not sell
                        if (buyAmmountLeft > 0)
                        {
                            buyData.Quantity -= buyAmmountLeft;
                        }

                        var investment = new CompletedInvestment(new CompletedInvestmentData(buyData, sellDatas));
                        //GD.Print($"New Investment -> {buyOrder.ItemId}, Bought {investment.Quantity} for {investment.TotalBuyPrice}, Sold {investment.SellQuantity}/{investment.Data.SellDatas.Count} for {investment.TotalSellPrice}, for a Profit of {investment.Profit}");
                        CompletedInvestments.Add(investment);
                    }
                    // This means we did buy the item, but we have not sold any yet, so it is pending
                    else
                    {

                        // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                        var pendingSellDatas = CheckPostedSellOrdersForMatches(buyOrder, ref buyAmmountLeft, sortedPostedSellOrders);
                        var pendingInvestmentData = new PendingInvestmentData(buyData, pendingSellDatas, new Lazy<int>(() => Cache.Prices.GetPrice(buyOrder.ItemId)));

                        PendingInvestments.Add(new PendingInvestment(pendingInvestmentData));
                    }
                }
                catch (System.Exception e)
                {
                    GD.PushError(e);
                }

                SetStatusAndPrintDuration(status, ref i, buys.Count);
            }
            AppStatusIndicator.ShowStatus($"{status} ({buys.Count}/{buys.Count})");
        });
    }

    private void SetStatusAndPrintDuration(string status, ref int i, int buysCount)
    {
        AppStatusIndicator.ShowStatus($"{status} ({i}/{buysCount})");
        i++;
    }

    private List<SellData> CheckHistorySellOrdersForCompleteInvestmentMatches(CommerceTransactionHistory buyOrder, ref int buyAmmountLeft, IOrderedEnumerable<CommerceTransactionHistory> sortedSellOrders)
    {
        var sellDataList = new List<SellData>();
        foreach (var sellOrder in sortedSellOrders.Where(s => s.ItemId == buyOrder.ItemId && s.Purchased > buyOrder.Purchased))
        {
            var sellData = new SellData(sellOrder);
            if (buyAmmountLeft > 0)
            {
                // Get all the other partial uses of this sell order
                var databasePartials = CompletedInvestments.SelectMany(i => i.Data.SellDatas).Where(s => s.TransactionId == sellOrder.Id);

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
                        //GD.Print($"Skipped sell order \"{sellOrder.Id}\" as it was already in the database and fully used.");
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
                sellDataList.Add(sellData);
            }
            // We made sure all the buys are accounted for so we don't need to check any more sells
            else break;
        }
        return sellDataList;
    }

    private List<PendingSellData> CheckPostedSellOrdersForMatches(CommerceTransactionHistory buyOrder, ref int buyAmmountLeft, IOrderedEnumerable<CommerceTransactionCurrent> sortedPostedSellOrders)
    {
        var pendingSellDataList = new List<PendingSellData>();
        foreach (var postedSellOrder in sortedPostedSellOrders.Where(s => s.ItemId == buyOrder.ItemId && s.Created > buyOrder.Purchased))
        {
            var pendingSellData = new PendingSellData(postedSellOrder);
            if (buyAmmountLeft > 0)
            {
                // Get all the other partial uses of this sell order
                var databasePartials = PendingInvestments.SelectMany(i => i.Data.PostedSellDatas).Where(s => s.TransactionId == postedSellOrder.Id);

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
                pendingSellDataList.Add(pendingSellData);
            }
            // We made sure all the buys are accounted for so we don't need to check any more sells
            else break;
        }
        return pendingSellDataList;
    }

    public void GenerateCollapsedCompleted()
    {
        List<CollapsedCompletedInvestment> groups = new();

        foreach (var investment in CompletedInvestments)
        {
            var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.Data.BuyData.ItemId && ci.IndividualBuyPrice == investment.IndividualBuyPrice && ci.Quantity + investment.Quantity <= Constants.MaxItemStack);
            if (readyGroup is not null)
            {
                readyGroup.SubInvestments.Add(investment);
            }
            else
            {
                var newCollapsedInvestment = new CollapsedCompletedInvestment(investment);
                groups.Add(newCollapsedInvestment);
            }
        }

        groups.ForEach(c => CollapsedCompletedInvestments.Add(c));
    }

    // public void GenerateCollapsedPending()
    // {
    //     List<CollapsedPendingInvestment> groups = new();

    //     foreach (var investment in PendingInvestments.Where(p => p.Data.PostedSellDatas.Count == 0))
    //     {
    //         var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.Data.BuyData.ItemId && ci.IndividualBuyPrice == investment.IndividualBuyPrice && ci.Quantity + investment.Quantity <= Constants.MaxItemStack);
    //         if (readyGroup is not null)
    //         {
    //             readyGroup.SubInvestments.Add(investment);
    //         }
    //         else
    //         {
    //             var newCollapsedInvestment = new CollapsedPendingInvestment(investment);
    //             groups.Add(newCollapsedInvestment);
    //         }
    //     }

    //     groups.ForEach(c => CollapsedPendingInvestments.Add(c));
    // }

    [DataContract]
    public class CompletedInvestmentsData
    {
        [DataMember] public List<CompletedInvestmentData> Investments { get; internal set; } = new();
        [DataMember] public List<long> NotInvestments { get; internal set; } = new();
    }

    [DataContract]
    public class PendingInvestmentsData
    {
        [DataMember] public List<PendingInvestmentData> PendingInvestments { get; private set; } = new();
    }

    // ---------- Saving
    const string databasePath = "user://database.completed";
    public void Save()
    {
        var savedData = new CompletedInvestmentsData();
        savedData.Investments = CompletedInvestments.Select(c => c.Data).ToList();
        savedData.NotInvestments = NotInvestments;
        SaveSystem.SaveToFile(databasePath, savedData);
    }

    // ---------- Loading
    public void Load()
    {
        if (SaveSystem.TryLoadFromFile(databasePath, out CompletedInvestmentsData newData))
        {
            CompletedInvestments = newData.Investments.Select(i => new CompletedInvestment(i)).ToList() ?? new();
            NotInvestments = newData.NotInvestments ?? new();
        }
        GD.Print($"Loaded Investment Database: i:{CompletedInvestments.Count}, n:{NotInvestments.Count}");
    }
}
