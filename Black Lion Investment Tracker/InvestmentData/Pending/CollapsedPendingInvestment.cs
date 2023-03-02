using System;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedPendingInvestment : CollapsedInvestment<PendingInvestment>
{

    public int IndividualPotentialSellPrice => SubInvestments.First().AverageIndividualPostedSellPrice;
    public int TotalPotentialSellPrice => SubInvestments.Sum(i => i.TotalPostedSellPrice);

    /// <summary>
    /// Already has the tax calculated on the <see cref="PendingInvestment"/> in <see cref="SubInvestments"/>.
    /// </summary>
    public int IndividualPotentialProfit => SubInvestments.First().AverageIndividualPotentialProfit;
    /// <summary>
    /// Already has the tax calculated on the <see cref="PendingInvestment"/> in <see cref="SubInvestments"/>.
    /// </summary>
    public int TotalPotentialProfit => SubInvestments.Sum(i => i.TotalPotentialProfit);

    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderBy(i => i.BuyData.DatePurchased).First().BuyData.DatePurchased;

    public CollapsedPendingInvestment(params PendingInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }
}
