using Godot;
using System;

namespace BLIT.Tools;

public static class ThreadsHelper
{
    static int mainThreadId;

    internal static void SetMainThreadId(int threadId)
    {
        mainThreadId = threadId;
    }

    public static void CallOnMainThread(Action action)
    {
        if (action is null) return;

        if (mainThreadId == System.Environment.CurrentManagedThreadId)
            action.Invoke();
        else
            Callable.From(action).CallDeferred();
    }
}
