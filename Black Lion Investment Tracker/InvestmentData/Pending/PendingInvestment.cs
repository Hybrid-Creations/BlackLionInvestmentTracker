using System;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class PendingInvestment : Investment<PendingInvestmentData, BuyData, PendingSellData>
{
    public int TotalPotentialSellPrice => Data.CurrentIndividualSellPrice * Data.PostedSellDatas.Sum(p => p.Quantity);

    // // The Total We Sold For Reduced By The 15% BLTP Tax Minus The Total We Bought The Items For
    public int PotentialProfit => (int)Math.Floor((TotalPotentialSellPrice * Constants.MultiplyTax) - TotalBuyPrice);
    public double PotentialROI => PotentialProfit / (double)TotalBuyPrice * 100;

    public PendingInvestment(PendingInvestmentData data) : base(data) { }
}
