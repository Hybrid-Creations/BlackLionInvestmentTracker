using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIT;

public class CollapsedPendingInvestmentData
{
    public int ItemId;
    public int IndividualPrice;
    public Lazy<int> CurrentIndividualSellPrice;

    public int Quantity => SubInvestments.Sum(s => s.Quantity);
    public int TotalBuyPrice => SubInvestments.Sum(i => i.TotalBuyPrice);
    public int TotalPotentialSellPrice => SubInvestments.Sum(i => i.TotalPotentialSellPrice);
    // The sub investments already account for the BLTP 15% tax
    public int TotalPotentialProfit => SubInvestments.Sum(i => i.PotentialProfit);
    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderByDescending(i => i.PurchaseDate).First().PurchaseDate;

    public List<PendingInvestmentData> SubInvestments;

    public CollapsedPendingInvestmentData(PendingInvestmentData firstInvestment, Lazy<int> currentSellPrice)
    {
        ItemId = firstInvestment.ItemId;
        IndividualPrice = firstInvestment.IndividualPrice;
        SubInvestments = new List<PendingInvestmentData>
        {
            firstInvestment
        };
        CurrentIndividualSellPrice = currentSellPrice;
    }
}
