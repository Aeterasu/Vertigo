using Godot;
using System;

namespace Vertigo
{
    public class JumpRope : Area2D
    {
        [Export] public GravityMode GravityMode { get; private set; } = GravityMode.UPWARDS;

        public override void _Ready()
        {
            base._Ready();
        }
    }
}
