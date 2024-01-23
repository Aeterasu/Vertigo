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

            UpdateCollisionMaskBits(Affiliation);
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

        private void UpdateCollisionMaskBits(Affiliation affiliation)
        {
            // 0 - player layer
            // 2 - enemy layer
            // 3 - player attacks layer
            // 4 - enemy attack layer

            switch (affiliation)
            {
                case Affiliation.NEUTRAL:
                    SetCollisionMaskBit(0, true);
                    SetCollisionMaskBit(2, true);

                    SetCollisionLayerBit(3, true);
                    SetCollisionLayerBit(4, true);
                    break;
                case Affiliation.PLAYER:
                    SetCollisionMaskBit(0, false);
                    SetCollisionMaskBit(2, true);

                    SetCollisionLayerBit(3, true);
                    SetCollisionLayerBit(4, false);
                    break;
                case Affiliation.ENEMY:
                    SetCollisionMaskBit(0, true);
                    SetCollisionMaskBit(2, false);

                    SetCollisionLayerBit(3, false);
                    SetCollisionLayerBit(4, true);
                    break;
            }
        }
    }
}
