using Godot;
using System;

namespace Vertigo
{
    public class Projectile : Area2D
    {
        [Export] public Affiliation Affiliation { get; private set; } = Affiliation.NEUTRAL;
        [Export] private float _speed = 64f;

        //private HitFX _hitFx;

        public Vector2 Direction { get; private set; } = Vector2.Zero;

        public override void _Ready()
        {
            base._Ready();

            this.UpdateAttackCollisionMaskBits(Affiliation);

            //_hitFx = GetNodeOrNull<HitFX>("HitFX");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            Direction = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));

            Position += Direction * _speed * delta;

            if (Position.x <= 0 || Position.x >= 480 || Position.y <= -32f || Position.y >= 302f)
            {
                this.SafeQueueFree();
            }
        }

        public void ProcessCollision()
        {
            /*if (_hitFx.IsValid())
            {
                RemoveChild(_hitFx);
                _hitFx.GlobalPosition = GlobalPosition;
                Game.Instance.GameLayer.AddChild(_hitFx);
                _hitFx.Animate();
            }*/

            this.SafeQueueFree();
        }
    }
}
