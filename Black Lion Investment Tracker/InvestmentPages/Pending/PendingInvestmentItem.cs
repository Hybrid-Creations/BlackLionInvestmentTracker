using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestmentItem : InvestmentItem
{
    int currentSellPrice;

    public virtual void Init(ItemData item, PendingInvestment investment, int currentSellPrice)
    {
        if (IsQueuedForDeletion())
            return;

        this.currentSellPrice = currentSellPrice;

        itemProperties.GetNode<ItemIcon>("Icon").Init(item.Icon, investment.BuyData.Quantity, false);
        itemProperties.GetNode<Label>("Name").Text = item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = investment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = investment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenPrice").Text = GetCurrentListedPrice(investment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = investment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = investment.BuyData.DatePurchased.ToTimeSinceString();
    }

    private string GetCurrentListedPrice(PendingInvestment investment)
    {
        var listedIsHigher = currentSellPrice < investment.LowestIndividualSellPrice;
        return $"{(listedIsHigher ? $"\n[right][color=gray]ea[/color] [color=#ff9200]{currentSellPrice.ToCurrencyString(RichImageType.PX32)}" : Constants.EmptyItemPropertyEntry)}";
    }
}
