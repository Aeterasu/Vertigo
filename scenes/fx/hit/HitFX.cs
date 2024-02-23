using Godot;
using System;

namespace Vertigo
{
    public class HitFX : Sprite
    {
        [Export] private float _rotationSpeed = 360f;
        [Export] private float _scaleSpeed = 1.5f;

        private float _direction = 1.0f;

        private float _currentScale = 1.0f;

        private bool _animated = false;

        public override void _Ready()
        {
            base._Ready();

            _currentScale = 0.0f;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            RotationDegrees += _rotationSpeed * delta;

            _currentScale = Mathf.Max(_currentScale - _scaleSpeed * delta, 0f);

            Scale = Vector2.One * _currentScale;

            if (_currentScale <= 0 && _animated)
            {
                QueueFree();
            }
        }

        public void Animate()
        {
            _direction = (float)(1 - (2 * GD.RandRange(0, 1)));
            _currentScale = 1.0f;
            _animated = true;
        }
    }
}