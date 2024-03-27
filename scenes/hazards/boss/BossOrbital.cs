using Godot;
using System;

namespace Vertigo
{
    public class BossOrbital : Sprite
    {
        [Export] private readonly float _xCoeff = 64.0f;
        [Export] private readonly float _yCoeff = 48.0f;
        [Export] private readonly float _speed = 2.0f;

        private Vector2 _defaultPosition = new Vector2();

        private float _ticks = 0.0f;

        public override void _Ready()
        {
            base._Ready();

            _defaultPosition = Position;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _ticks += delta * _speed;

            Position = _defaultPosition + new Vector2(Mathf.Cos(_ticks) * _xCoeff, Mathf.Sin(_ticks) * _yCoeff);

            if (Mathf.Sin(_ticks) > 0.0f)
            {
                ZIndex = 8;
            }
            else
            {
                ZIndex = 7;
            }
        }
    }
}