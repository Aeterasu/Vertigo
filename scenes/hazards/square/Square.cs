using Godot;
using System;

namespace Vertigo
{
    public class Square : Hazard
    {
        public const string GROUP_SQUARE = "Square";

        [Export] private float _speed = 16f;
        [Export] private float _rotationSpeed = 32f;

        [Export] private Vector2 _vectorDirection = Vector2.Right;

        [Export(PropertyHint.Range, "-1, 1, 1")] private float _direction = 1f;

        [Export] private bool _bounce = false; // squares will bounce from each other
        [Export] private bool _rotateSprite = true;

        private HealthComponent _healthComponent;
        private HurtboxComponent _hurtboxComponent;

        private CurrencyGenerator _currencyGenerator;
        
        private GrazeArea _grazeArea;

        private Sprite _sprite;

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
            
            Connect("area_entered", this, "OnAreaEntered");

            _healthComponent = GetNode<HealthComponent>("%HealthComponent");
            _healthComponent.OnHealthDepleted += OnHealthDepleted;

            _hurtboxComponent = GetNode<HurtboxComponent>("%HurtboxComponent");
            _hurtboxComponent.OnProjectileCollide += OnProjectileCollide;

            _currencyGenerator = GetNode<CurrencyGenerator>("%CurrencyGenerator");

            _grazeArea = GetNode<GrazeArea>("%GrazeArea");

            _sprite = GetNode<Sprite>("%Sprite");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_rotateSprite)
            {
                _sprite.RotationDegrees += _rotationSpeed * delta;
            }

            Position += _vectorDirection * _speed * Direction * (1 / Slowdown) * delta;

            if (Position.x <= -96f || Position.x >= 576f || Position.y <= -96f || Position.y >= 366f)
            {
                OnLeftThePlayArea?.Invoke(this);
                this.SafeQueueFree();
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            Update();
        }

        public override void _Draw()
        {
            base._Draw();

            if (_grazeArea.AffinityIncrease > 1f)
            {
                DrawArc(Vector2.Zero, _grazeArea.CircleShape2D.Radius * _grazeArea.Scale.Length(), 0, Mathf.Tau, 24, new Color(1, 1, 1, 0.35f), 1f);
            }
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is Hazard && _bounce)
            {
                Direction *= -1f;
            }
        }

        private void OnHealthDepleted()
        {
            OnDestroyed?.Invoke(this);

            _hurtboxComponent.GetNode<CollisionShape2D>("CollisionShape2D").CallDeferred("set_disabled", true);
            _hurtboxComponent.Immune = true;
            _currencyGenerator.Generate();
            this.SafeQueueFree();
        }

        private void OnProjectileCollide(Projectile projectile)
        {
            _healthComponent.DealDamage();
            projectile.ProcessCollision();

            if (Dullness.Instance.IsValid())
            {
                Slowdown += Dullness.Instance.TotalSlowdown - 1;
            }
        }
    }
}
