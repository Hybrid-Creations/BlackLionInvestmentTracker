using Godot;

public static class NodeExtensions
{
    public static void AddChildSafe(this Node node, Node child, int index = -1)
    {
        node.CallDeferred("add_child", child);
        if (index > -1)
            node.CallDeferred("move_child", child, index);
    }
}
