using System;
using System.Linq;
using Godot;

namespace BLIT.Investments;

public class CollapsedCompletedInvestment : CollapsedInvestment<CompletedInvestment>
{
    public int IndividualSellPrice { get; private set; }
    public int TotalSellPrice => SubInvestments.Sum(i => i.TotalSellPrice);

    // Already has tax calculated
    public int TotalProfit => SubInvestments.Sum(i => i.TotalProfit);
    public int IndividualProfit => SubInvestments.First().AverageIndividualProfit;

    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderBy(i => i.BuyData.DatePurchased).First().BuyData.DatePurchased;

    public CollapsedCompletedInvestment(params CompletedInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
        IndividualSellPrice = Mathf.RoundToInt(SubInvestments.Average(s => s.AverageIndividualSellPrice));
    }
}
