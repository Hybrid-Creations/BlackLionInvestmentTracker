using BLIT.ConstantVariables;
using BLIT.Investments;

namespace BLIT.Extensions;

public static class DataExtentions
{
    // Buy
    public static string GetBuyPriceStringFromInvestment<TCollapsedInvestment, TInvestment, TInvestmentData, TBuyData, TSellData>(this TCollapsedInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    where TCollapsedInvestment : CollapsedInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>
    where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
    where TInvestmentData : InvestmentData<TBuyData, TSellData>
    where TBuyData : BuyData
    where TSellData : SellData
    {
        return GetBuyPriceString(collapsedInvestment.TotalBuyPrice, collapsedInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>(this TInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
    where TInvestmentData : InvestmentData<TBuyData, TSellData>
    where TBuyData : BuyData
    where TSellData : SellData
    {
        return GetBuyPriceString(investment.TotalBuyPrice, investment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    private static string GetBuyPriceString(int totalBuyPrice, int individualBuyPrice, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return $"{$"[{alignment}]".ToLower()}{totalBuyPrice.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]each[/color] {individualBuyPrice.ToCurrencyString(true)}" : "")}";
    }

    //Sell
    public static string GetSellPriceStringFromInvestment<TCollapsedInvestment, TInvestment, TInvestmentData, TBuyData, TSellData>(this TCollapsedInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    where TCollapsedInvestment : CollapsedInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>
    where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
    where TInvestmentData : InvestmentData<TBuyData, TSellData>
    where TBuyData : BuyData
    where TSellData : SellData
    {
        return GetSellPriceString(collapsedInvestment.TotalSellPrice, collapsedInvestment.IndividualSellPrice, includeIndividualPrice, alignment);
    }

    public static string GetSellPriceStringFromInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>(this TInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
    where TInvestmentData : InvestmentData<TBuyData, TSellData>
    where TBuyData : BuyData
    where TSellData : SellData
    {
        return GetSellPriceString(investment.TotalSellPrice, investment.IndividualSellPrice, includeIndividualPrice, alignment);
    }

    private static string GetSellPriceString(int totalSellPrice, int individualSellPrice, bool includeIndividualPrice, RichStringAlignment alignment)

    {
        return $"{$"[{alignment}]".ToLower()}{totalSellPrice.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]each[/color] {individualSellPrice.ToCurrencyString(true)}" : "")}";
    }
}
