using Godot;
using System;

namespace Vertigo
{
    public class TimeManager : Node
    {
        [Export] private readonly float _difficultyStepThreshold = 10f;
        [Export] private readonly float _rainThreshold = 25f;
        [Export] private readonly float _dizzynessThreshold = 45f;
        [Export] private readonly float _spiralDizzynessThreshold = 90f;
        [Export] private readonly float _waveDizzynessThreshold = 120f;

        private float _timer = 0f; 
        private float _persistentTimer = 0f;

        private CPUParticles2D _rain;
        private bool _rainActivated = false;

        private DrunkNode2D _dizzyness;
        private bool _dizzynessActivated = false;

        private SpiralDrunkNode2D _spiralDizzyness;
        private bool _spiralDizzynessActivated = false;

        private WaveScreenDistortion _waveDizzyness;
        private bool _waveDizzynessActivated = false;

        public event Action OnDifficultyStepReached;

        public float GetTimer { get => _timer; }

        public float GetDifficultyStepThreshold
        {
            get => _difficultyStepThreshold;
        }

        public float GetPersistentTimer
        {
            get => _persistentTimer;
        }

        public float GetSecElapsed { get => _persistentTimer; }

        public bool IsTimerStopped { get; private set; } = true;

        public static TimeManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            _rain = GetNode<CPUParticles2D>("%Rain");

            _dizzyness = GetNode<DrunkNode2D>("%DrunkNode2D");
            _spiralDizzyness = GetNode<SpiralDrunkNode2D>("%SpiralDrunkNode2D");
            _waveDizzyness = GetNode<WaveScreenDistortion>("%WaveScreenDistortion");

            StartTimer();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!IsTimerStopped)
            {
                _timer += delta;
                _persistentTimer += delta;
            }
            else
            {

            }

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

            if (_persistentTimer >= _waveDizzynessThreshold && !_waveDizzynessActivated)
            {
                _waveDizzyness.Enabled = true;
                _waveDizzynessActivated = true;
            }
        }

        public void StartTimer()
        {
            IsTimerStopped = false;
        }

        public void StopTimer()
        {
            IsTimerStopped = true;
        }
    }
}
