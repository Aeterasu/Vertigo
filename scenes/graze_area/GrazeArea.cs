using Godot;
using System;

namespace Vertigo
{
    public class GrazeArea : Area2D
    {
        public float AffinityIncrease { get; private set; } = 1f;

        public CollisionShape2D CollisionShape2D { get; private set; }
        public CircleShape2D CircleShape2D { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            CollisionShape2D = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

            if (CollisionShape2D.Shape is CircleShape2D)
            {
                CircleShape2D = CollisionShape2D.Shape as CircleShape2D;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Affinity.Instance.IsValid())
            {
                AffinityIncrease = Affinity.Instance.TotalIncrease;
            }
            else
            {
                AffinityIncrease = 1f;
            }

            Scale = Vector2.One * AffinityIncrease;
        }
    }
}
