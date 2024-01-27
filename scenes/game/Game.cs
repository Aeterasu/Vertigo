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
        [Export] public PackedScene DiamondScene { get; private set; }
        [Export] public PackedScene ConvergenceProjectileScene { get; private set; }

        private Control _gui;
        private Countdown _countdown;

        private TimeManager _timeManager;

        public float RopeTopPosition { get; private set; }
        public float RopeBottomPosition { get; private set; }
        
        public CanvasLayer GameLayer { get; private set; }

        public ScreenShakeCamera2D ScreenShakeCamera2D { get; private set; }

        public GameState State { get; private set; } = GameState.INTRO;

        public static Game Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            GameLayer = GetNode<CanvasLayer>("%GameLayer");

            ScreenShakeCamera2D = GetNodeOrNull<ScreenShakeCamera2D>("%ScreenShakeCamera2D");

            var topShape = GetNode<JumpRope>("%JumpRopeTop").GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;

            RopeTopPosition = GetNode<JumpRope>("%JumpRopeTop").GlobalPosition.y + topShape.Extents.y * 2;
            RopeBottomPosition = GetNode<JumpRope>("%JumpRopeBottom").GlobalPosition.y;

            _timeManager = GetNode<TimeManager>("%TimeManager");

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
        }

        private void OnCountdownFinished()
        {
            State = GameState.GAME;
            _gui.Visible = true;
            _timeManager.StartTimer();
        }
    }
}
