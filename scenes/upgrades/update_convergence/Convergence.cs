using Godot;
using System;

namespace Vertigo
{
    public class Convergence : Upgrade
    {
        [Export] private readonly int _initialProjectileCount = 1;
        [Export] private readonly int _projectilesPerStack = 1;
        [Export] private readonly float _spread = 8f;

        private JumpRope _jumpRopeTop;
        private JumpRope _jumpRopeBottom;

        public int ProjectileCount { get; private set; } = 0;

        public override void _Ready()
        {
            base._Ready();

            ProjectileCount += _initialProjectileCount;

            _jumpRopeTop = GetNodeOrNull<JumpRope>("%JumpRopeTop");
            _jumpRopeBottom = GetNodeOrNull<JumpRope>("%JumpRopeBottom");

            if (_jumpRopeTop.IsValid())
            {
                _jumpRopeTop.OnJumpRopeActivated += OnJumpRopeTopActivated;
            }

            if (_jumpRopeBottom.IsValid())
            {
                _jumpRopeBottom.OnJumpRopeActivated += OnJumpRopeBottomActivated;
            }
        }

        public override void AddStack()
        {
            base.AddStack();

            ProjectileCount += _projectilesPerStack;
        }

        private void OnJumpRopeTopActivated(JumpRope jumpRope)
        {
            if (!Game.Instance.IsValid())
            {
                return;
            }

            for (int i = 0; i < ProjectileCount; i++)
            {
                CreateProjectile(90, jumpRope, (i - (ProjectileCount / 2)) * _spread);
            }
        }

        private void OnJumpRopeBottomActivated(JumpRope jumpRope)
        {
            if (!Game.Instance.IsValid())
            {
                return;
            }

            for (int i = 0; i < ProjectileCount; i++)
            {
                CreateProjectile(-90, jumpRope, (i - (ProjectileCount / 2)) * _spread);
            }
        }

        private void CreateProjectile(float directionAngleDegrees, JumpRope jumpRope, float spread = 0)
        {
            var attack = Game.Instance.ConvergenceProjectileScene.Instance();

            if (attack is Node2D)
            {
                (attack as Node2D).GlobalPosition = jumpRope.LastPlayerPosition;
                (attack as Node2D).RotationDegrees = directionAngleDegrees;
            }

            if (attack is PlayerHomingProjectile)
            {
                (attack as PlayerHomingProjectile).Target = jumpRope.LastPlayerPosition + Vector2.Right * spread + Vector2.Down * (int)jumpRope.GravityMode * 256f;
            }

            if (Game.Instance.IsValid() && Game.Instance.GameLayer.IsValid())
            {
                Game.Instance.GameLayer.CallDeferred("add_child", attack);
            }
        }
    }
}