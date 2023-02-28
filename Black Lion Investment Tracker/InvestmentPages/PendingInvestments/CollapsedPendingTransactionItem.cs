using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class CollapsedPendingTransactionItem : VBoxContainer
{
    [Export]
    HBoxContainer itemProperties;

    [ExportCategory("Sub Investments")]
    [Export]
    PackedScene pendingInvestmentItemScene;
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

    CollapsedPendingInvestment collapsedPendingInvestment;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public void Init(ItemData _item, CollapsedPendingInvestment _collapsedPendingInvestment)
    {
        collapsedPendingInvestment = _collapsedPendingInvestment;

        itemProperties.GetNode<TextureRect>("Icon").Texture = _item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = _collapsedPendingInvestment.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = _item.Name;
        itemProperties.GetNode<RichTextLabel>("InvestmentPrice").Text = _collapsedPendingInvestment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("CurrentSellPrice").Text = _collapsedPendingInvestment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(_collapsedPendingInvestment);
        itemProperties.GetNode<RichTextLabel>("CurrentProfit").Text = $"[right]{_collapsedPendingInvestment.TotalPotentialProfit.ToCurrencyString(true)}[/right]";
        itemProperties.GetNode<Label>("InvestDate").Text = _collapsedPendingInvestment.OldestPurchaseDate.ToTimeSinceString();
    }

    private static string GetNeededPriceForAnyProfit(CollapsedPendingInvestment _collapsedPendingInvestment)
    {
        // If you are making profit, it looks good
        if (_collapsedPendingInvestment.TotalPotentialProfit > 0)
            return "[right]-----[/right]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(_collapsedPendingInvestment.IndividualSellPrice * Constants.MultiplyInverseTax);
            return $"[right]{(_collapsedPendingInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;

            foreach (var investment in collapsedPendingInvestment.SubInvestments.OrderBy(si => si.LatestSellDate))
            {
                var instance = pendingInvestmentItemScene.Instantiate<PendingInvestmentItem>();
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
                        Main.Database.CollapsedPendingInvestments.Remove(collapsedPendingInvestment);

                        foreach (var investment in collapsedPendingInvestment.SubInvestments)
                        {
                            Main.Database.PendingInvestments.Remove(investment);
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
