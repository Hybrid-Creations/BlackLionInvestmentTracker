using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedCompletedInvestment
{
    public List<CompletedInvestment> SubInvestments { get; protected set; }

    public int ItemId => SubInvestments.First().BuyData.ItemId;
    public int Quantity => SubInvestments.Sum(i => i.BuyData.Quantity);

    public int IndividualBuyPrice => SubInvestments.First().BuyData.IndividualBuyPrice;
    public int TotalBuyPrice => SubInvestments.Sum(i => i.BuyData.TotalBuyPrice);

    public int IndividualSellPrice => SubInvestments.First().IndividualSellPrice;
    public int TotalSellPrice => SubInvestments.Sum(i => i.TotalSellPrice);

    // Already has tax calculated
    public int TotalProfit => SubInvestments.Sum(i => i.Profit);
    public int IndividualProfit => SubInvestments.First().IndividualProfit;

    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderBy(i => i.BuyData.DatePurchased).First().BuyData.DatePurchased;

    public CollapsedCompletedInvestment(params CompletedInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }
}
