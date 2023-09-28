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

    protected static void ProbablyRealException(System.Exception e)
    {
        GD.PushError(e);
        GD.PushWarning("Unexpected error from GW2Sharp, might be an API issue?");
        APIStatusIndicator.ShowStatus("Possible Issues With API, Some Requests Are Failing.");
    }
}
