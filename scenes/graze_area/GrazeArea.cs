using Godot;
using System;

namespace Vertigo
{
    public class GrazeArea : Area2D
    {
        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Affinity.Instance.IsValid())
            {
                Scale = Vector2.One * Affinity.Instance.TotalIncrease;
            }
            else
            {
                Scale = Vector2.One;
            }
        }
    }
}
