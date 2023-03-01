using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>
where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
where TInvestmentData : InvestmentData<TBuyData, TSellData>
where TBuyData : BuyData
where TSellData : SellData
{
    public IList<TInvestment> SubInvestments { get; protected set; }

    public int ItemId => SubInvestments.First().Data.BuyData.ItemId;

    public int IndividualBuyPrice => SubInvestments.First().IndividualBuyPrice;
    public int IndividualSellPrice => SubInvestments.First().IndividualSellPrice;

    public int Quantity => SubInvestments.Sum(s => s.Quantity);
    public int TotalBuyPrice => SubInvestments.Sum(i => i.TotalBuyPrice);
    public int TotalSellPrice => SubInvestments.Sum(i => i.TotalSellPrice);
    // The sub investments already account for the BLTP 15% tax
    public int TotalProfit => SubInvestments.Sum(i => i.Profit);
    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderByDescending(i => i.Data.BuyData.DatePurchased).First().Data.BuyData.DatePurchased;

    public CollapsedInvestment(params TInvestment[] subInvestments)
    {
        SubInvestments = subInvestments;
    }
}
