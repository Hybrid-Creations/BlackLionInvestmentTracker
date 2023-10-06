using System;
using System.Linq;
using Godot;

namespace BLIT.Investments;

public class CollapsedCompletedInvestment : CollapsedInvestment<CompletedInvestment>
{
    public override bool IndividualSellPriceDifferent
    {
        get
        {
            var allSellDatas = SubInvestments.SelectMany(i => i.SellDatas);
            var firstPrice = allSellDatas.First().IndividualSellPrice;
            return !allSellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public override int IndividualSellPrice => SubInvestments.First().SellDatas.First().IndividualSellPrice;
    public override int AverageIndividualSellPrice => Mathf.RoundToInt(SubInvestments.SelectMany(i => i.SellDatas).Average(s => s.IndividualSellPrice));
    public override int TotalSellPrice => SubInvestments.Sum(si => si.TotalSellPrice);
    public int TotalNetSellPrice => SubInvestments.Sum(si => si.TotalNetSellPrice);

    public override int AverageIndividualProfit => Mathf.RoundToInt(SubInvestments.Average(i => i.AverageIndividualProfit));

    // Already has tax calculated
    public override int TotalProfit => SubInvestments.Sum(si => si.TotalNetProfit);
    public DateTimeOffset OldestPurchaseDate => SubInvestments.Min(i => i.BuyData.DatePurchased);
    public DateTimeOffset NewestSellDate => SubInvestments.SelectMany(i => i.SellDatas).Max(s => s.Date);

    public DateTimeOffset LastActiveDate => SubInvestments.SelectMany(i => i.SellDatas).Select(s => s.Date).Union(SubInvestments.Select(i => i.BuyData.DatePurchased)).Max();

    public CollapsedCompletedInvestment(params CompletedInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }

    public CollapsedCompletedInvestment() { }
}
