using System.Linq;
using BLIT.Extensions;
using Godot;

namespace BLIT.UI;

public partial class CollapsedTransactionItem : VBoxContainer
{
    [Export]
    HBoxContainer itemProperties;

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

        itemProperties.GetNode<TextureRect>("Icon").Texture = _icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = _collapsedInvestment.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = $"{_itemName}";
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = $"[right]{_collapsedInvestment.TotalBuyPrice.ToCurrencyString(true)}[/right]";
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = $"[right]{_collapsedInvestment.TotalSellPrice.ToCurrencyString(true)}[/right]";
        itemProperties.GetNode<RichTextLabel>("Profit").Text = $"[right]{_collapsedInvestment.TotalProfit.ToCurrencyString(true)}[/right]";
        itemProperties.GetNode<Label>("InvestDate").Text = _collapsedInvestment.OldestPurchaseDate.ToTimeSinceString();
        itemProperties.GetNode<Label>("SellDate").Text = _collapsedInvestment.LatestSellDate.ToTimeSinceString();
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
                instance.Init(Cache.Items.GetItemData(investment.ItemId).Name, Cache.Icons.GetIcon(investment.ItemId), investment);
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
