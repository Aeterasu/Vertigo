using Godot;
using System;

namespace Vertigo
{
    public class JumpRopeSmoothPath : SmoothPath
    {
        [Export] private float _positionY = 64f;
        [Export] private float _xOffset = 16f;
        [Export] private float _yOffset = 16f;
        [Export] private float _animationInDuration = 0.2f;
        [Export] private float _animationOutDuraion = 0.6f;

        public Vector2 _point1 = Vector2.Zero;
        public Vector2 _point2 = Vector2.Zero;

        private Tween _tween = new Tween();

        public override void _Ready()
        {
            base._Ready();

            Modulate = new Color(1, 1, 1, 1);

            _point1 = new Vector2(120, _positionY);
            _point2 = new Vector2(360, _positionY);

            Curve = new Curve2D();
            Curve.ClearPoints();
            Curve.AddPoint(new Vector2(-32, _positionY));
            Curve.AddPoint(_point1);
            Curve.AddPoint(_point2);
            Curve.AddPoint(new Vector2(512, _positionY));

            AddChild(_tween);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            Curve.SetPointPosition(1, _point1);
            Curve.SetPointPosition(2, _point2);

            Update();
        }

        public void Animate(float positionX, float direction)
        {
            Vector2 point1Target = new Vector2(positionX - _xOffset, _positionY + _yOffset * direction);
            Vector2 point2Target = new Vector2(positionX + _xOffset, _positionY + _yOffset * direction);

            _tween.InterpolateProperty(this, "_point1", _point1, point1Target, _animationInDuration, Tween.TransitionType.Back, Tween.EaseType.Out, 0f);
            _tween.InterpolateProperty(this, "_point1", point1Target, new Vector2(positionX, _positionY), _animationOutDuraion, Tween.TransitionType.Back, Tween.EaseType.Out, _animationInDuration);

            _tween.InterpolateProperty(this, "_point2", _point2, point2Target, _animationInDuration, Tween.TransitionType.Back, Tween.EaseType.Out, 0f);
            _tween.InterpolateProperty(this, "_point2", point2Target, new Vector2(positionX, _positionY), _animationOutDuraion, Tween.TransitionType.Back, Tween.EaseType.Out, _animationInDuration);

            _tween.Start();
        }
    }
}
