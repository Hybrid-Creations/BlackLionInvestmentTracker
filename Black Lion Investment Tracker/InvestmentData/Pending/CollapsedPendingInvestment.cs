using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedPendingInvestment
{
    public List<PendingInvestment> SubInvestments { get; protected set; }

    public int ItemId => SubInvestments.First().BuyData.ItemId;
    public int Quantity => SubInvestments.Sum(i => i.BuyData.Quantity);

    public int IndividualBuyPrice => SubInvestments.First().BuyData.IndividualBuyPrice;
    public int TotalBuyPrice => SubInvestments.Sum(i => i.BuyData.TotalBuyPrice);

    public int IndividualPotentialSellPrice => SubInvestments.First().IndividualPostedSellPrice;
    public int TotalPotentialSellPrice => SubInvestments.Sum(i => i.TotalPostedSellPrice);

    /// <summary>
    /// Already has the tax calculated on the <see cref="PendingInvestment"/> in <see cref="SubInvestments"/>.
    /// </summary>
    public int IndividualPotentialProfit => SubInvestments.First().IndividualPotentialProfit;
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
