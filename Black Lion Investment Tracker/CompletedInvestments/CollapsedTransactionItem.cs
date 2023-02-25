using System.Linq;
using BLIT.Extensions;
using Godot;

namespace BLIT.UI;

public partial class CollapsedTransactionItem : VBoxContainer
{
    [Export]
    VBoxContainer collapsedItem;

    [ExportCategory("Sub Investments")]
    [Export]
    PackedScene investmentItemScene;
    [Export]
    VBoxContainer subInvestmentHolder;
    [Export]
    HBoxContainer subInvestmentTitles;

    [ExportCategory("Toggle Button")]
    [Export]
    Button toggleTreeButton;
    [Export]
    Texture2D arrowRight;
    [Export]
    Texture2D arrowDown;

    CollapsedInvestmentData collapsedInvestment;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public void Init(string _itemName, Texture2D _icon, CollapsedInvestmentData _collapsedInvestment)
    {
        collapsedInvestment = _collapsedInvestment;

        collapsedItem.GetNode<TextureRect>("ItemProperties/Icon").Texture = _icon;
        collapsedItem.GetNode<Label>("ItemProperties/Icon/Quantity").Text = _collapsedInvestment.Quantity.ToString();
        collapsedItem.GetNode<Label>("ItemProperties/Name").Text = $"{_itemName}";
        collapsedItem.GetNode<RichTextLabel>("ItemProperties/BuyPrice").Text = $"[right]{_collapsedInvestment.TotalBuyPrice.ToCurrencyString(true)}[/right]";
        collapsedItem.GetNode<RichTextLabel>("ItemProperties/SellPrice").Text = $"[right]{_collapsedInvestment.TotalSellPrice.ToCurrencyString(true)}[/right]";
        collapsedItem.GetNode<RichTextLabel>("ItemProperties/Profit").Text = $"[right]{_collapsedInvestment.TotalProfit.ToCurrencyString(true)}[/right]";
        collapsedItem.GetNode<Label>("ItemProperties/InvestDate").Text = _collapsedInvestment.OldestPurchaseDate.ToTimeSinceString();
        collapsedItem.GetNode<Label>("ItemProperties/SellDate").Text = _collapsedInvestment.LatestSellDate.ToTimeSinceString();
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;

            foreach (var investment in collapsedInvestment.SubInvestments.OrderBy(si => si.PurchaseDate))
            {
                var instance = investmentItemScene.Instantiate<TransactionItem>();
                instance.Init(Cache.Items.GetItem(investment.ItemId).Name, Cache.Icons.GetIcon(investment.ItemId), investment);
                subInvestmentHolder.AddChild(instance, 0);
            }
        }
        else
        {
            toggleTreeButton.Icon = arrowRight;
            subInvestmentTitles.Hide();
            subInvestmentHolder.ClearChildren();
        }
    }

    public void GUIInput(InputEvent @event)
    {
        const string MarkNotAnInvestment = "Mark Not An Investment";

        if (@event is InputEventMouseButton mouse)
        {
            if (mouse.ButtonMask == MouseButtonMask.Right)
            {
                RightClickMenu.OpenMenu(GetTree().Root, mouse.GlobalPosition, new[] { MarkNotAnInvestment }, (str) =>
                {
                    if (str == MarkNotAnInvestment)
                    {
                        Main.Database.CollapsedInvestments.Remove(collapsedInvestment);

                        foreach (var investment in collapsedInvestment.SubInvestments)
                        {
                            Main.Database.Investments.Remove(investment);
                            Main.Database.NotInvestments.Add(investment.TransactionId);
                        }

                        Main.Database.Save();
                        QueueFree();
                    }
                });
            }
            else if (mouse.ButtonMask == MouseButtonMask.Left)
            {
                RightClickMenu.CloseInstance();
            }
        }
    }
}
