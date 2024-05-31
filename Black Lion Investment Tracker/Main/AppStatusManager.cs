using System.Collections.Concurrent;
using BLIT.Tools;
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
                var entryInstance = Instance?.appStatusScene.Instantiate<AppStatusEntry>();
                entryInstance.SetPendingStatus(status.AlignRichString(RichStringAlignment.RIGHT));
                Instance.AddChildSafe(entryInstance);
                statusMessages[key] = entryInstance;
            });
        }
    }

    public static void ClearStatus(string key)
    {
        Callable
            .From(() =>
            {
                if (statusMessages.TryGetValue(key, out var statusEntry))
                {
                    if (statusMessages.TryRemove(key, out var _))
                        statusEntry.QueueFreeSafe();
                    else
                        GD.PrintErr($"Failed to remove status entry");
                }
                else
                    GD.PushWarning($"Status was not in dictionary {key}");
            })
            .CallDeferred(); // This guarantees that we don't miss removing an entry that was added durring this frame
    }
}
