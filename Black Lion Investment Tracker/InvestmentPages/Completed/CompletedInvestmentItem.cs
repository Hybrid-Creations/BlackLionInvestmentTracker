using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class CompletedInvestmentItem : InvestmentItem
{
    public virtual void Init(ItemData item, CompletedInvestment investment)
    {
        itemProperties.GetNode<TextureRect>("Icon").Texture = item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = investment.BuyData.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = investment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = investment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = investment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = investment.BuyData.DatePurchased.ToTimeSinceString();
        itemProperties.GetNode<Label>("SellDate").Text = investment.LatestSellDate.ToTimeSinceString();
    }
}
