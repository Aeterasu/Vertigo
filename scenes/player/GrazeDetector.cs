using Godot;
using System;

namespace Vertigo
{
    public class GrazeDetector : Area2D
    {
        [Export] private readonly int _grazeTickRate = 2;

        private int _tick = 0;

        public bool IsGrazing { get; private set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _tick = _grazeTickRate;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _tick -= 1;

            if (_tick <= 0)
            {
                UpdateGraze();
            }
        }

        private void UpdateGraze()
        {
            IsGrazing = false;

            foreach (Area2D area in GetOverlappingAreas())
            {
                if (area is GrazeArea)
                {
                    IsGrazing = true;
                    return;
                }
            }
        }
    }
}