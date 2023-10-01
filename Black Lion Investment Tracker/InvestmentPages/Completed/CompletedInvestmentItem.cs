using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class CompletedInvestmentItem : InvestmentItem
{
    public virtual void Init(ItemData item, bool isInDeliveryBox, CompletedInvestment investment)
    {
        if (IsQueuedForDeletion())
            return;

        itemProperties.GetNode<ItemIcon>("Icon").Init(item.Icon, investment.BuyData.Quantity, isInDeliveryBox);
        itemProperties.GetNode<Label>("Name").Text = item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = investment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = investment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = investment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = investment.BuyData.DatePurchased.ToTimeSinceString();
        itemProperties.GetNode<Label>("SellDate").Text = investment.LatestSellDate.ToTimeSinceString();
    }
}
