using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public enum HazardSpriteRotationMode
    {
        NONE,
        SQUARE,
        ARROW,
    }

    public class Square : Hazard
    {
        public const string GROUP_SQUARE = "Square";

        [Export] private readonly float _speed = 16f;
        [Export] private readonly float _rotationSpeed = 32f;

        [Export] private Vector2 _vectorDirection = Vector2.Right;

        [Export(PropertyHint.Range, "-1, 1, 1")] private float _direction = 1f;
        [Export] private readonly bool directionAffectX = true;
        [Export] private readonly bool directionAffectY = true;

        [Export] private readonly bool _bounce = false; // squares will bounce from each other
        [Export] private readonly bool _rotateSprite = true;
        [Export] private readonly HazardSpriteRotationMode _spriteRotationMode = HazardSpriteRotationMode.SQUARE;

        private HealthComponent _healthComponent;
        private HurtboxComponent _hurtboxComponent;

        private CurrencyGenerator _currencyGenerator;
        
        private GrazeArea _grazeArea;
        private GrazeVisual _grazeVisual;

        private Sprite _sprite;

        private SquareDestroyedFX _squareDestroyedFx;

        private CPUParticles2D _slowdownFx;

        private readonly List<Area2D> _collisionList = new List<Area2D>();

        public float Direction
        {
            get => _direction;
            set
            {
                if (value < -1)
                {
                    value = -1;
                }

                if (value > 1)
                {
                    value = 1;
                }

                _direction = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            AddToGroup(GROUP_SQUARE);

            _healthComponent = GetNode<HealthComponent>("%HealthComponent");
            _healthComponent.OnHealthDepleted += OnHealthDepleted;

            _hurtboxComponent = GetNode<HurtboxComponent>("%HurtboxComponent");
            _hurtboxComponent.OnProjectileCollide += OnProjectileCollide;

            _currencyGenerator = GetNode<CurrencyGenerator>("%CurrencyGenerator");

            _grazeArea = GetNode<GrazeArea>("%GrazeArea");
            _grazeVisual = GetNodeOrNull<GrazeVisual>("GrazeVisual");

            _sprite = GetNode<Sprite>("%Sprite");

            _squareDestroyedFx = GetNodeOrNull<SquareDestroyedFX>("SquareDestroyedFX");

            if (_squareDestroyedFx.IsValid())
            {
                RemoveChild(_squareDestroyedFx);
            }

            _slowdownFx = GetNodeOrNull<CPUParticles2D>("SlowdownFx");

            if (_slowdownFx.IsValid())
            {
                _slowdownFx.Emitting = false;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_rotateSprite)
            {
                switch (_spriteRotationMode)
                {
                    case HazardSpriteRotationMode.NONE:
                        break;
                    case HazardSpriteRotationMode.SQUARE:
                        _sprite.RotationDegrees += _rotationSpeed * delta;
                        break;
                    case HazardSpriteRotationMode.ARROW:
                        _sprite.RotationDegrees = 90 * Direction;
                        break;
                }
            }

            Vector2 targetDirection = _vectorDirection.Normalized();

            if (directionAffectX)
            {
                targetDirection.x *= Direction;
            }

            if (directionAffectY)
            {
                targetDirection.y *= Direction;
            }

            Position += targetDirection * _speed * (1 / Slowdown) * delta;

            if (Position.x <= -96f || Position.x >= 576f || Position.y <= -96f || Position.y >= 366f)
            {
                OnLeftThePlayArea?.Invoke(this);
                this.SafeQueueFree();
            }

            var areas = GetOverlappingAreas();

            foreach (Area2D area in areas)
            {
                if (area is Hazard && _bounce && !_collisionList.Contains(area))
                {
                    Direction *= -1f;
                    _collisionList.Add(area);
                }
            }

            foreach (Area2D area in new List<Area2D>(_collisionList))
            {
                if (!areas.Contains(area))
                {
                    _collisionList.Remove(area);
                }
            }

            if (_grazeVisual.IsValid())
            {
                if (_grazeArea.AffinityIncrease > 1f)
                {
                    _grazeVisual.TargetScale = _grazeArea.AffinityIncrease;
                    _grazeVisual.TargetAlpha = 0.5f;
                }
                else
                {
                    _grazeVisual.TargetScale = 1f;
                    _grazeVisual.TargetAlpha = 0.5f;
                }
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            Update();
        }

        private void OnHealthDepleted()
        {
            OnDestroyed?.Invoke(this);

            _hurtboxComponent.GetNode<CollisionShape2D>("CollisionShape2D").CallDeferred("set_disabled", true);
            _hurtboxComponent.Immune = true;
            _currencyGenerator.Generate();

            if (_squareDestroyedFx.IsValid())
            {
                _squareDestroyedFx.GlobalPosition = GlobalPosition;
                _squareDestroyedFx.SquareSprite.RotationDegrees = _sprite.RotationDegrees;
                Game.Instance.GameLayer.AddChild(_squareDestroyedFx);
                _squareDestroyedFx.Animate();
            }

            this.SafeQueueFree();
        }

        private void OnProjectileCollide(Projectile projectile)
        {
            _healthComponent.DealDamage();
            projectile.ProcessCollision();

            if (Dullness.Instance.IsValid())
            {
                Slowdown += Dullness.Instance.TotalSlowdown - 1;
                
                if (_slowdownFx.IsValid())
                {
                    _slowdownFx.Emitting = true;
                }
            }
        }

        public void Destroy()
        {
            OnHealthDepleted();
        }
    }
}
