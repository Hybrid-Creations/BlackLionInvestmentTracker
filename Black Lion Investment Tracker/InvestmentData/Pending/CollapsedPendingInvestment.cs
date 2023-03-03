using System;
using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;

namespace BLIT.Investments;

public class CollapsedPendingInvestment : CollapsedInvestment<PendingInvestment>
{
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
    public int TotalSellPrice => SubInvestments.Sum(si => si.TotalSellPrice);

    public double IndividualProfit => (SubInvestments.First().PostedSellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - SubInvestments.First().BuyData.IndividualBuyPrice;
    public double AverageIndividualProfit => SubInvestments.Average(i => i.AverageIndividualProfit);
    // Already has tax calculated
    public double TotalProfit => SubInvestments.Sum(si => si.TotalProfit);
    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderBy(i => i.BuyData.DatePurchased).First().BuyData.DatePurchased;

    public CollapsedPendingInvestment(params PendingInvestment[] subInvestments)
    {
        SubInvestments = subInvestments.ToList();
    }

    public string GetSellPriceStringFromInvestment(bool includeIndividualPrice, RichStringAlignment alignment)
    {
        var individualPrefix = AllSellDatasAreTheSame ? "each" : "avg";
        string sellPrice = (AllSellDatasAreTheSame ? IndividualSellPrice : AverageIndividualSellPrice).ToCurrencyString(true);
        return $"{$"[{alignment}]".ToLower()}{TotalSellPrice.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]{individualPrefix}[/color] {sellPrice}" : "")}";
    }

    public string GetProfitStringFromInvestment(bool includeIndividualProfit, RichStringAlignment alignment)
    {
        var individualPrefix = AllSellDatasAreTheSame ? "each" : "avg";
        string profit = (AllSellDatasAreTheSame ? IndividualProfit : AverageIndividualProfit).ToCurrencyString(true);
        return $"{$"[{alignment}]".ToLower()}{TotalProfit.ToCurrencyString(true)}{(includeIndividualProfit ? $"\n[color=gray]{individualPrefix}[/color] {profit}" : "")}";
    }

}
