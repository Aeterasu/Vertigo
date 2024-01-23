using Godot;
using System;

namespace Vertigo
{
    public class SpawnPoint : Position2D
    {
        [Export(PropertyHint.Range, "-1, 1, 1")] public float Direction { get; private set; } = 1f;
    }
}