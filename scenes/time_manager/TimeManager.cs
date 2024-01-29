using Godot;
using System;
using System.Diagnostics;

namespace Vertigo
{
    public class TimeManager : Node
    {
        [Export] private readonly float _difficultyStepThreshold = 10f;
        [Export] private readonly float _rainThreshold = 25f;
        [Export] private readonly float _dizzynessThreshold = 45f;
        [Export] private readonly float _spiralDizzynessThreshold = 90f;

        private float _timer = 0f; 
        private float _persistentTimer = 0f;

        private CPUParticles2D _rain;
        private bool _rainActivated = false;

        private DrunkNode2D _dizzyness;
        private bool _dizzynessActivated = false;

        private SpiralDrunkNode2D _spiralDizzyness;
        private bool _spiralDizzynessActivated = false;

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

            _rain = GetNode<CPUParticles2D>("%Rain");

            _dizzyness = GetNode<DrunkNode2D>("%DrunkNode2D");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _timer += delta;
            _persistentTimer += delta;

            if (_timer >= _difficultyStepThreshold)
            {
                OnDifficultyStepReached?.Invoke();
                _timer = 0f;
            }

            if (_persistentTimer >= _rainThreshold && !_rainActivated)
            {
                _rain.Emitting = true;
                _rainActivated = true;
            }

            if (_persistentTimer >= _dizzynessThreshold && !_dizzynessActivated)
            {
                _dizzyness.Enabled = true;
                _dizzynessActivated = true;
            }

            if (_persistentTimer >= _spiralDizzynessThreshold && !_spiralDizzynessActivated)
            {
                _spiralDizzyness.Enabled = true;
                _spiralDizzynessActivated = true;
            }
        }

        public void StartTimer()
        {
            Stopwatch.Start();
        }

        public void StopTimer()
        {
            Stopwatch.Stop();
        }
    }
}
