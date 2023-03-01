using System.Collections.Generic;

namespace BLIT.Investments;

// // ~~~~~


// // ~~~~~

public sealed class PotentialInvestmentData : InvestmentData<BuyData, SellData>
{
    public PotentialInvestmentData(BuyData buyData, List<SellData> sellDatas) : base(buyData, sellDatas) { }
}
