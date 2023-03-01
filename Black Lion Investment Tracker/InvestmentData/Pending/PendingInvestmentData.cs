using System;
using System.Collections.Generic;

namespace BLIT.Investments;

public class PendingInvestmentData : InvestmentData<BuyData, PendingSellData>
{
    Lazy<int> LazyIndividualSellPrice { get; set; }

    public List<PendingSellData> PostedSellDatas => AscociatedSellDatas;

    public int CurrentIndividualSellPrice => LazyIndividualSellPrice.Value;

    public PendingInvestmentData(BuyData buyData, List<PendingSellData> sellDatas, Lazy<int> currentSellPrice) : base(buyData, sellDatas)
    {
        LazyIndividualSellPrice = currentSellPrice;
    }
}
