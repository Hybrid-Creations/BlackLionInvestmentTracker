using System.Collections.Generic;

namespace BLIT.Investments;

public sealed class CompletedInvestmentData : InvestmentData<BuyData, SellData>
{
    public List<SellData> SellDatas => AscociatedSellDatas;
    public CompletedInvestmentData(BuyData buyData, List<SellData> sellDatas) : base(buyData, sellDatas) { }
}
