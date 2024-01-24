using Godot;
using System;

namespace Vertigo
{
    public enum PlayerState
    {
        NORMAL,
        DEAD,
    }

    public class Player : Area2D
    {
        [Export] private readonly float _gravity = 8f;
        [Export] private readonly float _springCoeff = 0.3f;
        [Export] private readonly float _horizontalSpeed = 4f;
        [Export] private readonly float _knockbackFalloff = 8f;
        [Export] private readonly float _verticalStallFalloff = 12f;

        [Export] private readonly float _boundaryLeft = -9f;
        [Export] private readonly float _boundaryRight = 649f;

        private GravityMode _gravityMode = GravityMode.DOWNWARDS;

        private float _velocityX = 0f;
        private float _velocityY = 0f;

        private float _verticalStallTimer = 0f;

        private float _knockbackX = 0f;

        private PlayerGun _playerGun;
        private PowerComponent _powerComponent;
        private float _firingAngleDegrees = 0f;
        private float _directionX = 1f;

        private HealthComponent _healthComponent;

        private GrazeDetector _grazeDetector;

        private PlayerState _state = PlayerState.NORMAL;

        private Node2D _spritePivot;
        private AnimationPlayer _animationPlayer;

        public GravityMode GravityMode
        {
            get => _gravityMode;
            private set
            {
                if (_gravityMode != value)
                {
                    _gravityMode = value;

                    if (value == GravityMode.UPWARDS)
                    {
                        _animationPlayer.Play("FlipUp");
                    }
                    else if (value == GravityMode.DOWNWARDS)
                    {
                        _animationPlayer.Play("FlipDown");
                    }
                }
            }
        }

        public bool IsGrazing { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Connect("area_entered", this, "OnAreaEntered");

            _playerGun = GetNode<PlayerGun>("%PlayerGun");
            _playerGun.OnFired += OnGunFired;

            _powerComponent = GetNode<PowerComponent>("%PowerComponent");

            _healthComponent = GetNode<HealthComponent>("%HealthComponent");
            _healthComponent.OnHealthDepleted += OnHealthDepleted;

            _grazeDetector = GetNode<GrazeDetector>("%GrazeDetector");

            _spritePivot = GetNode<Node2D>("%SpritePivot");
            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _spritePivot.Scale = _spritePivot.Scale.LinearInterpolate(new Vector2(_directionX, 1f), 10f * delta);

            HandleInput();

            HandleMovement(delta);

            HandleGraze();

            GD.Print(IsGrazing);
        }

        private void HandleInput()
        {
            _velocityX = 0f;

            if (_state == PlayerState.DEAD)
            {
                return;
            }

            if (Input.IsActionPressed("move_left"))
            {
                _velocityX = -_horizontalSpeed;
                _firingAngleDegrees = 180f;
                _directionX = -1f;
            }

            if (Input.IsActionPressed("move_right"))
            {
                _velocityX = _horizontalSpeed;
                _firingAngleDegrees = 0f;
                _directionX = 1f;
            }

            if (Input.IsActionJustPressed("action_confirm") && _playerGun.IsValid())
            {
                _playerGun.Fire(directionAngleDegrees: _firingAngleDegrees, power: _powerComponent);
            }
        }

        private void HandleMovement(float delta)
        {
            if (_state == PlayerState.DEAD)
            {
                return;
            }

            if (_verticalStallTimer <= 0)
            {
                _velocityY += _gravity * (int)GravityMode * delta;
            }
            else
            {
                _velocityY = Mathf.Lerp(_velocityY, 0f, _verticalStallFalloff * delta);
            }

            _knockbackX = Mathf.Lerp(_knockbackX, 0f, _knockbackFalloff * delta);

            Position += new Vector2(_velocityX, _velocityY) + new Vector2(_knockbackX, 0f);

            if (Position.x < _boundaryLeft)
            {
                Position = new Vector2(_boundaryRight, Position.y);
            }
            else if (Position.x > _boundaryRight)
            {
                Position = new Vector2(_boundaryLeft, Position.y);
            }

            _verticalStallTimer -= delta;
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is JumpRope)
            {
                var jumpRope = area as JumpRope;

                if (jumpRope.GravityMode != GravityMode)
                {
                    GravityMode = jumpRope.GravityMode;
                    _velocityY *= _springCoeff;

                    jumpRope.AnimateSmoothPath(Position.x, -(float)jumpRope.GravityMode);
                }
            }

            if (area is Hazard)
            {
                _healthComponent.DealDamage();
            }
        }

        private void OnGunFired()
        {
            _verticalStallTimer = _playerGun.VerticalStall;
            _knockbackX += _playerGun.HorizontalKnockback * _directionX;

            Game.Instance.IfValid().ScreenShakeCamera2D.IfValid().AddShake(_playerGun.ScreenShakeStrength);
        }

        private void OnHealthDepleted()
        {
            _state = PlayerState.DEAD;
        }

        private void HandleGraze()
        {
            if (!_grazeDetector.IsValid())
            {
                return;
            }

            IsGrazing = _grazeDetector.IsGrazing;
            _powerComponent.IsGrazing = IsGrazing;
        }
    }
}