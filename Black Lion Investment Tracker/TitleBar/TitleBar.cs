using BLIT.Extensions;
using BLIT.Investments;
using BLIT.Saving;
using BLIT.UI;
using Godot;

namespace BLIT;

public partial class TitleBar : Control
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.File, "*.tscn")]
    string settingsScene;

    [Export]
    PackedScene restorePopup;

    [Export]
    Button restoreButton;

    [Export]
    Main main;

    Window window;
    Vector2 dragStartPosition;

    public override void _Ready()
    {
        CheckForBackups();
        window = GetWindow();
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        window.Position = window.Position + GetGlobalMousePosition().ToVector2I() - dragStartPosition.ToVector2I();
        if (Input.IsMouseButtonPressed(MouseButton.Left) == false)
            SetProcess(false);
    }

    void CheckForBackups()
    {
        if (SaveSystem.BackupsExist(InvestmentsDatabase.DatabaseFileName) == false)
            restoreButton.Hide();
    }

    public void GUIInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouse)
        {
            if (mouse.ButtonMask == MouseButtonMask.Left)
            {
                dragStartPosition = mouse.GlobalPosition;
                SetProcess(!IsProcessing());
            }
        }
    }

    public void OpenRestoreDialogue()
    {
        var dialogue = restorePopup.Instantiate<RestorePopup>();
        dialogue.Setup(main);
        GetTree().Root.AddChild(dialogue);
        dialogue.OnPopupAccepted += CheckForBackups;
    }

    public void OpenSettings()
    {
        GetTree().ChangeSceneToFile(settingsScene);
    }

    public void MinimizeApp()
    {
        GetWindow().Mode = Window.ModeEnum.Minimized;
    }

    public void CloseApp()
    {
        main.CloseApp();
    }
}
