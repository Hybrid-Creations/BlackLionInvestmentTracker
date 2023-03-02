using System;
using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
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
}
