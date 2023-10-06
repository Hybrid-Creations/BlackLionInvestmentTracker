using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class PotentialInvestmentItem : InvestmentItem
{
    public virtual void Init(ItemData item, bool isInDeliveryBox, PotentialInvestment investment)
    {
        if (IsQueuedForDeletion())
            return;

        itemProperties.GetNode<ItemIcon>("Icon").Init(item.Icon, investment.BuyData.Quantity, isInDeliveryBox);
        itemProperties.GetNode<Label>("Name").Text = item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = investment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = investment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenPrice").Text = GetNeededPriceForAnyProfit(investment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = investment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = investment.BuyData.DatePurchased.ToTimeSinceString();
    }

    private string GetNeededPriceForAnyProfit(PotentialInvestment investment)
    {
        // If you are making profit, it looks good
        if (investment.TotalPotentialProfit > 0)
            return Constants.EmptyItemPropertyEntry;
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt((investment.BuyData.IndividualBuyPrice + 1) * Constants.MultiplyInverseTax);
            return DataExtensions.CombineTotalAndIndividual(investment.BuyData.Quantity * idealPrice, idealPrice).ColorRichString("#ffd500").AlignRichString(RichStringAlignment.RIGHT);
        }
    }
}
