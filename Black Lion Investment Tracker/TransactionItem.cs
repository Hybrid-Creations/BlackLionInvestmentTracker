using BLIT.Extensions;
using Godot;

namespace BLIT;
public partial class TransactionItem : VBoxContainer
{
    public void Init(string itemName, Texture2D icon, InvestmentData investment)
    {
        GetNode<TextureRect>("ItemProperties/Icon").Texture = icon;
        GetNode<Label>("ItemProperties/Icon/Quantity").Text = investment.Quantity.ToString();
        GetNode<Label>("ItemProperties/Name").Text = itemName;
        GetNode<RichTextLabel>("ItemProperties/BuyPrice").Text = $"[right]{investment.TotalBuyPrice.ToCurrencyString(true)}[/right]";
        GetNode<RichTextLabel>("ItemProperties/SellPrice").Text = $"[right]{investment.TotalSellPrice.ToCurrencyString(true)}[/right]";
        GetNode<RichTextLabel>("ItemProperties/Profit").Text = $"[right]{investment.Profit.ToCurrencyString(true)}[/right]";
        GetNode<Label>("ItemProperties/InvestDate").Text = investment.PurchaseDate.ToTimeSinceString();
        GetNode<Label>("ItemProperties/SellDate").Text = investment.LatestSellDate.ToTimeSinceString();
    }
}

public class ItemData
{
    public string Name;
    public int Profit;

    public ItemData(string name, int profit)
    {
        Name = name;
        Profit = profit;
    }
}
