using Godot;
using System;

namespace Vertigo
{
    public class Player : Area2D
    {
        [Export] private readonly float _gravity = 8f;
        [Export] private readonly float _horizontalSpeed = 4f;

        private GravityMode _gravityMode = GravityMode.DOWNWARDS;

        private float _velocityX = 0f;
        private float _velocityY = 0f;

        public override void _Ready()
        {
            base._Ready();

            Connect("area_entered", this, "OnAreaEntered");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            HandleInput();

            _velocityY += _gravity * (int)_gravityMode * delta;

            Position += new Vector2(_velocityX, _velocityY);
        }

        private void HandleInput()
        {
            _velocityX = 0f;

            if (Input.IsActionPressed("move_left"))
            {
                _velocityX = -_horizontalSpeed;
            }

            if (Input.IsActionPressed("move_right"))
            {
                _velocityX = _horizontalSpeed;
            }
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is JumpRope)
            {
                var jumpRope = area as JumpRope;

                if (jumpRope.GravityMode != _gravityMode)
                {
                    _gravityMode = jumpRope.GravityMode;
                    _velocityY = 0f;
                }
            }
        }
    }
}