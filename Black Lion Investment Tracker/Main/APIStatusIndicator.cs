using Godot;

namespace BLIT;

public partial class APIStatusIndicator : Panel
{
    [Export]
    RichTextLabel statusLabel;

    static APIStatusIndicator Instance;

    public override void _Ready()
    {
        Instance = this;
        statusLabel.Clear();
        Hide();
    }

    public static void ShowStatus(string status)
    {
        ThreadsHelper.CallOnMainThread(() =>
        {
            Instance.statusLabel.Text = $"[center][right]{status} => [color=cyan][url=https://status.gw2efficiency.com/]Check End Points[/url][/color] <= This App Uses /v2/commerce/*";
            ;
            Instance.Show();
        });
    }

    public static void ClearStatus()
    {
        ThreadsHelper.CallOnMainThread(() =>
        {
            Instance.statusLabel.Clear();
            Instance.Hide();
        });
    }
}
