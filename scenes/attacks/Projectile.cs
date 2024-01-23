using Godot;
using System;

namespace Vertigo
{
    public class Projectile : Area2D
    {
        [Export] public Affiliation Affiliation { get; private set; } = Affiliation.NEUTRAL;
        [Export] private float _speed = 64f;
        [Export] private float _knockback = 4f;

        public override void _Ready()
        {
            base._Ready();

            this.UpdateAttackCollisionMaskBits(Affiliation);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            Vector2 direction = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));

            Position += direction * _speed * delta;

            if (Position.x <= 0 || Position.x >= 480 || Position.y <= 0f || Position.y >= 270f)
            {
                this.SafeQueueFree();
            }
        }
    }
}
