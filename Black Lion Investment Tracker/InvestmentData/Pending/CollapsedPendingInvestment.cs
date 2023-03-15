using System;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class CollapsedPendingInvestment : CollapsedInvestment<PendingInvestment>
{
    public int LowestIndividualSellPrice => SubInvestments.SelectMany(i => i.PostedSellDatas).Max(s => s.IndividualSellPrice);

    public bool AllSellDatasAreTheSame
    {
        get
        {
            var allSellDatas = SubInvestments.SelectMany(i => i.PostedSellDatas);
            var firstPrice = allSellDatas.First().IndividualSellPrice;
            return allSellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public int IndividualSellPrice => SubInvestments.First().PostedSellDatas.First().IndividualSellPrice;
    public double AverageIndividualSellPrice => SubInvestments.SelectMany(i => i.PostedSellDatas).Average(s => s.IndividualSellPrice);
    public int TotalSellPrice => SubInvestments.Sum(si => si.TotalListedSellPrice);

    public double IndividualProfit => (SubInvestments.First().PostedSellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - SubInvestments.First().BuyData.IndividualBuyPrice;
    public double AverageIndividualProfit => SubInvestments.Average(i => i.AverageIndividualProfit);
    // Already has tax calculated
    public double TotalProfit => SubInvestments.Sum(si => si.TotalProfit);
    public DateTimeOffset OldestPurchaseDate => SubInvestments.Max(i => i.BuyData.DatePurchased);

    public CollapsedPendingInvestment(params PendingInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }
}
