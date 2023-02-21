using Godot;

namespace blit.Extensions;

public static class NodeExtensions
{
    public static void AddChildSafe(this Node node, Node child, int index = -1)
    {
        node.CallDeferred("add_child", child);
        if (index > -1)
            node.CallDeferred("move_child", child, index);
    }
    public static void AddChild(this Node node, Node child, int index = -1)
    {
        node.AddChild(child);
        if (index > -1)
            node.MoveChild(child, index);
    }

    public static void QueueFreeSafe(this Node node)
    {
        node.CallDeferred("queue_free", node);
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
