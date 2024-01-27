using Godot;
using System;

namespace Vertigo
{
    public class SquareTelegraph : Node2D
    {
        [Export] public NodePath SquarePath { get; private set; }
        [Export] private float _duration = 0.5f;

        private Node _parent;

        private Vector2 _squareGlobalPosition = Vector2.Zero;

        private AnimationPlayer _animationPlayer;

        public Square Square { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Square = GetNodeOrNull<Square>(SquarePath);
            _squareGlobalPosition = Square.GlobalPosition;
            RemoveChild(Square);

            _parent = GetTree().Root.GetChild(0);

            if (Game.Instance.IsValid())
            {
                if (Game.Instance.GameLayer.IsValid())
                {
                    _parent = Game.Instance.GameLayer;
                }
            }

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
            _animationPlayer.Play("Idle");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!Square.IsValid())
            {
                return;
            }

            _duration = Mathf.Max(_duration - delta, 0f);

            if (_duration <= 0)
            {
                Square.GlobalPosition = _squareGlobalPosition;
                _parent.CallDeferred("add_child", Square);
                this.SafeQueueFree();
            }
        }
    }
}