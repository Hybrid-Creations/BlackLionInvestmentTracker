using Godot;

namespace BLIT.Extensions;

public static class CanvasItemExtensions
{
    public static void HideSafe(this CanvasItem canvasItem)
    {
        canvasItem.CallDeferred(CanvasItem.MethodName.Hide);
    }

    public static void ShowSafe(this CanvasItem canvasItem)
    {
        canvasItem.CallDeferred(CanvasItem.MethodName.Hide);
    }
}
