using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public sealed partial class CollapsedPendingInvestmentItem : CollapsedInvestmentItem<CollapsedPendingInvestment, PendingInvestment, PendingInvestmentData, BuyData, PendingSellData, PendingInvestmentItem>
{
    public override void Init(ItemData _item, CollapsedPendingInvestment _collapsedPendingInvestment)
    {
        base.Init(_item, _collapsedPendingInvestment);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = _collapsedPendingInvestment.GetSellPriceStringFromInvestment<CollapsedPendingInvestment, PendingInvestment, PendingInvestmentData, BuyData, PendingSellData>(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(_collapsedPendingInvestment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = $"[right]{_collapsedPendingInvestment.TotalPotentialProfit.ToCurrencyString(true)}[/right]";
    }

    private static string GetNeededPriceForAnyProfit(CollapsedPendingInvestment _collapsedPendingInvestment)
    {
        // If you are making profit, it looks good
        if (_collapsedPendingInvestment.TotalPotentialProfit > 0)
            return "[right]-----[/right]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(_collapsedPendingInvestment.IndividualSellPrice * Constants.MultiplyInverseTax);
            return $"[right]{(_collapsedPendingInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }

    protected override void OnMarkedNotAnInvestment()
    {
        Main.Database.CollapsedPendingInvestments.Remove(collapsedInvestment);

        foreach (var investment in collapsedInvestment.SubInvestments)
        {
            Main.Database.PendingInvestments.Remove(investment);
            Main.Database.NotInvestments.Add(investment.Data.BuyData.TransactionId);
        }
    }
}
