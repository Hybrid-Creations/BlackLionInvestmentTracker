using System.Collections.Generic;

namespace BLIT.Investments;

public class InvestmentData<TBuyData, TSellData>
where TBuyData : BuyData
where TSellData : SellData
{
    public TBuyData BuyData { get; protected set; }
    internal List<TSellData> AscociatedSellDatas { get; set; } = new();

    public InvestmentData(TBuyData buyData, List<TSellData> sellDatas)
    {
        BuyData = buyData;
        AscociatedSellDatas = sellDatas;
    }
}
