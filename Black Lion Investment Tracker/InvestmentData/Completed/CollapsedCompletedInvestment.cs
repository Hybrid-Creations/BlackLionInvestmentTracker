using System;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedCompletedInvestment : CollapsedInvestment<CompletedInvestment>
{
    public int AverageIndividualSellPrice => SubInvestments.First().AverageIndividualSellPrice;
    public int TotalSellPrice => SubInvestments.Sum(si => si.TotalSellPrice);

    // Already has tax calculated
    public int TotalProfit => SubInvestments.Sum(si => si.TotalProfit);
    public int AverageIndividualProfit => SubInvestments.First().AverageIndividualProfit;

    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderBy(i => i.BuyData.DatePurchased).First().BuyData.DatePurchased;

    public CollapsedCompletedInvestment(params CompletedInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }

    public CollapsedCompletedInvestment() { }
}
