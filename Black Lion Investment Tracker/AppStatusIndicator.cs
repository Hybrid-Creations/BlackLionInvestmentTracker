using Godot;

public partial class AppStatusIndicator : Panel
{
    [Export]
    RichTextLabel statusLabel;

    static AppStatusIndicator Instance;

    public override void _Ready()
    {
        base._Ready();

        Instance = this;
        statusLabel.Clear();
        Hide();
    }

    public static void ShowStatus(string status)
    {
        Instance.statusLabel.Text = $"[center][right]{status}";
        Instance.Show();
    }

    public static void ClearStatus()
    {
        Instance.statusLabel.Text = $"";
        Instance.Hide();
    }
}
