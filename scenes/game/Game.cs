using Godot;
using System;

namespace Vertigo
{
    public class Game : Node2D
    {
        [Export] public PackedScene DiamondScene { get; private set; }
        [Export] public PackedScene ConvergenceProjectileScene { get; private set; }

        public float RopeTopPosition { get; private set; }
        public float RopeBottomPosition { get; private set; }
        
        public CanvasLayer GameLayer { get; private set; }

        public ScreenShakeCamera2D ScreenShakeCamera2D { get; private set; }

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
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
        }
    }
}
