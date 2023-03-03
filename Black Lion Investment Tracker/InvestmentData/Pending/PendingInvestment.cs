using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class PendingInvestment : Investment
{
    internal List<SellData> PostedSellDatas { get; private set; } = new();

    public bool AllSellDatasAreTheSame
    {
        get
        {
            var firstPrice = PostedSellDatas.First().IndividualSellPrice;
            return PostedSellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public int IndividualSellPrice => PostedSellDatas.First().IndividualSellPrice;
    public double AverageIndividualSellPrice => PostedSellDatas.Average(s => s.IndividualSellPrice);
    public int TotalSellPrice => PostedSellDatas.Sum(s => s.TotalSellPrice);

    public double IndividualProfit => (PostedSellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice;
    /// <summary>
    /// The Average Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public double AverageIndividualProfit => TotalProfit / PostedSellDatas.Count;
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public double TotalProfit => (TotalSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice;

    public PendingInvestment(BuyData buyData, List<SellData> sellDatas) : base(buyData)
    {
        PostedSellDatas = sellDatas;
    }
}
