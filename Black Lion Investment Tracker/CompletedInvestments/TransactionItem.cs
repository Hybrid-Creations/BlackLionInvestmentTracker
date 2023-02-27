using BLIT.Extensions;
using Godot;

namespace BLIT;
public partial class TransactionItem : VBoxContainer
{
    public void Init(ItemData item, InvestmentData investment)
    {
        GetNode<TextureRect>("ItemProperties/Icon").Texture = item.Icon;
        GetNode<Label>("ItemProperties/Icon/Quantity").Text = investment.Quantity.ToString();
        GetNode<Label>("ItemProperties/Name").Text = $" {item.Name}";
        GetNode<RichTextLabel>("ItemProperties/BuyPrice").Text = $"[right]{investment.TotalBuyPrice.ToCurrencyString(true)}[/right]";
        GetNode<RichTextLabel>("ItemProperties/SellPrice").Text = $"[right]{investment.TotalSellPrice.ToCurrencyString(true)}[/right]";
        GetNode<RichTextLabel>("ItemProperties/Profit").Text = $"[right]{investment.Profit.ToCurrencyString(true)}[/right]";
        GetNode<Label>("ItemProperties/InvestDate").Text = investment.PurchaseDate.ToTimeSinceString();
        GetNode<Label>("ItemProperties/SellDate").Text = investment.LatestSellDate.ToTimeSinceString();
    }
}
