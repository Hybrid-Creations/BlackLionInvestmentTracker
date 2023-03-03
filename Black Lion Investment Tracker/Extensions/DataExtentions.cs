using BLIT.ConstantVariables;
using BLIT.Investments;

namespace BLIT.Extensions;

public static class DataExtentions
{
    // // Buy
    public static string GetBuyPriceStringFromInvestment(this CollapsedCompletedInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceString(collapsedInvestment.TotalBuyPrice, collapsedInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this CompletedInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceString(investment.BuyData.TotalBuyPrice, investment.BuyData.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this CollapsedPendingInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceString(collapsedInvestment.TotalBuyPrice, collapsedInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this PendingInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceString(investment.BuyData.TotalBuyPrice, investment.BuyData.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this CollapsedPotentialInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceString(collapsedInvestment.TotalBuyPrice, collapsedInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this PotentialInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceString(investment.BuyData.TotalBuyPrice, investment.BuyData.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    private static string GetBuyPriceString(int totalBuyPrice, int individualBuyPrice, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return $"{$"[{alignment}]".ToLower()}{totalBuyPrice.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]each[/color] {individualBuyPrice.ToCurrencyString(true)}" : "")}";
    }


    // //Sell
    public static string GetSellPriceStringFromInvestment(this CollapsedCompletedInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        var individualPrefix = collapsedInvestment.AllSellDatasAreTheSame ? "each" : "avg";
        var sellPrice = collapsedInvestment.AllSellDatasAreTheSame ? collapsedInvestment.IndividualSellPrice : collapsedInvestment.AverageIndividualSellPrice;
        return GetSellPriceString(collapsedInvestment.TotalSellPrice, sellPrice, includeIndividualPrice, individualPrefix, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this CompletedInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        var individualPrefix = investment.AllSellDatasAreTheSame ? "each" : "avg";
        var sellPrice = investment.AllSellDatasAreTheSame ? investment.IndividualSellPrice : investment.AverageIndividualSellPrice;
        return GetSellPriceString(investment.TotalSellPrice, sellPrice, includeIndividualPrice, individualPrefix, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this CollapsedPendingInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        var individualPrefix = collapsedInvestment.AllSellDatasAreTheSame ? "each" : "avg";
        var sellPrice = collapsedInvestment.AllSellDatasAreTheSame ? collapsedInvestment.IndividualSellPrice : collapsedInvestment.AverageIndividualSellPrice;
        return GetSellPriceString(collapsedInvestment.TotalSellPrice, sellPrice, includeIndividualPrice, individualPrefix, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this PendingInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        var individualPrefix = investment.AllSellDatasAreTheSame ? "each" : "avg";
        var sellPrice = investment.AllSellDatasAreTheSame ? investment.IndividualSellPrice : investment.AverageIndividualSellPrice;
        return GetSellPriceString(investment.TotalSellPrice, sellPrice, includeIndividualPrice, individualPrefix, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this CollapsedPotentialInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetSellPriceString(collapsedInvestment.TotalPotentialSellPrice, collapsedInvestment.IndividualPotentialSellPrice, includeIndividualPrice, "each", alignment);
    }

    public static string GetSellPriceStringFromInvestment(this PotentialInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetSellPriceString(investment.TotalPotentialSellPrice, investment.IndividualPotentialSellPrice, includeIndividualPrice, "each", alignment);
    }

    private static string GetSellPriceString(int totalSellPrice, double individualSellPrice, bool includeIndividualPrice, string individualPrefix, RichStringAlignment alignment)
    {
        return $"{$"[{alignment}]".ToLower()}{totalSellPrice.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]{individualPrefix}[/color] {individualSellPrice.ToCurrencyString(true)}" : "")}";
    }


    // //Profit
    public static string GetProfitStringFromInvestment(this CollapsedCompletedInvestment collapsedInvestment, bool includeIndividualProfit, RichStringAlignment alignment)
    {
        var individualPrefix = collapsedInvestment.AllSellDatasAreTheSame ? "each" : "avg";
        var profit = collapsedInvestment.AllSellDatasAreTheSame ? collapsedInvestment.IndividualProfit : collapsedInvestment.AverageIndividualProfit;
        return GetProfitString(collapsedInvestment.TotalProfit, profit, includeIndividualProfit, individualPrefix, alignment);
    }

    public static string GetProfitStringFromInvestment(this CompletedInvestment investment, bool includeIndividualProfit, RichStringAlignment alignment)
    {
        var individualPrefix = investment.AllSellDatasAreTheSame ? "each" : "avg";
        var profit = investment.AllSellDatasAreTheSame ? investment.IndividualProfit : investment.AverageIndividualProfit;
        return GetProfitString(investment.TotalProfit, profit, includeIndividualProfit, individualPrefix, alignment);
    }

    public static string GetProfitStringFromInvestment(this CollapsedPendingInvestment collapsedInvestment, bool includeIndividualProfit, RichStringAlignment alignment)
    {
        var individualPrefix = collapsedInvestment.AllSellDatasAreTheSame ? "each" : "avg";
        var profit = collapsedInvestment.AllSellDatasAreTheSame ? collapsedInvestment.IndividualProfit : collapsedInvestment.AverageIndividualProfit;
        return GetProfitString(collapsedInvestment.TotalProfit, profit, includeIndividualProfit, individualPrefix, alignment);
    }

    public static string GetProfitStringFromInvestment(this PendingInvestment investment, bool includeIndividualProfit, RichStringAlignment alignment)
    {
        var individualPrefix = investment.AllSellDatasAreTheSame ? "each" : "avg";
        var profit = investment.AllSellDatasAreTheSame ? investment.IndividualProfit : investment.AverageIndividualProfit;
        return GetProfitString(investment.TotalProfit, profit, includeIndividualProfit, individualPrefix, alignment);
    }

    public static string GetProfitStringFromInvestment(this CollapsedPotentialInvestment collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetProfitString(collapsedInvestment.TotalPotentialProfit, collapsedInvestment.IndividualPotentialProfit, includeIndividualPrice, "each", alignment);
    }

    public static string GetProfitStringFromInvestment(this PotentialInvestment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetProfitString(investment.TotalPotentialProfit, investment.IndividualPotentialProfit, includeIndividualPrice, "each", alignment);
    }

    private static string GetProfitString(double totalProfit, double individualProfit, bool includeIndividualPrice, string individualPrefix, RichStringAlignment alignment)
    {
        return $"{$"[{alignment}]".ToLower()}{totalProfit.ToCurrencyString(true)}{(includeIndividualPrice ? $"\n[color=gray]{individualPrefix}[/color] {individualProfit.ToCurrencyString(true)}" : "")}";
    }
}
