using System;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class InvestmentItem : VBoxContainer
{
    [ExportCategory("UI References")]
    [Export]
    protected HBoxContainer itemProperties;

    // public virtual void Init(ItemData item, Investment investment)
    // {
    //     TextureRect texRectIcon = itemProperties.GetNode<TextureRect>("Icon");
    //     Label lblQuantity = itemProperties.GetNode<Label>("Icon/Quantity");
    //     Label lblName = itemProperties.GetNode<Label>("Name");
    //     RichTextLabel richLblBuyPrice = itemProperties.GetNode<RichTextLabel>("BuyPrice");
    //     RichTextLabel richLblSellPrice = itemProperties.GetNode<RichTextLabel>("SellPrice");
    //     RichTextLabel richLblProfit = itemProperties.GetNode<RichTextLabel>("Profit");
    //     Label lblBuyDate = itemProperties.GetNode<Label>("BuyDate");
    //     Label lblSellDate = itemProperties.GetNode<Label>("SellDate");

    //     if (texRectIcon is not null)
    //         texRectIcon.Texture = item.Icon;

    //     if (lblQuantity is not null)
    //         lblQuantity.Text = investment.BuyData.Quantity.ToString();

    //     if (lblName is not null)
    //         lblName.Text = item.Name;

    //     if (richLblBuyPrice is not null)
    //         richLblBuyPrice.Text = $"{investment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT)}";

    //     if (richLblSellPrice is not null)
    //         richLblSellPrice.Text = $"{investment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT)}";

    //     if (richLblProfit is not null)
    //         richLblProfit.Text = $"{investment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT)}";

    //     if (lblBuyDate is not null)
    //         lblBuyDate.Text = investment.BuyData.DatePurchased.ToTimeSinceString();

    //     if (lblSellDate is not null)
    //         lblSellDate.Text = investment.LatestSellDate.ToTimeSinceString();
    // }
}
