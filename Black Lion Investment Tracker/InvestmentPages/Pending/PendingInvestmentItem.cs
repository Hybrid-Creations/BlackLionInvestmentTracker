using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestmentItem : InvestmentItem
{
    public virtual void Init(ItemData item, PendingInvestment investment)
    {
        itemProperties.GetNode<TextureRect>("Icon").Texture = item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = investment.BuyData.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = investment.GetBuyPriceStringFromInvestment(RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = investment.GetSellPriceStringFromInvestment(RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenPrice").Text = GetCurrentListedPrice(investment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = investment.GetProfitStringFromInvestment(RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = investment.BuyData.DatePurchased.ToTimeSinceString();
    }

    private string GetCurrentListedPrice(PendingInvestment investment)
    {
        var listedIsHigher = investment.CurrentSellPrice < investment.LowestIndividualSellPrice;
        return $"{(listedIsHigher ? $"[right][color=#FF5A00]{investment.CurrentSellPrice.ToCurrencyString(RichImageType.PX32)}" : Constants.EmptyItemPropertyEntry)}";
    }
}