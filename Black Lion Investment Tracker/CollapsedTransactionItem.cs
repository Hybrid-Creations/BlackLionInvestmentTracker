using BLIT;
using BLIT.Extensions;
using Godot;

namespace BLIT;

public partial class CollapsedTransactionItem : VBoxContainer
{
    [Export]
    VBoxContainer subInvestmentHolder;

    public override void _Ready()
    {
        base._Ready();
        GetNode<HBoxContainer>("SubItemTitles").Hide();
    }

    public void Init(string itemName, Texture2D icon, CollapsedInvestmentData investment)
    {
        GetNode<TextureRect>("ItemProperties/Icon").Texture = icon;
        GetNode<Label>("ItemProperties/Icon/Quantity").Text = investment.Quantity.ToString();
        GetNode<Label>("ItemProperties/Name").Text = itemName;
        GetNode<RichTextLabel>("ItemProperties/BuyPrice").Text = $"[right]{investment.TotalBuyPrice.ToCurrencyString(true)}[/right]";
        GetNode<RichTextLabel>("ItemProperties/SellPrice").Text = $"[right]{investment.TotalSellPrice.ToCurrencyString(true)}[/right]";
        GetNode<RichTextLabel>("ItemProperties/Profit").Text = $"[right]{investment.TotalProfit.ToCurrencyString(true)}[/right]";
        GetNode<Label>("ItemProperties/InvestDate").Text = investment.LatestPurchaseDate.ToTimeSinceString();
        GetNode<Label>("ItemProperties/SellDate").Text = investment.LatestSellDate.ToTimeSinceString();
    }
}
