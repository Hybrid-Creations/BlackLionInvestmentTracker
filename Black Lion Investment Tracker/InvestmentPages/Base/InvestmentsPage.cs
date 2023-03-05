using Godot;

namespace BLIT.UI;

public partial class InvestmentsPage : VBoxContainer
{
    [Export]
    protected PackedScene collapsedInvestmentScene;
    [Export]
    protected VBoxContainer investmentHolder;
    [Export]
    protected Label loadingLabel;
}
