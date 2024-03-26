using Godot;
using System;

namespace Vertigo
{
    public class Boss : Square
    {
        [Export] private readonly float _lerpWeight = 3f;
        [Export] private readonly float _sinWaveSpeed = 2.0f;
        [Export] private readonly float _sinWaveStrength = 16.0f;

        private float _sinWaveTicks = 0.0f;

        private Vector2 _offset = new Vector2();

        public Vector2 TargetPosition { get; private set; } = new Vector2();
        public Vector2 CurrentPosition { get; private set; } = new Vector2();

        public override void _Ready()
        {
            base._Ready();

            uint random = GD.Randi() % 2;

            if (random == 0)
            {
                Direction = 1;
            }
            else
            {
                Direction = -1;
            }

            if (Direction == -1)
            {
                CurrentPosition = new Vector2(-128f, 138f);
                TargetPosition = new Vector2(72f, 138f);
            }
            else
            {
                CurrentPosition = new Vector2(480f + 128f, 138f);
                TargetPosition = new Vector2(480f - 72f, 138f);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _sinWaveTicks += _sinWaveSpeed * delta;

            CurrentPosition = CurrentPosition.LinearInterpolate(TargetPosition, _lerpWeight * delta);

            _offset = Vector2.Up * Mathf.Sin(_sinWaveTicks) * _sinWaveStrength;

            GlobalPosition = CurrentPosition + _offset;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
        }
    }
}