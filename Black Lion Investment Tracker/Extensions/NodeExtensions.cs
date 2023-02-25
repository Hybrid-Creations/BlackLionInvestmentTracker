using Godot;

namespace BLIT.Extensions;

public static class NodeExtensions
{
    public static void AddChildSafe(this Node node, Node child, int index = -1)
    {
        node.CallDeferred(Node.MethodName.AddChild, child);
        if (index > -1)
            node.CallDeferred(Node.MethodName.MoveChild, child, index);
    }
    public static void AddChild(this Node node, Node child, int index = -1)
    {
        node.AddChild(child);
        if (index > -1)
            node.MoveChild(child, index);
    }

    public static void QueueFreeSafe(this Node node)
    {
        node.CallDeferred(Node.MethodName.QueueFree);
    }

    public static void ClearChildrenSafe(this Node node)
    {
        foreach (var child in node.GetChildren())
            child.QueueFreeSafe();
    }

    public static void ClearChildren(this Node node)
    {
        foreach (var child in node.GetChildren())
            child.QueueFree();
    }
}
