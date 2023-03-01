using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public sealed partial class CollapsedPotentialInvestmentItem : CollapsedInvestmentItem<CollapsedPotentialInvestment, PotentialInvestment, PotentialInvestmentData, BuyData, PotentialSellData, PotentialInvestmentItem>
{
    public override void Init(ItemData _item, CollapsedPotentialInvestment _collapsedPotentialInvestment)
    {
        base.Init(_item, _collapsedPotentialInvestment);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = _collapsedPotentialInvestment.GetSellPriceStringFromInvestment<CollapsedPotentialInvestment, PotentialInvestment, PotentialInvestmentData, BuyData, PotentialSellData>(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(_collapsedPotentialInvestment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = $"[right]{_collapsedPotentialInvestment.TotalPotentialProfit.ToCurrencyString(true)}[/right]";
    }

    private static string GetNeededPriceForAnyProfit(CollapsedPotentialInvestment _collapsedPotentialInvestment)
    {
        // If you are making profit, it looks good
        if (_collapsedPotentialInvestment.TotalPotentialProfit > 0)
            return "[right]-----[/right]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(_collapsedPotentialInvestment.IndividualSellPrice * Constants.MultiplyInverseTax);
            return $"[right]{(_collapsedPotentialInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }

    protected override void OnMarkedNotAnInvestment()
    {
        Main.Database.CollapsedPotentialInvestments.Remove(collapsedInvestment);

        foreach (var investment in collapsedInvestment.SubInvestments)
        {
            Main.Database.PotentialInvestments.Remove(investment);
            Main.Database.NotInvestments.Add(investment.Data.BuyData.TransactionId);
        }
    }
}
