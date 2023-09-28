using System.Linq;
using BLIT.Extensions;
using Godot;
using Godot.Collections;

namespace BLIT.UI;

public partial class InvestmentsPage : VBoxContainer
{
    [Export]
    protected PackedScene collapsedInvestmentScene;

    [Export]
    protected VBoxContainer investmentHolder;

    [Export]
    protected Label loadingLabel;

    [Export]
    Label nameLabel;

    [Export]
    TextureRect nameSortingArrow;

    [Export]
    Label buyPriceLabel;

    [Export]
    TextureRect buyPriceSortingArrow;

    [Export]
    Label sellPriceLabel;

    [Export]
    TextureRect sellPriceSortingArrow;

    [Export]
    Label totalProfitLabel;

    [Export]
    TextureRect totalProfitSortingArrow;

    [Export]
    Label lastActiveLabel;

    [Export]
    protected TextureRect lastActiveSortingArrow;

    protected SortingMode Sorting = SortingMode.LastActive;
    protected SortingDirection Direction = SortingDirection.Descending;

    public override void _Ready()
    {
        nameLabel.GuiInput += (arg) =>
        {
            if (arg is InputEventMouseButton mouse && mouse.IsPressed() && mouse.ButtonMask == MouseButtonMask.Left)
                SortByName();
        };
        buyPriceLabel.GuiInput += (arg) =>
        {
            if (arg is InputEventMouseButton mouse && mouse.IsPressed() && mouse.ButtonMask == MouseButtonMask.Left)
                SortByBuyPrice();
        };
        sellPriceLabel.GuiInput += (arg) =>
        {
            if (arg is InputEventMouseButton mouse && mouse.IsPressed() && mouse.ButtonMask == MouseButtonMask.Left)
                SortBySellPrice();
        };
        totalProfitLabel.GuiInput += (arg) =>
        {
            if (arg is InputEventMouseButton mouse && mouse.IsPressed() && mouse.ButtonMask == MouseButtonMask.Left)
                SortByTotalProfit();
        };
        lastActiveLabel.GuiInput += (arg) =>
        {
            if (arg is InputEventMouseButton mouse && mouse.IsPressed() && mouse.ButtonMask == MouseButtonMask.Left)
                SortByLastActive();
        };

        ClearList();
        ResetAllSortingArrows();
    }

    protected void ClearList()
    {
        // Remove Old Investment Items From UI
        investmentHolder.ClearChildren();
        loadingLabel.Show();
    }

    protected static void ProbablyRealException(System.Exception e)
    {
        GD.PushError(e);
        GD.PushWarning("Unexpected error from GW2Sharp, might be an API issue?");
        APIStatusIndicator.ShowStatus("Possible Issues With API, Some Requests Are Failing.");
    }

    protected Array<Node> RemoveChildren()
    {
        var children = investmentHolder.GetChildren();
        foreach (var child in children)
            investmentHolder.RemoveChild(child);

        return children;
    }

    protected void ResetAllSortingArrows()
    {
        nameSortingArrow.Hide();
        buyPriceSortingArrow.Hide();
        sellPriceSortingArrow.Hide();
        totalProfitSortingArrow.Hide();
        lastActiveSortingArrow.Hide();
    }

    void SortByName()
    {
        if (Sorting == SortingMode.Name)
            Direction = Direction == SortingDirection.Ascending ? SortingDirection.Descending : SortingDirection.Ascending;
        else
        {
            Sorting = SortingMode.Name;
            Direction = SortingDirection.Descending;
        }

        ResetAllSortingArrows();
        nameSortingArrow.Show();
        nameSortingArrow.FlipV = Direction == SortingDirection.Ascending;

        // Remove all the children so we can add them back later
        var children = RemoveChildren();

        // Add all the children back in the new order
        IOrderedEnumerable<Node> sortedChildren;
        if (Direction == SortingDirection.Descending)
            sortedChildren = children.OrderByDescending(ci => ((CollapsedInvestmentItem)ci).ItemName);
        else
            sortedChildren = children.OrderBy(ci => ((CollapsedInvestmentItem)ci).ItemName);

        foreach (var child in sortedChildren)
            investmentHolder.AddChild(child);
    }

