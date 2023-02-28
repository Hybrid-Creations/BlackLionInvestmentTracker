using BLIT.ConstantVariables;
using BLIT.Investments;

namespace BLIT.Extensions;

public static class DataExtentions
{
    // Buy
    public static string GetBuyPriceStringFromInvestment(this CollapsedCompletedInvestment _collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceStringFromInvestment(_collapsedInvestment.TotalBuyPrice, _collapsedInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this CollapsedPendingInvestment _collapsedPendingInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceStringFromInvestment(_collapsedPendingInvestment.TotalBuyPrice, _collapsedPendingInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this CompletedInvestment _collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceStringFromInvestment(_collapsedInvestment.TotalBuyPrice, _collapsedInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this PendingInvestment _collapsedPendingInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceStringFromInvestment(_collapsedPendingInvestment.TotalBuyPrice, _collapsedPendingInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    private static string GetBuyPriceStringFromInvestment(int totalPrice, int individualPrice, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return $"{$"[{alignment}]".ToLower()}{totalPrice.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]each[/color] {individualPrice.ToCurrencyString(true)}" : "")}";
    }

    //Sell
    public static string GetSellPriceStringFromInvestment(this CollapsedCompletedInvestment _collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetSellPriceStringFromInvestment(_collapsedInvestment.TotalSellPrice, _collapsedInvestment.IndividualSellPrice, includeIndividualPrice, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this CollapsedPendingInvestment _collapsedPendingInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetSellPriceStringFromInvestment(_collapsedPendingInvestment.TotalBuyPrice, _collapsedPendingInvestment.CurrentIndividualSellPrice, includeIndividualPrice, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this CompletedInvestment _collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetSellPriceStringFromInvestment(_collapsedInvestment.TotalSellPrice, _collapsedInvestment.IndividualSellPrice, includeIndividualPrice, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this PendingInvestment _collapsedPendingInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetSellPriceStringFromInvestment(_collapsedPendingInvestment.TotalBuyPrice, _collapsedPendingInvestment.Data.CurrentIndividualSellPrice, includeIndividualPrice, alignment);
    }

    private static string GetSellPriceStringFromInvestment(int totalPrice, int individualPrice, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return $"{$"[{alignment}]".ToLower()}{totalPrice.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]each[/color] {individualPrice.ToCurrencyString(true)}" : "")}";
    }
}
