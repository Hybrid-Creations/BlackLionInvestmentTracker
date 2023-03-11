using System.Collections.Concurrent;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using Godot;
using Godot.Collections;

namespace BLIT.Status;

public partial class AppStatusManager : VBoxContainer
{
    [Export]
    PackedScene appStatusScene;

    static readonly ConcurrentDictionary<string, AppStatusEntry> statusMessages = new();

    static AppStatusManager Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public static void ShowStatus(string key, string status)
    {
        if (statusMessages.TryGetValue(key, out var statusEntry))
            statusEntry.SetPendingStatus(status.AlignRichString(RichStringAlignment.RIGHT));
        else
        {
            var isntance = Instance.appStatusScene.Instantiate<AppStatusEntry>();
            isntance.SetPendingStatus(status.AlignRichString(RichStringAlignment.RIGHT));
            Instance.AddChildSafe(isntance);
            statusMessages[key] = isntance;
        }
    }

    public static void ClearStatus(string key)
    {
        if (statusMessages.TryGetValue(key, out var statusEntry))
        {
            if (statusMessages.TryRemove(key, out var _) == false)
                GD.PrintErr($"Failed to remove status entry");

            statusEntry.QueueFreeSafe();
        }
    }
}
