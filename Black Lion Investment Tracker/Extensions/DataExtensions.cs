using BLIT.ConstantVariables;
using BLIT.Investments;

namespace BLIT.Extensions;

public static class DataExtensions
{
    // // Buy
    public static string GetBuyPriceStringFromInvestment<T>(this CollapsedInvestment<T> collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
        where T : Investment
    {
        return GetBuyPriceString(collapsedInvestment.TotalBuyPrice, collapsedInvestment.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    public static string GetBuyPriceStringFromInvestment(this Investment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return GetBuyPriceString(investment.BuyData.TotalBuyPrice, investment.BuyData.IndividualBuyPrice, includeIndividualPrice, alignment);
    }

    private static string GetBuyPriceString(int totalBuyPrice, int individualBuyPrice, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        return $"{(includeIndividualPrice ? CombineTotalAndIndividual(totalBuyPrice, individualBuyPrice) : $"{totalBuyPrice.ToCurrencyString(RichImageType.PX32)}")}".AlignRichString(
            alignment
        );
    }

    // //Sell

    public static string GetSellPriceStringFromInvestment<T>(this CollapsedInvestment<T> collapsedInvestment, bool includeIndividualPrice, RichStringAlignment alignment)
        where T : Investment
    {
        var individualPrefix = collapsedInvestment.IndividualSellPriceDifferent ? "avg" : "ea";
        var sellPrice = collapsedInvestment.IndividualSellPriceDifferent ? collapsedInvestment.IndividualSellPrice : collapsedInvestment.AverageIndividualSellPrice;
        return GetSellPriceString(collapsedInvestment.TotalSellPrice, sellPrice, includeIndividualPrice, individualPrefix, alignment);
    }

    public static string GetSellPriceStringFromInvestment(this Investment investment, bool includeIndividualPrice, RichStringAlignment alignment)
    {
        var individualPrefix = investment.IndividualSellPriceDifferent ? "avg" : "ea";
        var sellPrice = investment.IndividualSellPriceDifferent ? investment.IndividualSellPrice : investment.AverageIndividualSellPrice;
        return GetSellPriceString(investment.TotalSellPrice, sellPrice, includeIndividualPrice, individualPrefix, alignment);
    }

    private static string GetSellPriceString(int totalSellPrice, double individualSellPrice, bool includeIndividualPrice, string individualPrefix, RichStringAlignment alignment)
    {
        return $"{(includeIndividualPrice ? CombineTotalAndIndividual(totalSellPrice, individualSellPrice, individualPrefix) : $"{totalSellPrice.ToCurrencyString(RichImageType.PX32)}")}".AlignRichString(
            alignment
        );
    }

    // //Profit

    public static string GetProfitStringFromInvestment<T>(this CollapsedInvestment<T> collapsedInvestment, bool includeIndividualProfit, RichStringAlignment alignment)
        where T : Investment
    {
        var individualPrefix = collapsedInvestment.Quantity > 1 ? "avg" : "ea";
        var profit = collapsedInvestment.AverageIndividualProfit;
        return GetProfitString(collapsedInvestment.TotalProfit, profit, includeIndividualProfit, individualPrefix, alignment);
    }

    public static string GetProfitStringFromInvestment(this Investment investment, bool includeIndividualProfit, RichStringAlignment alignment)
    {
        var individualPrefix = investment.BuyData.Quantity > 1 ? "avg" : "ea";
        return GetProfitString(investment.TotalNetProfit, investment.AverageIndividualProfit, includeIndividualProfit, individualPrefix, alignment);
    }

    private static string GetProfitString(double totalProfit, double individualProfit, bool includeIndividualPrice, string individualPrefix, RichStringAlignment alignment)
    {
        return $"{(includeIndividualPrice ? CombineTotalAndIndividual(totalProfit, individualProfit, individualPrefix) : $"{totalProfit.ToCurrencyString(RichImageType.PX32)}")}".AlignRichString(
            alignment
        );
    }

    public static string CombineTotalAndIndividual(double total, double individual, string individualPrefix = "ea")
    {
        return $"{total.ToCurrencyString(RichImageType.PX32)} \n {$"[color=gray]{individualPrefix}[/color] {individual.ToCurrencyString(RichImageType.PX32)}"}";
    }
}
