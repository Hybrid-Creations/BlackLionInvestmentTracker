using System.Collections.Concurrent;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using Godot;

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
            ThreadsHelper.CallOnMainThread(() =>
            {
                GD.Print(status);
                var instance = Instance.appStatusScene.Instantiate<AppStatusEntry>();
                instance.SetPendingStatus(status.AlignRichString(RichStringAlignment.RIGHT));
                Instance.AddChildSafe(instance);
                statusMessages[key] = instance;
            });
        }
    }

    public static void ClearStatus(string key)
    {
        if (statusMessages.TryGetValue(key, out var statusEntry))
        {
            if (statusMessages.TryRemove(key, out var _))
                statusEntry.QueueFreeSafe();
            else
                GD.PrintErr($"Failed to remove status entry");
        }
    }
}
