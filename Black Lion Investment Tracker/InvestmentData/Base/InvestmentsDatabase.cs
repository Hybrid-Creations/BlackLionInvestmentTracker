using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using BLIT.ConstantVariables;
using BLIT.Saving;
using BLIT.UI;
using Godot;
using Gw2Sharp.WebApi.Exceptions;
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

    public double TotalInvested => CompletedInvestments.Sum(i => i.BuyData.TotalBuyPrice);
    public double TotalReturn => CompletedInvestments.Sum(i => i.TotalSellPrice);
    public double TotalProfit => CompletedInvestments.Sum(i => i.TotalProfit);
    public double ROI => TotalProfit / TotalInvested * 100;

    bool updating;

    public void RefreshData(Action OnAfterUpdate, CancellationToken cancelSource)
    {
        if (updating == true) return;
        updating = true;
        Task.Run(async () =>
        {
            AppStatusIndicator.ShowStatus("Updating Database...");
            APIStatusIndicator.ClearStatus();
            Cache.Prices.Clear();

            await CalculateAndUpdateInvestments(cancelSource);

            if (cancelSource.IsCancellationRequested)
            {
                updating = false;
                return;
            }

            OnAfterUpdate?.Invoke();
            updating = false;
        }, cancelSource);
    }

    public Task RefreshDataAsync(CancellationToken cancelToken)
    {
        if (updating == true) return null;
        updating = true;
        return Task.Run(async () =>
        {
            AppStatusIndicator.ShowStatus("Updating Database...");
            Cache.Prices.Clear();

            await CalculateAndUpdateInvestments(cancelToken);

            if (cancelToken.IsCancellationRequested)
            {
                updating = false;
                return;
            }

            updating = false;

        }, cancelToken);
    }

    // Do Calculations On History For Investments
    async Task CalculateAndUpdateInvestments(CancellationToken cancelToken)
    {
        try
        {
            GD.Print("Starting Database Update");

            // We don't want to clear completed here as that is keeping history that might not be obtainable again
            PendingInvestments.Clear();
            PotentialInvestments.Clear();

            // Get all the buy and sell orders from the API
            if (cancelToken.IsCancellationRequested) return;

            var buyOrders = await GetBuyOrdersAsync(0, cancelToken);
            var sellOrders = await GetSellOrdersAsync(0, cancelToken);
            var postedSellOrders = await GetPostedSellOrdersAsync(0, cancelToken);

            if (cancelToken.IsCancellationRequested) return;

            // Create the Investment database from those buy and sell orders
            CreateInvestmentsFromOrders(buyOrders.ToList(), sellOrders.ToList(), postedSellOrders.ToList());

            CollapsedCompletedInvestments.Clear();
            GenerateCollapsedCompleted();

            CollapsedPendingInvestments.Clear();
            GenerateCollapsedPending();

            CollapsedPotentialInvestments.Clear();
            GenerateCollapsedPotential();

            AppStatusIndicator.ClearStatus();
        }
        catch (System.Exception e)
        {
            GD.PushError(e);
        }
    }

    private static async Task<IEnumerable<CommerceTransactionHistory>> GetBuyOrdersAsync(int pageIndex, CancellationToken cancelToken)
    {
        try
        {
            AppStatusIndicator.ShowStatus("Downloading buy orders from GW2 server...");
            var pageBuys = await Main.MyClient.WebApi.V2.Commerce.Transactions.History.Buys.PageAsync(pageIndex, cancelToken);

            GD.Print($"num items in page {pageIndex} => {pageBuys.Count}");
            return pageBuys.Concat(await GetBuyOrdersAsync(pageIndex + 1, cancelToken));
        }
        catch (PageOutOfRangeException)
        {
            GD.Print("End of pages for buy orders.");
            return new List<CommerceTransactionHistory>();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return new List<CommerceTransactionHistory>();
        }
    }

    private static async Task<IEnumerable<CommerceTransactionHistory>> GetSellOrdersAsync(int pageIndex, CancellationToken cancelToken)
    {
        try
        {
            AppStatusIndicator.ShowStatus("Downloading sell orders from GW2 server...");
            var pageBuys = await Main.MyClient.WebApi.V2.Commerce.Transactions.History.Sells.PageAsync(pageIndex, cancelToken);

            GD.Print($"num items in page {pageIndex} => {pageBuys.Count}");
            return pageBuys.Concat(await GetSellOrdersAsync(pageIndex + 1, cancelToken));
        }
        catch (PageOutOfRangeException)
        {
            GD.Print("End of pages for sell orders.");
            return Enumerable.Empty<CommerceTransactionHistory>();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return Enumerable.Empty<CommerceTransactionHistory>();
        }
    }

    private static async Task<IEnumerable<CommerceTransactionCurrent>> GetPostedSellOrdersAsync(int pageIndex, CancellationToken cancelToken)
    {
        try
        {
            AppStatusIndicator.ShowStatus("Downloading posted sell orders from GW2 server...");
            var pageBuys = await Main.MyClient.WebApi.V2.Commerce.Transactions.Current.Sells.PageAsync(pageIndex, cancelToken);

            GD.Print($"num items in page {pageIndex} => {pageBuys.Count}");
            return pageBuys.Concat(await GetPostedSellOrdersAsync(pageIndex + 1, cancelToken));
        }
        catch (PageOutOfRangeException)
        {
            GD.Print("End of pages for sell orders.");
            return Enumerable.Empty<CommerceTransactionCurrent>();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return Enumerable.Empty<CommerceTransactionCurrent>();
        }
    }

    private void CreateInvestmentsFromOrders(List<CommerceTransactionHistory> buys, List<CommerceTransactionHistory> sells, List<CommerceTransactionCurrent> postedSells)
    {
        var sortedSells = sells.OrderBy(s => s.Purchased).ToList();
        var sortedPostedSellOrders = postedSells.OrderBy(p => p.Created).ToList();

        string status = $"Checking for investments in transactions...";
        AppStatusIndicator.ShowStatus($"{status} (0/{buys.Count})");

        // Go through all buys and check if any are investments
        int i = 0;
        foreach (var buyOrder in buys.OrderBy(b => b.Purchased))
        {
            //Skip if we already have the transaction in the database, that means we have used it already
            if (NotInvestments.Any(l => l == buyOrder.Id))
            {
                SetStatusAndPrintAmmount(status, ref i, buys.Count);
                continue;
            }
            if (CompletedInvestments.Any(i => i.BuyData.TransactionId == buyOrder.Id))
            {
                SetStatusAndPrintAmmount(status, ref i, buys.Count);
                continue;
            }
            if (PendingInvestments.Any(i => i.BuyData.TransactionId == buyOrder.Id))
            {
                SetStatusAndPrintAmmount(status, ref i, buys.Count);
                continue;
            }
            if (PotentialInvestments.Any(i => i.BuyData.TransactionId == buyOrder.Id))
            {
                SetStatusAndPrintAmmount(status, ref i, buys.Count);
                continue;
            }

            try
            {
                int buyAmmountLeft = buyOrder.Quantity;
                // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                var sellDataList = CheckHistorySellOrdersForCompleteInvestmentMatches(buyOrder, ref buyAmmountLeft, sortedSells);
                var buyData = new BuyData(buyOrder);

                // If there are previous sell orders ascociated with this buy order, its a completed investment
                if (sellDataList.Count > 0)
                {
                    // If there are left over bought items, remove them from this transaction because they did not sell
                    buyData.Quantity -= buyAmmountLeft;

                    var investment = new CompletedInvestment(buyData, sellDataList);
                    CompletedInvestments.Add(investment);
                }
                // If there are not any previous sell orders ascociated with this buy order that means it is either a pending investment or potential investment
                else
                {
                    Lazy<int> lazyCurrentSellPrice = new(() => Cache.Prices.GetPrice(buyOrder.ItemId));
                    // Make sure the sell transactions we look at are for the same item and only after the date the buy was purchase
                    var postedSellOrders = CheckPostedSellOrdersForMatches(buyOrder, ref buyAmmountLeft, sortedPostedSellOrders);
                    if (postedSellOrders.Count > 0)
                    {
                        var pendingInvestment = new PendingInvestment(buyData, postedSellOrders, lazyCurrentSellPrice);
                        PendingInvestments.Add(pendingInvestment);
                    }
                    else
                    {
                        var potentialInvsetment = new PotentialInvestment(buyData, lazyCurrentSellPrice);
                        PotentialInvestments.Add(potentialInvsetment);
                    }
                }
            }
            catch (System.Exception e)
            {
                GD.PushError(e);
            }

            SetStatusAndPrintAmmount(status, ref i, buys.Count);
        }
        AppStatusIndicator.ShowStatus($"{status} ({buys.Count}/{buys.Count})");
    }

    private static void SetStatusAndPrintAmmount(string status, ref int i, int buysCount)
    {
        AppStatusIndicator.ShowStatus($"{status} ({i}/{buysCount})");
        i++;
    }

    private List<SellData> CheckHistorySellOrdersForCompleteInvestmentMatches(CommerceTransactionHistory buyOrder, ref int buyAmmountLeft, List<CommerceTransactionHistory> sortedSellOrders)
    {
        var sellDatasList = new List<SellData>();
        // Look through sell orders that are of the same type of item, and was purchased at a later date from the buyOrder
        foreach (var sellOrder in sortedSellOrders.Where(s => s.ItemId == buyOrder.ItemId && s.Purchased > buyOrder.Purchased))
        {
            var sellData = new SellData(sellOrder);
            if (buyAmmountLeft > 0)
            {
                // Get all the other partial uses of this sell order
                var databasePartials = CompletedInvestments.SelectMany(i => i.SellDatas).Where(s => s.TransactionId == sellOrder.Id);

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
                        continue;
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

                sellDatasList.Add(sellData);
            }
            // We made sure all the buys are accounted for so we don't need to check any more sells
            else break;
        }
        return sellDatasList;
    }

    private List<SellData> CheckPostedSellOrdersForMatches(CommerceTransactionHistory buyOrder, ref int buyAmmountLeft, List<CommerceTransactionCurrent> sortedPostedSellOrders)
    {
        var pendingSellDataList = new List<SellData>();
        // Look through sell orders that are of the same type of item, and was created at a later date from the buyOrder
        foreach (var postedSellOrder in sortedPostedSellOrders.Where(s => s.ItemId == buyOrder.ItemId && s.Created > buyOrder.Purchased))
        {
            var pendingSellData = new SellData(postedSellOrder);
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
                        continue;
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
            var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.BuyData.ItemId && ci.IndividualBuyPrice == investment.BuyData.IndividualBuyPrice && ci.Quantity + investment.BuyData.Quantity <= Constants.MaxItemStack);
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

    public void GenerateCollapsedPending()
    {
        List<CollapsedPendingInvestment> groups = new();

        foreach (var investment in PendingInvestments)
        {
            var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.BuyData.ItemId && ci.IndividualBuyPrice == investment.BuyData.IndividualBuyPrice && ci.Quantity + investment.BuyData.Quantity <= Constants.MaxItemStack);
            if (readyGroup is not null)
            {
                readyGroup.SubInvestments.Add(investment);
            }
            else
            {
                var newCollapsedInvestment = new CollapsedPendingInvestment(investment);
                groups.Add(newCollapsedInvestment);
            }
        }

        groups.ForEach(c => CollapsedPendingInvestments.Add(c));
    }

    public void GenerateCollapsedPotential()
    {
        List<CollapsedPotentialInvestment> groups = new();

        foreach (var investment in PotentialInvestments)
        {
            var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.BuyData.ItemId && ci.IndividualBuyPrice == investment.BuyData.IndividualBuyPrice && ci.Quantity + investment.BuyData.Quantity <= Constants.MaxItemStack);
            if (readyGroup is not null)
            {
                readyGroup.SubInvestments.Add(investment);
            }
            else
            {
                var newCollapsedInvestment = new CollapsedPotentialInvestment(investment);
                groups.Add(newCollapsedInvestment);
            }
        }

        groups.ForEach(c => CollapsedPotentialInvestments.Add(c));
    }

    [DataContract]
    public class CompletedInvestmentsData
    {
        [DataMember] public List<CompletedInvestment> CompletedInvestments { get; internal set; } = new();
        [DataMember] public List<long> NotInvestments { get; internal set; } = new();
    }

    // ---------- Saving
    const string databasePath = "user://database.completed";
    public void Save()
    {
        var savedData = new CompletedInvestmentsData();
        savedData.CompletedInvestments = CompletedInvestments;
        savedData.NotInvestments = NotInvestments;
        SaveSystem.SaveToFile(databasePath, savedData);
    }

    // ---------- Loading
    public void Load()
    {
        if (SaveSystem.TryLoadFromFile(databasePath, out CompletedInvestmentsData newData))
        {
            CompletedInvestments = newData.CompletedInvestments ?? new();
            NotInvestments = newData.NotInvestments ?? new();
        }
        GD.Print($"Loaded Investment Database => investments: {CompletedInvestments.Count}, not investments: {NotInvestments.Count}");
    }
}
