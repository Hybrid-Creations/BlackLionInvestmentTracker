using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public abstract partial class InvestmentItem<TInvestment, TInvestmentData, TBuyData, TSellData> : VBoxContainer
where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
where TInvestmentData : InvestmentData<TBuyData, TSellData>
where TBuyData : BuyData
where TSellData : SellData
{
    [ExportCategory("UI References")]
    [Export]
    protected HBoxContainer itemProperties;

    public virtual void Init(ItemData item, TInvestment investment)
    {
        TextureRect texRectIcon = itemProperties.GetNode<TextureRect>("Icon");
        Label lblQuantity = itemProperties.GetNode<Label>("Icon/Quantity");
        Label lblName = itemProperties.GetNode<Label>("Name");
        RichTextLabel richLblBuyPrice = itemProperties.GetNode<RichTextLabel>("BuyPrice");
        RichTextLabel richLblSellPrice = itemProperties.GetNode<RichTextLabel>("SellPrice");
        Label lblBuyDate = itemProperties.GetNode<Label>("BuyDate");
        Label lblSellDate = itemProperties.GetNode<Label>("SellDate");

        if (texRectIcon is not null)
            texRectIcon.Texture = item.Icon;

        if (lblQuantity is not null)
            lblQuantity.Text = investment.Quantity.ToString();

        if (lblName is not null)
            lblName.Text = item.Name;

        if (richLblBuyPrice is not null)
            richLblBuyPrice.Text = $"{investment.GetBuyPriceStringFromInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>(true, RichStringAlignment.RIGHT)}";

        if (richLblSellPrice is not null)
            richLblSellPrice.Text = $"{investment.GetSellPriceStringFromInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>(true, RichStringAlignment.RIGHT)}";

        if (lblBuyDate is not null)
            lblBuyDate.Text = investment.Data.BuyData.DatePurchased.ToString();

        if (lblSellDate is not null)
            lblSellDate.Text = investment.LatestSellDate.ToString();
    }
}
