using Godot;
using System;

namespace Vertigo
{
    public enum PlayerState
    {
        NORMAL,
        DEAD,
    }

    public enum PlayerBoundaryType
    {
        CLAMP,
        WRAP_AROUND,
    }

    public class Player : Area2D
    {
        public const string GROUP_PLAYER = "Player";

        [Export] private readonly float _gravity = 8f;
        [Export] private readonly float _springCoeff = 0.3f;
        [Export] private readonly float _horizontalSpeed = 4f;
        [Export] private readonly float _knockbackFalloff = 8f;
        [Export] private readonly float _verticalStallFalloff = 12f;

        [Export] private readonly PlayerBoundaryType _boundaryType = PlayerBoundaryType.CLAMP;
        [Export] private readonly float _boundaryLeft = -9f;
        [Export] private readonly float _boundaryRight = 649f;

        [Export(PropertyHint.Range, "0, 32")] private readonly float _onHitImmunity = 1f;
        [Export(PropertyHint.Range, "0, 1")] private readonly float _onHitImmunityFlashRate = 0.4f;
        private float _onHitImmunityFlashCooldown = 0f;
        private bool _isFlashing = true;

        private GravityMode _gravityMode = GravityMode.DOWNWARDS;

        private float _velocityX = 0f;
        private float _velocityY = 0f;

        private float _verticalStallTimer = 0f;

        private float _knockbackX = 0f;

        private PlayerGun _playerGun;
        private PowerComponent _powerComponent;
        private float _firingAngleDegrees = 0f;
        private float _directionX = 1f;

        private ExperienceComponent _experienceComponent;

        private HealthComponent _healthComponent;

        private GrazeDetector _grazeDetector;

        private PlayerState _state = PlayerState.NORMAL;

        private Node2D _spritePivot;
        private AnimationPlayer _animationPlayer;

        private ImmunityBubble _immunityBubble;

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

        public float ImmunityTimer { get; set; } = 0f;
        public bool IsImmune { get => ImmunityTimer > 0f; }

        public float OnHitImmunityTimer { get; set; } = 0f;
        public bool IsOnHitImmune { get => OnHitImmunityTimer > 0f; }

        public bool IsDead { get => _healthComponent.IfValid().IsDead; }

        public event Action OnPlayerHit;
        public event Action OnPlayerDeath;

        public override void _Ready()
        {
            base._Ready();

            AddToGroup(GROUP_PLAYER);

            _playerGun = GetNode<PlayerGun>("%PlayerGun");
            _playerGun.OnFired += OnGunFired;

            _powerComponent = GetNode<PowerComponent>("%PowerComponent");

            _experienceComponent = GetNode<ExperienceComponent>("%ExperienceComponent");

            _healthComponent = GetNode<HealthComponent>("%HealthComponent");
            _healthComponent.OnHealthDepleted += OnHealthDepleted;

            _grazeDetector = GetNode<GrazeDetector>("%GrazeDetector");

            _spritePivot = GetNode<Node2D>("%SpritePivot");
            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

            _immunityBubble = GetNode<ImmunityBubble>("%ImmunityBubble");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _spritePivot.Scale = _spritePivot.Scale.LinearInterpolate(new Vector2(_directionX, 1f), 10f * delta);

            ImmunityTimer = Mathf.Max(ImmunityTimer - delta, 0f);
            OnHitImmunityTimer = Mathf.Max(OnHitImmunityTimer - delta, 0f);

            HandleAreas();

            HandleInput();

            HandleMovement(delta);

            HandleGraze();

            HandleImmunityBubble();

            HandleOnHitImmunityFlash(delta);
        }

        private void HandleInput()
        {
            _velocityX = 0f;

            if (_state == PlayerState.DEAD)
            {
                return;
            }

            if (Input.IsActionPressed(InputActions.INPUT_ACTION_MOVE_LEFT))
            {
                _velocityX = -_horizontalSpeed;
                _firingAngleDegrees = 180f;
                _directionX = -1f;
            }

            if (Input.IsActionPressed(InputActions.INPUT_ACTION_MOVE_RIGHT))
            {
                _velocityX = _horizontalSpeed;
                _firingAngleDegrees = 0f;
                _directionX = 1f;
            }

            if (Input.IsActionPressed(InputActions.INPUT_ACTION_CONFIRM) && _playerGun.IsValid())
            {
                _playerGun.Fire(directionAngleDegrees: _firingAngleDegrees, power: _powerComponent);
            }

            if (Input.IsActionJustReleased(InputActions.INPUT_ACTION_CONFIRM) && _playerGun.IsValid())
            {
                _playerGun.ResetCooldownOnMash();
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

            Position += (new Vector2(_velocityX, _velocityY) + new Vector2(_knockbackX, 0f)) * delta;

            if (_boundaryType == PlayerBoundaryType.WRAP_AROUND)
            {
                if (Position.x < _boundaryLeft)
                {
                    Position = new Vector2(_boundaryRight, Position.y);
                }
                else if (Position.x > _boundaryRight)
                {
                    Position = new Vector2(_boundaryLeft, Position.y);
                }
            }
            else if (_boundaryType == PlayerBoundaryType.CLAMP)
            {
                Position = new Vector2(Mathf.Clamp(Position.x, 0, 480), Position.y);
            }

            _verticalStallTimer -= delta;
        }

        private void HandleAreas()
        {
            foreach (Area2D area in GetOverlappingAreas())
            {
                if (area is JumpRope)
                {
                    var jumpRope = area as JumpRope;

                    if (jumpRope.GravityMode != GravityMode)
                    {
                        GravityMode = jumpRope.GravityMode;
                        _velocityY *= _springCoeff;

                        jumpRope.LastPlayerPosition = GlobalPosition;
                        jumpRope.AnimateSmoothPath(Position.x, -(float)jumpRope.GravityMode);
                        jumpRope.OnJumpRopeActivated?.Invoke(jumpRope);
                    }
                }

                if (area is Hazard && !IsImmune && !IsOnHitImmune && !_healthComponent.IsDead)
                {
                    _healthComponent.DealDamage();

                    Game.Instance.IfValid().ScreenShakeCamera2D.IfValid().AddShake(6f);

                    OnPlayerHit?.Invoke();
                    
                    if (_healthComponent.Health > 0)
                    {
                        OnHitImmunityTimer += _onHitImmunity;
                    }

                    if (area is Square)
                    {
                        (area as Square).Destroy();
                    }
                }
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
            _experienceComponent.Enabled = false;

            OnPlayerDeath?.Invoke();

            if (TimeManager.Instance.IsValid())
            {
                TimeManager.Instance.StopTimer();
            }
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

        private void HandleImmunityBubble()
        {
            _immunityBubble.IsImmune = IsImmune;

            if (ImmunityTimer > 2f)
            {
                _immunityBubble.PlayIdleAnimation();
            }

            if (ImmunityTimer <= 2.5f)
            {
                _immunityBubble.PlayFlashAnimation();
            }

            if (ImmunityTimer <= 0.8f)
            {
                _immunityBubble.PlayFastFlashAnimation();
            }
        }

        private void HandleOnHitImmunityFlash(float delta)
        {
            if (!IsOnHitImmune)
            {
                _spritePivot.Visible = true;
                _onHitImmunityFlashCooldown = 0f;
                _isFlashing = false;
                return;
            }

            _onHitImmunityFlashCooldown -= delta;

            if (_onHitImmunityFlashCooldown <= 0)
            {
                _onHitImmunityFlashCooldown = _onHitImmunityFlashRate;
                _isFlashing = !_isFlashing;
            }

            _spritePivot.Visible = _isFlashing;
        }
    }
}