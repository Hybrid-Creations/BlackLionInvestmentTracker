using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

public class SellData
{
    public virtual long TransactionId { get; protected set; }
    public virtual int ItemId { get; protected set; }
    public virtual int Quantity { get; internal set; }
    public virtual int IndividualSellPrice { get; protected set; }
    internal virtual DateTimeOffset Date { get; set; }

    protected SellData() { }

    public SellData(CommerceTransactionHistory sellTransaction)
    {
        if (sellTransaction is null) return;

        TransactionId = sellTransaction.Id;
        ItemId = sellTransaction.ItemId;
        Date = sellTransaction.Created;
        IndividualSellPrice = sellTransaction.Price;
        Quantity = sellTransaction.Quantity;
    }
}

public class BuyData
{
    public long TransactionId { get; protected set; }
    public int ItemId { get; protected set; }
    public int Quantity { get; internal set; }
    public int IndividualBuyPrice { get; protected set; }
    public DateTimeOffset DatePurchased { get; protected set; }

    public BuyData(CommerceTransactionHistory buyTransaction)
    {
        TransactionId = buyTransaction.Id;
        ItemId = buyTransaction.ItemId;
        Quantity = buyTransaction.Quantity;
        IndividualBuyPrice = buyTransaction.Price;
        DatePurchased = buyTransaction.Purchased;
    }
}

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

public class CollapsedInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>
where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
where TInvestmentData : InvestmentData<TBuyData, TSellData>
where TBuyData : BuyData
where TSellData : SellData
{
    public IList<TInvestment> SubInvestments { get; protected set; }

    public int ItemId => SubInvestments.First().Data.BuyData.ItemId;

    public int IndividualBuyPrice => SubInvestments.First().IndividualBuyPrice;
    public int IndividualSellPrice => SubInvestments.First().IndividualSellPrice;

    public int Quantity => SubInvestments.Sum(s => s.Quantity);
    public int TotalBuyPrice => SubInvestments.Sum(i => i.TotalBuyPrice);
    public int TotalSellPrice => SubInvestments.Sum(i => i.TotalSellPrice);
    // The sub investments already account for the BLTP 15% tax
    public int TotalProfit => SubInvestments.Sum(i => i.Profit);
    public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderByDescending(i => i.Data.BuyData.DatePurchased).First().Data.BuyData.DatePurchased;

    public CollapsedInvestment(params TInvestment[] subInvestments)
    {
        SubInvestments = subInvestments;
    }
}

// // ~~~~~

public sealed class CompletedInvestmentData : InvestmentData<BuyData, SellData>
{
    public List<SellData> SellDatas => AscociatedSellDatas;
    public CompletedInvestmentData(BuyData buyData, List<SellData> sellDatas) : base(buyData, sellDatas) { }
}

public sealed class CompletedInvestment : Investment<CompletedInvestmentData, BuyData, SellData>
{
    public CompletedInvestment(CompletedInvestmentData data) : base(data) { }
}

public sealed class CollapsedCompletedInvestment : CollapsedInvestment<CompletedInvestment, CompletedInvestmentData, BuyData, SellData>
{
    public CollapsedCompletedInvestment(params CompletedInvestment[] subInvestments) : base(subInvestments) { }
}

// // ~~~~~


// // ~~~~~

public sealed class PotentialInvestmentData : InvestmentData<BuyData, SellData>
{
    public PotentialInvestmentData(BuyData buyData, List<SellData> sellDatas) : base(buyData, sellDatas) { }
}

public sealed class PotentialInvestment : Investment<PotentialInvestmentData, BuyData, SellData>
{
    public PotentialInvestment(PotentialInvestmentData data) : base(data) { }
}

public sealed class CollapsedPotentialnvestment : CollapsedInvestment<PotentialInvestment, PotentialInvestmentData, BuyData, SellData>
{
    public CollapsedPotentialnvestment(params PotentialInvestment[] subInvestments) : base(subInvestments) { }
}
