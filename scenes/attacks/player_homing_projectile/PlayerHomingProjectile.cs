using Godot;
using System;

namespace Vertigo
{
    public class PlayerHomingProjectile : Projectile
    {
        [Export] private readonly float _deletionThreshold = 20f; // if a projectile can't hit the target before this time it will be deleted
        [Export] private readonly int _refreshTickrate = 5;
        private int _ticks = 15;

        private float _deletionTimer = 0f;

        public Vector2 Target = Vector2.Zero;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _ticks -= 1;

            if (_ticks <= 0)
            {
                _ticks = _refreshTickrate;
                UpdateTarget();
            }

            _deletionTimer += delta;

            if (_deletionTimer >= _deletionThreshold)
            {
                this.SafeQueueFree();
            }

            Rotation = Mathf.LerpAngle(Rotation, Mathf.Deg2Rad(Mathf.Rad2Deg(GlobalPosition.AngleToPoint(Target)) + 180), 6 * delta);
        }

        public void UpdateTarget()
        {
            float minDistance = 999999f;

            foreach (Node node in GetTree().GetNodesInGroup(Square.GROUP_SQUARE))
            {
                if (node is Square)
                {
                    Square square = node as Square;

                    if (GlobalPosition.DistanceTo(square.GlobalPosition) < minDistance)
                    {
                        minDistance = GlobalPosition.DistanceTo(square.GlobalPosition);
                        Target = square.GlobalPosition;
                    }
                }
            }
        }
    }
}
