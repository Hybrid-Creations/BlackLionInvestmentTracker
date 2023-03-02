using System.Collections.Generic;
using System.Linq;

namespace BLIT.Investments;

public class CollapsedInvestment<TInvestment>
where TInvestment : Investment
{
    public List<TInvestment> SubInvestments { get; protected set; }

    public int ItemId => SubInvestments.First().BuyData.ItemId;
    public int Quantity => SubInvestments.Sum(i => i.BuyData.Quantity);

    public int IndividualBuyPrice => SubInvestments.First().BuyData.IndividualBuyPrice;
    public int TotalBuyPrice => SubInvestments.Sum(i => i.BuyData.TotalBuyPrice);

    public CollapsedInvestment(params TInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }
}
