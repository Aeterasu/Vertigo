using Godot;
using System;
using System.Diagnostics;

namespace Vertigo
{
    public class TimeManager : Node
    {
        [Export] private readonly float _difficultyStepThreshold = 10f;

        private float _timer = 0f; 

        public event Action OnDifficultyStepReached;

        public float GetTimer { get => _timer; }

        public float GetDifficultyStepThreshold
        {
            get => _difficultyStepThreshold;
        }

        public Stopwatch Stopwatch { get; private set; } = new Stopwatch();

        public override void _Ready()
        {
            base._Ready();

            Stopwatch.Start();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _timer += delta;

            if (_timer >= _difficultyStepThreshold)
            {
                OnDifficultyStepReached?.Invoke();
                _timer = 0f;
            }
        }
    }
}
