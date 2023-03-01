using System;
using System.Collections.Generic;

namespace BLIT.Investments;

public sealed class PotentialInvestmentData : InvestmentData<BuyData, PotentialSellData>
{
    Lazy<int> LazyIndividualSellPrice { get; set; }

    public List<PotentialSellData> PostedSellDatas => AscociatedSellDatas;

    public int CurrentIndividualSellPrice => LazyIndividualSellPrice.Value;

    public PotentialInvestmentData(BuyData buyData, List<PotentialSellData> sellDatas, Lazy<int> currentSellPrice) : base(buyData, sellDatas)
    {
        LazyIndividualSellPrice = currentSellPrice;
    }
}
