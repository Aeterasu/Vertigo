using Godot;
using System;

namespace Vertigo
{
    public class GrazeDetector : Area2D
    {
        [Export] private readonly int _grazeTickRate = 2;
        [Export] private readonly float _lingeringGrazeDuration = 0.3f;

        private int _tick = 0;

        private float _lingeringGrazeTimeLeft = 0f;

        public bool IsGrazing { get; private set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _tick = _grazeTickRate;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _lingeringGrazeTimeLeft = Mathf.Max(_lingeringGrazeTimeLeft - delta, 0f);

            _tick -= 1;

            if (_tick <= 0)
            {
                UpdateGraze();
            }

            IsGrazing = _lingeringGrazeTimeLeft > 0f;
        }

        private void UpdateGraze()
        {
            IsGrazing = false;

            foreach (Area2D area in GetOverlappingAreas())
            {
                if (area is GrazeArea)
                {
                    _lingeringGrazeTimeLeft = _lingeringGrazeDuration;
                    return;
                }
            }
        }
    }
}