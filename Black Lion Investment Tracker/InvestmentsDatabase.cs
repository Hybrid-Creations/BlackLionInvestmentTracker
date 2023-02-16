using System.Collections.Generic;
using System.Linq;

namespace BLIT;

public class InvestmentsDatabase
{
    public readonly List<InvestmentData> Investments;
    public long TotalInvested => Investments.Sum(i => i.Price * i.Quantity);
    public long TotalReturn => Investments.SelectMany(i => i.SellDatas).Sum(s => s.Price * s.Quantity);
    public long TotalProfit => Investments.Sum(i => i.Profit);
    public double ROI => TotalProfit / (double)TotalInvested * 100;

    public InvestmentsDatabase()
    {
        Investments = new List<InvestmentData>();
    }
}
