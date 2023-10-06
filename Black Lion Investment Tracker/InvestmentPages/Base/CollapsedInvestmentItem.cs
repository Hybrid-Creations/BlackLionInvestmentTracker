using System;
using Godot;

namespace BLIT.UI;

public partial class CollapsedInvestmentItem : VBoxContainer
{
    [ExportCategory("UI References")]
    [Export]
    protected HBoxContainer itemProperties;

    [ExportGroup("Toggle Button")]
    [Export]
    protected Button toggleTreeButton;

    [Export]
    protected Control toggleSpacer;

    [Export]
    protected Control TitleBorder3;

    [Export]
    protected Texture2D arrowRight;

    [Export]
    protected Texture2D arrowDown;

    [ExportCategory("Sub Investments")]
    [Export]
    protected PackedScene subInvestmentItemScene;

    [Export]
    protected VBoxContainer subInvestmentsHolder;

    [Export]
    protected HBoxContainer subInvestmentTitles;

    public string ItemName { get; protected set; }
    public virtual double TotalBuyPrice => throw new System.NotImplementedException();
    public virtual double TotalSellPrice => throw new System.NotImplementedException();
    public virtual double TotalProfit => throw new System.NotImplementedException();
    public virtual DateTimeOffset LastActive => throw new System.NotImplementedException();

    protected void HideTreeToggle()
    {
        toggleTreeButton.Hide();
        toggleSpacer.Show();
    }
}
