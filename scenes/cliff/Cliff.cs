using Godot;
using System;

namespace Vertigo
{
    public class Cliff : Node2D
    {
        private Camera2D _camera;

        private readonly Tween _tween = new Tween();

        public override void _Ready()
        {
            base._Ready();

            AddChild(_tween);

            GetNode<AnimationPlayer>("%AnimationPlayer").Play("Idle");

            _camera = GetNode<Camera2D>("Camera2D");
        }

        public void Animate()
        {
            _tween.InterpolateProperty(_camera, "position", new Vector2(-520f, 0f), Vector2.Zero, 2f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            _tween.Start();
        }

        public void QuickAnimate()
        {
            _tween.InterpolateProperty(_camera, "position", new Vector2(-128f, 0f), Vector2.Zero, 0.5f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            _tween.Start();
        }
    }
}