using Godot;
using System;

namespace Vertigo
{
    public class Square : Hazard
    {
        [Export] private float _speed = 16f;
        [Export] private float _rotationSpeed = 32f;

        [Export(PropertyHint.Range, "-1, 1, 1")] private float _direction = 1f;

        [Export] private bool _bounce = false; // squares will bounce from each other

        private HealthComponent _healthComponent;
        private HurtboxComponent _hurtboxComponent;

        private CurrencyGenerator _currencyGenerator;

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
            
            Connect("area_entered", this, "OnAreaEntered");

            _healthComponent = GetNode<HealthComponent>("%HealthComponent");
            _healthComponent.OnHealthDepleted += OnHealthDepleted;

            _hurtboxComponent = GetNode<HurtboxComponent>("%HurtboxComponent");
            _hurtboxComponent.OnProjectileCollide += OnProjectileCollide;

            _currencyGenerator = GetNode<CurrencyGenerator>("%CurrencyGenerator");

            _sprite = GetNode<Sprite>("%Sprite");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _sprite.RotationDegrees += _rotationSpeed * delta;

            Position += Vector2.Right * _speed * Direction * delta;

            if (Position.x <= -96f || Position.x >= 576f)
            {
                this.SafeQueueFree();
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
            _currencyGenerator.Generate();
            this.SafeQueueFree();
        }

        private void OnProjectileCollide(Projectile projectile)
        {
            _healthComponent.DealDamage();
            projectile.ProcessCollision();
        }
    }
}
