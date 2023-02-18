using System.Collections.Generic;
using System.Linq;

namespace BLIT;

public class InvestmentsDatabase
{
    public readonly List<InvestmentData> Investments;
    public readonly List<InvestmentData> CollapsedInvestments;
    public long TotalInvested => Investments.Sum(i => i.Price * i.Quantity);
    public long TotalReturn => Investments.SelectMany(i => i.SellDatas).Sum(s => s.Price * s.Quantity);
    public long TotalProfit => Investments.Sum(i => i.Profit);
    public double ROI => TotalProfit / (double)TotalInvested * 100;

    public InvestmentsDatabase()
    {
        Investments = new List<InvestmentData>();
        CollapsedInvestments = new List<InvestmentData>();
    }

    public void GenerateCollapsed()
    {
        List<List<InvestmentData>> groups = new();
        List<InvestmentData> currentGroup = new();
        int currentId = Investments.First().ItemId;
        int currentPrice = Investments.First().Price;
        int quantity = Investments.First().Quantity;

        foreach (var investment in Investments)
        {
            if (investment.ItemId == currentId && investment.Price == currentPrice)
                currentGroup.Add(investment);
            else
            {
                groups.Add(currentGroup);
                currentGroup = new();
                currentId = investment.ItemId;
                currentPrice = investment.Price;
                currentGroup.Add(investment);
            }
        }
        groups.Add(currentGroup);

        foreach (var group in groups)
        {
            CollapsedInvestments.Add(group.First());
            foreach (var investment in group.Skip(1))
            {
                CollapsedInvestments.Last().Quantity += investment.Quantity;
            }
        }
    }
}
