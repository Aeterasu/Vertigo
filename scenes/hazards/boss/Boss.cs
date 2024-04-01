using Godot;
using System;

namespace Vertigo
{
    public enum BossAttackType
    {
        NORMAL_BOSS,
        BOSS_SLOW_PROJECTILE,
    }

    public class Boss : Square
    {
        [Export] private readonly BossAttackType _attackType = BossAttackType.NORMAL_BOSS;
        [Export] private float _lerpWeight = 3f;
        [Export] private readonly float _sinWaveSpeed = 2.0f;
        [Export] private readonly float _sinWaveStrength = 16.0f;
        [Export] private readonly float _attackRate = 2.0f;
        [Export] private readonly PackedScene _projectileScene;
        [Export] private readonly uint _projectileCount = 3;
        [Export] private readonly float _projectileSpread = 7.0f;
        [Export] private readonly float _escapeTime = 2.0f;

        private int _subBossAttackCount = 0;

        private float _attackCooldown = 2.0f;

        private float _sinWaveTicks = 0.0f;

        private float _escapeElapsedTime = 0.0f;

        private Vector2 _offset = new Vector2();

        public Vector2 TargetPosition { get; private set; } = new Vector2();
        public Vector2 CurrentPosition { get; private set; } = new Vector2();

        public bool IsEscapeTriggered { get; private set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _attackCooldown = _attackRate;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _sinWaveTicks += _sinWaveSpeed * (1 / Slowdown) * delta;

            CurrentPosition = CurrentPosition.LinearInterpolate(TargetPosition, _lerpWeight * delta);

            _offset = Vector2.Up * Mathf.Sin(_sinWaveTicks) * _sinWaveStrength;

            GlobalPosition = CurrentPosition + _offset;

            _attackCooldown -= delta;

            if (_attackCooldown <= 0.0f)
            {
                _attackCooldown = _attackRate;
                Attack();
            }

            if (IsEscapeTriggered)
            {
                _escapeElapsedTime += delta;

                if (_escapeElapsedTime >= _escapeTime)
                {
                    this.SafeQueueFree();
                    return;
                }
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
        }

        private void Attack()
        {
            switch (_attackType)
            {
                case BossAttackType.NORMAL_BOSS:
                    if (_projectileCount % 2 != 0)
                    {
                        for (int i = 0; i < _projectileCount; i++)
                        {
                            CreateAttack(Mathf.Rad2Deg(new Vector2(-Direction, 0.0f).Angle()) + (i - (_projectileCount / 2)) * _projectileSpread);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _projectileCount + 1; i++)
                        {
                            if (i != _projectileCount / 2)
                            {
                                CreateAttack(Mathf.Rad2Deg(new Vector2(-Direction, 0.0f).Angle()) + (i - (_projectileCount / 2)) * _projectileSpread / 2);
                            }
                        }
                    }
                    break;
                case BossAttackType.BOSS_SLOW_PROJECTILE:
                    _subBossAttackCount += 1;
                    CreateAttack((_subBossAttackCount - 1) * _projectileSpread);
                    if (_subBossAttackCount >= 2)
                    {
                        _subBossAttackCount = 0;
                    }
                    break;
            }
        }

        private void CreateAttack(float directionAngleDegrees)
        {
            var attack = _projectileScene.Instance();

            if (attack is Node2D)
            {
                (attack as Node2D).GlobalPosition = GlobalPosition;
            }

            if (attack is BossProjectile)
            {
                (attack as BossProjectile).VectorDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad(directionAngleDegrees)), Mathf.Sin(Mathf.Deg2Rad(directionAngleDegrees)));
            }

            if (Game.Instance.IsValid() && Game.Instance.GameLayer.IsValid())
            {
                Game.Instance.GameLayer.AddChild(attack);
            }
        }

        public void TriggerEscape()
        {
            TargetPosition += Vector2.Right * Direction * 216.0f;

            IsEscapeTriggered = true;
        }

        public void SetDirection(int direction)
        {
            Direction = direction;

            if (direction == -1)
            {
                CurrentPosition = new Vector2(-128f, 138f);
                TargetPosition = new Vector2(72f, 138f);
            }
            else
            {
                CurrentPosition = new Vector2(480f + 128f, 138f);
                TargetPosition = new Vector2(480f - 72f, 138f);
            }

            GlobalPosition = CurrentPosition + _offset;
        }
    }
}