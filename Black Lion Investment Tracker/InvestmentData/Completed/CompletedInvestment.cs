using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using Godot;

namespace BLIT.Investments;

[DataContract]
public class CompletedInvestment : Investment
{
    [DataMember] internal List<SellData> SellDatas { get; private set; } = new();
    public DateTimeOffset LatestSellDate => SellDatas.OrderBy(s => s.Date).First().Date;

    public bool AllSellDatasAreTheSame
    {
        get
        {
            var firstPrice = SellDatas.First().IndividualSellPrice;
            return SellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public int IndividualSellPrice => SellDatas.First().IndividualSellPrice;
    public double AverageIndividualSellPrice => SellDatas.Average(s => s.IndividualSellPrice);
    public int TotalSellPrice => SellDatas.Sum(s => s.TotalSellPrice);

    public double IndividualProfit => (SellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice;
    /// <summary>
    /// The Average Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public double AverageIndividualProfit => TotalProfit / SellDatas.Count;
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public double TotalProfit => (TotalSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice;
    public CompletedInvestment(BuyData buyData, List<SellData> sellDatas) : base(buyData)
    {
        SellDatas = sellDatas;
    }

    public CompletedInvestment() { }

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
