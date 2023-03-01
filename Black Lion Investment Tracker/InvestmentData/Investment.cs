using System;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class Investment<TInvestmentData, TBuyData, TSellData>
where TInvestmentData : InvestmentData<TBuyData, TSellData>
where TBuyData : BuyData
where TSellData : SellData
{
    public TInvestmentData Data { get; protected set; }

    public int IndividualBuyPrice => Data.BuyData.IndividualBuyPrice;
    public int IndividualSellPrice => Data.AscociatedSellDatas.First().IndividualSellPrice;
    public int SellQuantity => Data.AscociatedSellDatas.Sum(s => s.Quantity);

    public int TotalBuyPrice => Data.BuyData.IndividualBuyPrice * Data.BuyData.Quantity;
    public int TotalSellPrice => Data.AscociatedSellDatas.Sum(s => s.IndividualSellPrice * s.Quantity);
    public int Quantity => Data.BuyData.Quantity;
    public DateTimeOffset LatestSellDate => Data.AscociatedSellDatas.OrderBy(d => d.Date).First().Date;

    // The Total We Sold For Reduced By The 15% BLTP Tax Minus The Total We Bought The Items For
    public int Profit => (int)Math.Floor((TotalSellPrice * Constants.MultiplyTax) - TotalBuyPrice);
    public double ROI => Profit / (double)TotalBuyPrice * 100;

    public Investment(TInvestmentData data)
    {
        Data = data;
    }

}
