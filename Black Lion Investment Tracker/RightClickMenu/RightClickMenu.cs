using System;
using System.Linq;
using Godot;

public partial class RightClickMenu : PanelContainer
{
    static RightClickMenu MainInstance;

    public static void OpenMenu(Node rootNode, Vector2 mousePosition, string[] buttons, Action<string> onButtonPress)
    {
        var scene = GD.Load<PackedScene>("res://RightClickMenu/RightClickMenu.tscn");
        var instance = scene.Instantiate<RightClickMenu>();
        instance.Position = mousePosition;

        var btnContainer = instance.GetNode<VBoxContainer>("ButtonContainer");

        foreach (var btnText in buttons)
        {
            var btn = new Button();
            btn.Text = btnText;
            btn.Flat = true;
            btn.Connect(Button.SignalName.Pressed, Callable.From(() =>
            {
                onButtonPress.Invoke(btnText);
                CloseInstance();
            }));
            btnContainer.AddChild(btn);

            if (btnText != buttons.Last())
            {
                var rect = new ColorRect();
                rect.Color = Color.Color8(128, 128, 128);
                rect.CustomMinimumSize = new Vector2(0, 1);
                btnContainer.AddChild(rect);
            }
        }

        var closeBtn = new Button();
        closeBtn.Text = "Close";
        closeBtn.Flat = true;
        closeBtn.Connect(Button.SignalName.Pressed, Callable.From(() => CloseInstance()));
        btnContainer.AddChild(closeBtn);
        rootNode.AddChild(instance);

        CloseInstance();
        MainInstance = instance;
    }

    public static void CloseInstance()
    {
        if (MainInstance?.IsQueuedForDeletion() == false)
        {
            MainInstance?.QueueFree();
            MainInstance = null;
        }
    }
}
