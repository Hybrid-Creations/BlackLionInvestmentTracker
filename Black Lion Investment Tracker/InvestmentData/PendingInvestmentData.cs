using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class PendingInvestmentData : InvestmentData<BuyData, PendingSellData>
{
    Lazy<int> LazyIndividualSellPrice { get; set; }

    public List<PendingSellData> PostedSellDatas => AscociatedSellDatas;

    public int CurrentIndividualSellPrice => LazyIndividualSellPrice.Value;
    // public long TransactionId;
    // public int ItemId;
    // public DateTimeOffset PurchaseDate;
    // public int IndividualPrice;
    // public int Quantity;
    // public Lazy<int> CurrentIndividualSellPrice;
    // public List<PendingSellData> PostedSellDatas = new();

    public PendingInvestmentData(BuyData buyData, List<PendingSellData> sellDatas, Lazy<int> currentSellPrice) : base(buyData, sellDatas)
    {
        LazyIndividualSellPrice = currentSellPrice;
    }
}

public class PendingInvestment : Investment<PendingInvestmentData, BuyData, PendingSellData>
{
    public int TotalPotentialSellPrice => Data.CurrentIndividualSellPrice * Data.PostedSellDatas.Sum(p => p.Quantity);

    // // The Total We Sold For Reduced By The 15% BLTP Tax Minus The Total We Bought The Items For
    public int PotentialProfit => (int)Math.Floor((TotalPotentialSellPrice * Constants.MultiplyTax) - TotalBuyPrice);
    public double PotentialROI => PotentialProfit / (double)TotalBuyPrice * 100;

    public PendingInvestment(PendingInvestmentData data) : base(data) { }
}

public sealed class CollapsedPendingInvestment : CollapsedInvestment<PendingInvestment, PendingInvestmentData, BuyData, PendingSellData>
{
    public int CurrentIndividualSellPrice => SubInvestments.First().Data.CurrentIndividualSellPrice;
    public int TotalPotentialProfit => SubInvestments.Sum(i => i.PotentialProfit);
    public CollapsedPendingInvestment(params PendingInvestment[] subInvestments) : base(subInvestments) { }
}