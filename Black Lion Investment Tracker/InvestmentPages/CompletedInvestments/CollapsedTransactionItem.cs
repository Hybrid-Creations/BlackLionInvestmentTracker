using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
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

    CollapsedCompletedInvestment collapsedInvestment;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public void Init(ItemData _item, CollapsedCompletedInvestment _collapsedInvestment)
    {
        collapsedInvestment = _collapsedInvestment;

        itemProperties.GetNode<TextureRect>("Icon").Texture = _item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = _collapsedInvestment.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = _item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = _collapsedInvestment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = _collapsedInvestment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = $"[right]{_collapsedInvestment.TotalProfit.ToCurrencyString(true)}[/right]";
        itemProperties.GetNode<Label>("InvestDate").Text = _collapsedInvestment.OldestPurchaseDate.ToTimeSinceString();
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;

            foreach (var investment in collapsedInvestment.SubInvestments.OrderBy(si => si.Data.BuyData.DatePurchased))
            {
                var instance = investmentItemScene.Instantiate<TransactionItem>();
                instance.Init(Cache.Items.GetItemData(investment.Data.BuyData.ItemId), investment);
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
                        Main.Database.CollapsedCompletedInvestments.Remove(collapsedInvestment);

                        foreach (var investment in collapsedInvestment.SubInvestments)
                        {
                            Main.Database.CompletedInvestments.Remove(investment);
                            Main.Database.NotInvestments.Add(investment.Data.BuyData.TransactionId);
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
