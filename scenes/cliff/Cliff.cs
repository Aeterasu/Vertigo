using Godot;
using System;

namespace Vertigo
{
    public class Cliff : Node2D
    {
        private Tween _tween = new Tween();

        public override void _Ready()
        {
            base._Ready();

            AddChild(_tween);

            GetNode<AnimationPlayer>("%AnimationPlayer").Play("Idle");

            Camera2D _camera = GetNode<Camera2D>("Camera2D");

            _tween.InterpolateProperty(_camera, "position", new Vector2(-128f, 0f), Vector2.Zero, 2f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            _tween.Start();
        }
    }
}