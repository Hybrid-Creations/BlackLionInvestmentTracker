using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class PotentialInvestmentItem : InvestmentItem<PotentialInvestment, PotentialInvestmentData, BuyData, PotentialSellData>
{
    public override void Init(ItemData item, PotentialInvestment potentialInvestment)
    {
        itemProperties.GetNode<RichTextLabel>("CurrentSellPrice").Text = potentialInvestment.GetSellPriceStringFromInvestment<PotentialInvestment, PotentialInvestmentData, BuyData, PotentialSellData>(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(potentialInvestment);
        itemProperties.GetNode<RichTextLabel>("CurrentProfit").Text = $"[right]{potentialInvestment.PotentialProfit.ToCurrencyString(true)}[/right]";
    }

    private static string GetNeededPriceForAnyProfit(PotentialInvestment potentialInvestment)
    {
        // If you are making profit, it looks good
        if (potentialInvestment.PotentialProfit > 0)
            return "[right]-----[/right]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(potentialInvestment.IndividualSellPrice * Constants.MultiplyInverseTax);
            return $"[right]{(potentialInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }
}
