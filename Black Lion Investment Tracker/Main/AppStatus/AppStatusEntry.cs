using Godot;

namespace BLIT.Status;

public partial class AppStatusEntry : Panel
{
    [Export]
    RichTextLabel statusLabel;

    string newStatus = string.Empty;

    public override void _Process(double delta)
    {
        statusLabel.Text = newStatus;
        SetProcess(false);
    }

    public void SetPendingStatus(string status)
    {
        newStatus = status;
        SetProcess(true);
    }
}
