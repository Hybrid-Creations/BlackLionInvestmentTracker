using System;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class CollapsedCompletedInvestment : CollapsedInvestment<CompletedInvestment>
{
    public bool AllSellDatasAreTheSame
    {
        get
        {
            var allSellDatas = SubInvestments.SelectMany(i => i.SellDatas);
            var firstPrice = allSellDatas.First().IndividualSellPrice;
            return allSellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public int IndividualSellPrice => SubInvestments.First().SellDatas.First().IndividualSellPrice;
    public double AverageIndividualSellPrice => SubInvestments.SelectMany(i => i.SellDatas).Average(s => s.IndividualSellPrice);
    public int TotalSellPrice => SubInvestments.Sum(si => si.TotalSellPrice);


    public double IndividualProfit => (SubInvestments.First().SellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - SubInvestments.First().BuyData.IndividualBuyPrice;
    public double AverageIndividualProfit => SubInvestments.Average(i => i.AverageIndividualProfit);
    // Already has tax calculated
    public double TotalProfit => SubInvestments.Sum(si => si.TotalProfit);
    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderBy(i => i.BuyData.DatePurchased).First().BuyData.DatePurchased;
    public DateTimeOffset NewestSellDate => SubInvestments.SelectMany(i => i.SellDatas).OrderByDescending(s => s.Date).First().Date;

    public CollapsedCompletedInvestment(params CompletedInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }

    public CollapsedCompletedInvestment() { }
}
