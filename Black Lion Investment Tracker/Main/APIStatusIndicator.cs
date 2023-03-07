using Godot;

namespace BLIT;

public partial class APIStatusIndicator : Panel
{
    [Export]
    RichTextLabel statusLabel;

    static APIStatusIndicator Instance;

    static string newStatus = "";

    public override void _Ready()
    {
        Instance = this;
        statusLabel.Clear();
        Hide();
    }

    public override void _Process(double delta)
    {
        if (string.IsNullOrWhiteSpace(newStatus) == false)
            statusLabel.Text = $"[center][right]{newStatus}";

        if (newStatus == "clear")
        {
            statusLabel.Text = "";
            Instance.Hide();
            Instance.SetProcess(false);
        }
    }

    public static void ShowStatus(string status)
    {
        newStatus = $"{status} => [color=cyan][url=https://status.gw2efficiency.com/]Check End Points[/url][/color] <= This App Uses /v2/commerce/*";
        Instance.Show();
        Instance.SetProcess(true);
    }

    public static void ClearStatus()
    {
        newStatus = "clear";
    }
}
