using System;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedPotentialInvestment : CollapsedInvestment<PotentialInvestment>
{
    public int IndividualPotentialSellPrice => SubInvestments.First().IndividualPotentialSellPrice;
    public int TotalPotentialSellPrice => SubInvestments.Sum(i => i.TotalPotentialSellPrice);

    /// <summary>
    /// Already has the tax calculated on the <see cref="PotentialInvestment"/> in <see cref="SubInvestments"/>.
    /// </summary>
    public int IndividualPotentialProfit => SubInvestments.First().IndividualPotentialProfit;
    /// <summary>
    /// Already has the tax calculated on the <see cref="PotentialInvestment"/> in <see cref="SubInvestments"/>.
    /// </summary>
    public int TotalPotentialProfit => SubInvestments.Sum(i => i.TotalPotentialProfit);

    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderBy(i => i.BuyData.DatePurchased).First().BuyData.DatePurchased;

    public CollapsedPotentialInvestment(params PotentialInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }
}
