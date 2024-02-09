using Godot;
using System;

namespace Vertigo
{
    public class Cliff : Node2D
    {
        public override void _Ready()
        {
            base._Ready();

            GetNode<AnimationPlayer>("%AnimationPlayer").Play("Idle");
        }
    }
}