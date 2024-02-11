using Godot;
using System;

namespace Vertigo
{
    public enum GameState
    {
        INTRO,
        GAME,
        UPGRADE_MENU,
        POST_MORTEM,
    }

    public class Game : Node2D
    {
        [Export] public PackedScene UpgradeMenuScene { get; private set; }
        [Export] public PackedScene DiamondScene { get; private set; }
        [Export] public PackedScene ConvergenceProjectileScene { get; private set; }

        [Export] public float OnHitSlowMotionRatio { get; private set; } = 0.2f;
        [Export] public float OnHitSlowMotionDuration { get; private set; } = 1f;

        [Export] public AudioStream GemPickupSound { get; private set; }

        private Player _player;
        private ExperienceComponent _playerExperienceComponent;

        private Control _gui;
        private Countdown _countdown;

        private TimeManager _timeManager;

        private UpgradeManager _upgradeManager;

        public float OnHitSlowMotionTimeLeft { get; private set; } = 0f;
        public float CurrentOnHitSlowMotionRatio { get; private set; } = 1f;
        public bool IsInOnHitSlowMotion { get; private set; } = false;

        public float RopeTopPosition { get; private set; }
        public float RopeBottomPosition { get; private set; }
        
        public CanvasLayer GameLayer { get; private set; }
        public CanvasLayer GUILayer { get; private set; }

        public ScreenShakeCamera2D ScreenShakeCamera2D { get; private set; }

        public GameState State { get; private set; } = GameState.INTRO;

        public static Game Instance { get; set; }

        public override void _Ready()
        {
            base._Ready();

            GameLayer = GetNode<CanvasLayer>("%GameLayer");
            GUILayer = GetNode<CanvasLayer>("%GUILayer");

            _player = GetNode<Player>("%Player");
            _playerExperienceComponent = _player.GetNode<ExperienceComponent>("%ExperienceComponent");
            _playerExperienceComponent.OnLevelUp += OnPlayerLevelUp;
            _player.OnPlayerHit += OnPlayerHit;

            ScreenShakeCamera2D = GetNodeOrNull<ScreenShakeCamera2D>("%ScreenShakeCamera2D");

            var topShape = GetNode<JumpRope>("%JumpRopeTop").GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;

            RopeTopPosition = GetNode<JumpRope>("%JumpRopeTop").GlobalPosition.y + topShape.Extents.y * 2;
            RopeBottomPosition = GetNode<JumpRope>("%JumpRopeBottom").GlobalPosition.y;

            _timeManager = GetNode<TimeManager>("%TimeManager");

            _upgradeManager = GetNode<UpgradeManager>("%UpgradeManager");

            _gui = GetNode<Control>("%GUI");
            _gui.Visible = false;

            _countdown = GetNode<Countdown>("%Countdown");

            if (_countdown.IsValid())
            {
                _countdown.OnCountdownFinished += OnCountdownFinished;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            HandleOnHitSlowMotion(delta);

            Engine.TimeScale = State == GameState.GAME ? CurrentOnHitSlowMotionRatio : 1f;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            _gui.Visible = State == GameState.GAME;
        }

        public void SetStateToGame()
        {
            State = GameState.GAME;
            _timeManager.StartTimer();            
        }

        private void OnCountdownFinished()
        {
            SetStateToGame();
        }

        private void HandleOnHitSlowMotion(float delta)
        {
            OnHitSlowMotionTimeLeft = Mathf.Max(OnHitSlowMotionTimeLeft - (delta / Engine.TimeScale), 0f);

            IsInOnHitSlowMotion = OnHitSlowMotionTimeLeft > 0f;

            if (IsInOnHitSlowMotion)
            {
                CurrentOnHitSlowMotionRatio = Mathf.Lerp(CurrentOnHitSlowMotionRatio, OnHitSlowMotionRatio, 5 * (delta / Engine.TimeScale));
            }
            else
            {
                CurrentOnHitSlowMotionRatio = Mathf.Lerp(CurrentOnHitSlowMotionRatio, 1f, 5 * (delta / Engine.TimeScale));
            }
        }

        private void OnPlayerHit()
        {
            OnHitSlowMotionTimeLeft = OnHitSlowMotionDuration;
        }

        private void OnPlayerLevelUp(ExperienceComponent exp)
        {
            if (UpgradeMenuScene == null)
            {
                return;
            }

            State = GameState.UPGRADE_MENU;
            _timeManager.StopTimer();

            var node = UpgradeMenuScene.Instance();

            if (!(node is UpgradeMenu))
            {
                node.SafeQueueFree();
                SetStateToGame();
                return;
            }
            
            UpgradeMenu upgradeMenu = node as UpgradeMenu;
            upgradeMenu.OnMenuClosed += OnUpgradeMenuClosed;
            upgradeMenu.OnUpgradeSelected += OnUpgradeSelected;
            GUILayer.AddChild(upgradeMenu);
        }

        private void OnUpgradeSelected(UpgradeCard upgradeCard)
        {
            _upgradeManager.AddUpgrade(upgradeCard);
        }

        private void OnUpgradeMenuClosed()
        {
            SetStateToGame();
        }
    }
}