    void SortByBuyPrice()
    {
        if (Sorting == SortingMode.BuyPrice)
            Direction = Direction == SortingDirection.Ascending ? SortingDirection.Descending : SortingDirection.Ascending;
        else
        {
            Sorting = SortingMode.BuyPrice;
            Direction = SortingDirection.Descending;
        }

        ResetAllSortingArrows();
        buyPriceSortingArrow.Show();
        buyPriceSortingArrow.FlipV = Direction == SortingDirection.Ascending;

        // Remove all the children so we can add them back later
        var children = RemoveChildren();

        // Add all the children back in the new order
        IOrderedEnumerable<Node> sortedChildren;
        if (Direction == SortingDirection.Descending)
            sortedChildren = children.OrderByDescending(ci => ((CollapsedInvestmentItem)ci).TotalBuyPrice);
        else
            sortedChildren = children.OrderBy(ci => ((CollapsedInvestmentItem)ci).TotalBuyPrice);

        foreach (var child in sortedChildren)
            investmentHolder.AddChild(child);
    }

    void SortBySellPrice()
    {
        if (Sorting == SortingMode.SellPrice)
            Direction = Direction == SortingDirection.Ascending ? SortingDirection.Descending : SortingDirection.Ascending;
        else
        {
            Sorting = SortingMode.SellPrice;
            Direction = SortingDirection.Descending;
        }

        ResetAllSortingArrows();
        sellPriceSortingArrow.Show();
        sellPriceSortingArrow.FlipV = Direction == SortingDirection.Ascending;

        // Remove all the children so we can add them back later
        var children = RemoveChildren();

        // Add all the children back in the new order
        IOrderedEnumerable<Node> sortedChildren;
        if (Direction == SortingDirection.Descending)
            sortedChildren = children.OrderByDescending(ci => ((CollapsedInvestmentItem)ci).TotalSellPrice);
        else
            sortedChildren = children.OrderBy(ci => ((CollapsedInvestmentItem)ci).TotalSellPrice);

        foreach (var child in sortedChildren)
            investmentHolder.AddChild(child);
    }

    void SortByTotalProfit()
    {
        if (Sorting == SortingMode.TotalProfit)
            Direction = Direction == SortingDirection.Ascending ? SortingDirection.Descending : SortingDirection.Ascending;
        else
        {
            Sorting = SortingMode.TotalProfit;
            Direction = SortingDirection.Descending;
        }

        ResetAllSortingArrows();
        totalProfitSortingArrow.Show();
        totalProfitSortingArrow.FlipV = Direction == SortingDirection.Ascending;

        // Remove all the children so we can add them back later
        var children = RemoveChildren();

        // Add all the children back in the new order
        IOrderedEnumerable<Node> sortedChildren;
        if (Direction == SortingDirection.Descending)
            sortedChildren = children.OrderByDescending(ci => ((CollapsedInvestmentItem)ci).TotalProfit);
        else
            sortedChildren = children.OrderBy(ci => ((CollapsedInvestmentItem)ci).TotalProfit);

        foreach (var child in sortedChildren)
            investmentHolder.AddChild(child);
    }

    void SortByLastActive()
    {
        if (Sorting == SortingMode.LastActive)
            Direction = Direction == SortingDirection.Ascending ? SortingDirection.Descending : SortingDirection.Ascending;
        else
        {
            Sorting = SortingMode.LastActive;
            Direction = SortingDirection.Descending;
        }

        ResetAllSortingArrows();
        lastActiveSortingArrow.Show();
        lastActiveSortingArrow.FlipV = Direction == SortingDirection.Ascending;

        // Remove all the children so we can add them back later
        var children = RemoveChildren();

        // Add all the children back in the new order
        IOrderedEnumerable<Node> sortedChildren;
        if (Direction == SortingDirection.Descending)
            sortedChildren = children.OrderByDescending(ci => ((CollapsedInvestmentItem)ci).LastActive);
        else
            sortedChildren = children.OrderBy(ci => ((CollapsedInvestmentItem)ci).LastActive);

        foreach (var child in sortedChildren)
            investmentHolder.AddChild(child);
    }

    protected enum SortingMode
    {
        Name,
        BuyPrice,
        SellPrice,
        TotalProfit,
        LastActive
    }

    protected enum SortingDirection
    {
        Ascending,
        Descending
    }
}
