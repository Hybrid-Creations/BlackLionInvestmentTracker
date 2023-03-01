using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestmentItem : InvestmentItem<PendingInvestment, PendingInvestmentData, BuyData, PendingSellData>
{
    public override void Init(ItemData item, PendingInvestment pendingInvestment)
    {
        base.Init(item, pendingInvestment);

        itemProperties.GetNode<RichTextLabel>("CurrentSellPrice").Text = pendingInvestment.GetSellPriceStringFromInvestment<PendingInvestment, PendingInvestmentData, BuyData, PendingSellData>(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(pendingInvestment);
        itemProperties.GetNode<RichTextLabel>("CurrentProfit").Text = $"[right]{pendingInvestment.PotentialProfit.ToCurrencyString(true)}[/right]";
    }

    private static string GetNeededPriceForAnyProfit(PendingInvestment pendingInvestment)
    {
        // If you are making profit, it looks good
        if (pendingInvestment.PotentialProfit > 0)
            return "[right]-----[/right]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(pendingInvestment.IndividualSellPrice * Constants.MultiplyInverseTax);
            return $"[right]{(pendingInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }
}
