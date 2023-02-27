using System.Linq;
using BLIT.Extensions;
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

    CollapsedPendingInvestmentData collapsedPendingInvestment;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public void Init(ItemData _item, CollapsedPendingInvestmentData _collapsedPendingInvestment)
    {
        collapsedPendingInvestment = _collapsedPendingInvestment;

        itemProperties.GetNode<TextureRect>("Icon").Texture = _item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = _collapsedPendingInvestment.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = $"{_item.Name}   " + _collapsedPendingInvestment.SubInvestments.Sum(s => s.PostedSellDatas.Count).ToString();
        itemProperties.GetNode<RichTextLabel>("InvestmentPrice").Text = GetInvestmentPrice(_collapsedPendingInvestment);
        itemProperties.GetNode<RichTextLabel>("CurrentSellPrice").Text = GetCurrentSellPrice(_collapsedPendingInvestment);
        itemProperties.GetNode<RichTextLabel>("BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(_collapsedPendingInvestment);
        itemProperties.GetNode<RichTextLabel>("CurrentProfit").Text = $"[right]{_collapsedPendingInvestment.TotalPotentialProfit.ToCurrencyString(true)}[/right]";
        itemProperties.GetNode<Label>("InvestDate").Text = _collapsedPendingInvestment.OldestPurchaseDate.ToTimeSinceString();
    }

    private static string GetInvestmentPrice(CollapsedPendingInvestmentData _collapsedPendingInvestment)
    {
        return $"[right]{_collapsedPendingInvestment.TotalBuyPrice.ToCurrencyString(true)}\n [color=gray]each[/color] {_collapsedPendingInvestment.IndividualPrice.ToCurrencyString(true)}[/right]";
    }

    private static string GetCurrentSellPrice(CollapsedPendingInvestmentData _collapsedPendingInvestment)
    {
        return $"[right]{_collapsedPendingInvestment.TotalPotentialSellPrice.ToCurrencyString(true)}\n [color=gray]each[/color] {_collapsedPendingInvestment.CurrentIndividualSellPrice.Value.ToCurrencyString(true)}[/right]";
    }

    private static string GetNeededPriceForAnyProfit(CollapsedPendingInvestmentData _collapsedPendingInvestment)
    {
        // If you are making profit, it looks good
        if (_collapsedPendingInvestment.TotalPotentialProfit > 0)
            return "[right]-----[/right]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(_collapsedPendingInvestment.IndividualPrice * Constants.MultiplyInverseTax);
            return $"[right]{(_collapsedPendingInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;

            foreach (var investment in collapsedPendingInvestment.SubInvestments.OrderBy(si => si.PurchaseDate))
            {
                var instance = pendingInvestmentItemScene.Instantiate<PendingInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.ItemId), investment);
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
