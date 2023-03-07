using BLIT.Extensions;
using Godot;

namespace BLIT;

public partial class TitleBar : Control
{
    Window window;
    Vector2 dragStartPosition;

    public override void _Ready()
    {
        window = GetWindow();
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        window.Position = window.Position + GetGlobalMousePosition().ToVector2I() - dragStartPosition.ToVector2I();
        if (Input.IsMouseButtonPressed(MouseButton.Left) == false)
            SetProcess(false);
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
}
