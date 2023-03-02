using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;
using Godot;

namespace BLIT.Investments;

public class PendingInvestment : Investment
{
    internal List<SellData> PostedSellDatas { get; private set; } = new();

    public int AverageIndividualPostedSellPrice { get; private set; }
    public int TotalPostedSellPrice { get; private set; }

    /// <summary>
    /// The Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public int AverageIndividualPotentialProfit { get; private set; }
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public int TotalPotentialProfit { get; private set; }

    public PendingInvestment(BuyData buyData, List<SellData> sellDatas) : base(buyData)
    {
        PostedSellDatas = sellDatas;

        AverageIndividualPostedSellPrice = Mathf.RoundToInt(PostedSellDatas.Average(p => p.IndividualSellPrice));
        TotalPostedSellPrice = PostedSellDatas.Sum(s => s.IndividualSellPrice * s.Quantity);

        AverageIndividualPotentialProfit = Mathf.FloorToInt((AverageIndividualPostedSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice);
        TotalPotentialProfit = Mathf.FloorToInt((TotalPostedSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice);
    }
}
