using Godot;
using System;

namespace Vertigo
{
    public class ScrollingText : Control
    {
        [Export] private readonly float _speed = 4f;

        private Control _pivot;

        public override void _Ready()
        {
            base._Ready();

            _pivot = GetNode<Control>("Pivot");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _pivot.RectPosition += Vector2.Right * _speed;

            if (_pivot.RectPosition.x >= 63 || _pivot.RectPosition.x <= -63)
            {
                _pivot.RectPosition = new Vector2(0f, _pivot.RectPosition.y);
            }
        }
    }
}