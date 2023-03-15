using System;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedPotentialInvestment : CollapsedInvestment<PotentialInvestment>
{
    private int currentSellPrice;

    public int CurrentSellPrice
    {
        get => currentSellPrice;
        set
        {
            SubInvestments.ForEach(s => s.CurrentSellPrice = value);
            currentSellPrice = value;
        }
    }
    public int TotalPotentialSellPrice => SubInvestments.Sum(i => i.TotalPotentialSellPrice);

    /// <summary>
    /// Already has the tax calculated on the <see cref="PotentialInvestment"/> in <see cref="SubInvestments"/>.
    /// </summary>
    public int IndividualPotentialProfit => SubInvestments.First().IndividualPotentialProfit;
    /// <summary>
    /// Already has the tax calculated on the <see cref="PotentialInvestment"/> in <see cref="SubInvestments"/>.
    /// </summary>
    public int TotalPotentialProfit => SubInvestments.Sum(i => i.TotalPotentialProfit);

    public DateTimeOffset OldestPurchaseDate => SubInvestments.Max(i => i.BuyData.DatePurchased);

    public CollapsedPotentialInvestment(params PotentialInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }
}
