using Godot;
using System;

namespace Vertigo
{
    public class Square : Hazard
    {
        [Export] private float _speed = 16f;
        [Export] private float _rotationSpeed = 32f;

        private float _direction = 1f;

        private Sprite _sprite;

        public float Direction
        {
            get => _direction;
            set
            {
                if (value < -1)
                {
                    value = -1;
                }

                if (value > 1)
                {
                    value = 1;
                }

                _direction = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _sprite = GetNode<Sprite>("%Sprite");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _sprite.RotationDegrees += _rotationSpeed * delta;

            Position += Vector2.Right * _speed * Direction * delta;
        }
    }
}
