using System;
using System.Linq;
using Godot;

namespace BLIT.Investments;

public class CollapsedPendingInvestment : CollapsedInvestment<PendingInvestment>
{
    public int LowestIndividualSellPrice => SubInvestments.SelectMany(i => i.SellDatas).Max(s => s.IndividualSellPrice);

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

    public override int AverageIndividualProfit => Mathf.RoundToInt(SubInvestments.Average(i => i.AverageIndividualProfit));
    public override int TotalProfit => SubInvestments.Sum(si => si.TotalNetProfit);

    public DateTimeOffset OldestPurchaseDate => SubInvestments.Max(i => i.BuyData.DatePurchased);

    public CollapsedPendingInvestment(params PendingInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }
}
