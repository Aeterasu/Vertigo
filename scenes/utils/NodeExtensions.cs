using Godot;
using System;

public static class NodeExtensions
{
    public static bool IsValid<T>(this T node) where T : Godot.Object
    {
        return node != null
            && Godot.Object.IsInstanceValid(node)
            && !node.IsQueuedForDeletion();  
    }

    public static void SafeQueueFree(this Node node)
    {
        if (node.IsValid())
        {
            node.QueueFree();
        }
    }

    public static T IfValid<T>(this T control) where T : Godot.Object => control.IsValid() ? control : null;
}