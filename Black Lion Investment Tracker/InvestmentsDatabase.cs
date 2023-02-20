using System.Collections.Generic;
using System.Linq;

namespace BLIT;

public class InvestmentsDatabase
{
    public readonly List<InvestmentData> Investments;
    public readonly List<CollapsedInvestmentData> CollapsedInvestments;
    public long TotalInvested => Investments.Sum(i => i.TotalBuyPrice);
    public long TotalReturn => Investments.SelectMany(i => i.SellDatas).Sum(s => s.TotalSellPrice);
    public long TotalProfit => Investments.Sum(i => i.Profit);
    public double ROI => TotalProfit / (double)TotalInvested * 100;

    public InvestmentsDatabase()
    {
        Investments = new List<InvestmentData>();
        CollapsedInvestments = new List<CollapsedInvestmentData>();
    }

    public void GenerateCollapsed()
    {
        List<CollapsedInvestmentData> groups = new();

        foreach (var investment in Investments)
        {
            var readyGroup = groups.FirstOrDefault(c => c.ItemId == investment.ItemId && c.IndividualPrice == investment.IndividualPrice && c.Quantity + investment.Quantity < Constants.MaxItemStack);
            if (readyGroup is not null)
            {
                readyGroup.SubInvestments.Add(investment);
            }
            else
            {
                var newCollapsedInvestment = new CollapsedInvestmentData(investment);
                newCollapsedInvestment.SubInvestments.Add(investment);
                groups.Add(newCollapsedInvestment);
            }
        }

        groups.ForEach(c => CollapsedInvestments.Add(c));
    }
}
