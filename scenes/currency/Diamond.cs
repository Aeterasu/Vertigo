using Godot;
using System;

namespace Vertigo
{
    public class Diamond : Sprite
    {
        [Export] private float _worth = 1f;
        [Export] private float _startupDuration = 1f;
        [Export] private float _speed = 64f;
        [Export] private float _distanceThreshold = 6f;

        private Vector2 _randomDirection = Vector2.Zero;

        private float _currentSpeed = 0f;

        private Player _player;

        private CPUParticles2D _cpuParticle2D;

        public override void _Ready()
        {
            base._Ready();

            foreach (Node node in GetTree().GetNodesInGroup(Player.GROUP_PLAYER))
            {
                if (node is Player)
                {
                    _player = node as Player;
                    break;
                }
            }

            _randomDirection = new Vector2(
                (float)GD.RandRange(-1, 1),
                (float)GD.RandRange(-1, 1)
            ).Normalized();

            _startupDuration *= (float)GD.RandRange(0.5f, 1.5f);

            _cpuParticle2D = GetNode<CPUParticles2D>("CPUParticles2D");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_player.IsDead)
            {
                _cpuParticle2D.Emitting = false;
            }

            if (GlobalPosition.DistanceTo(_player.GlobalPosition) <= _distanceThreshold && !_player.IsDead)
            {
                var experienceComponent = _player.GetNodeOrNull<ExperienceComponent>("ExperienceComponent");

                if (experienceComponent.IsValid())
                {
                    experienceComponent.GrantExp(_worth);
                }

                if (Game.Instance.IsValid())
                {
                    DeconflictedSoundPlayer.PlayUnconflictingSound(Game.Instance.GemPickupSound, Game.Instance, 0, 1);
                }

                this.SafeQueueFree();
            }

            if (_startupDuration > 0)
            {
                _startupDuration -= delta;
                GlobalPosition += _randomDirection * _speed * 1.1f * delta;
                _randomDirection = _randomDirection.LinearInterpolate(Vector2.Zero, 3 * delta);
                return;
            }

            if (_player == null)
            {
                return;
            }

            _currentSpeed = Mathf.Lerp(_currentSpeed, _speed, 9 * delta);

            GlobalPosition += GlobalPosition.DirectionTo(_player.GlobalPosition) * _currentSpeed * delta;
        }
    }
}
