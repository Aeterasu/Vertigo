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

        private float _difficultyStepTimer = 0f; 
        private float _timer = 0f;

        private CPUParticles2D _rain;
        private bool _rainActivated = false;

        private DrunkNode2D _dizzyness;
        private bool _dizzynessActivated = false;

        private SpiralDrunkNode2D _spiralDizzyness;
        private bool _spiralDizzynessActivated = false;

        private WaveScreenDistortion _waveDizzyness;
        private bool _waveDizzynessActivated = false;

        public event Action OnDifficultyStepReached;

        public float GetDifficultyStepThreshold
        {
            get => _difficultyStepThreshold;
        }

        public float GetElapsedTime
        {
            get => _timer;
        }

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
                _difficultyStepTimer += delta;
                _timer += delta;
            }
            else
            {

            }

            if (_difficultyStepTimer >= _difficultyStepThreshold && !Game.Instance.IsBossSpawned)
            {
                OnDifficultyStepReached?.Invoke();
                _difficultyStepTimer = 0f;
            }

            if (_timer >= _rainThreshold && !_rainActivated)
            {
                _rain.Emitting = true;
                _rainActivated = true;
            }

            if (_timer >= _dizzynessThreshold && !_dizzynessActivated)
            {
                _dizzyness.Enabled = true;
                _dizzynessActivated = true;
            }

            if (_timer >= _spiralDizzynessThreshold && !_spiralDizzynessActivated)
            {
                _spiralDizzyness.Enabled = true;
                _spiralDizzynessActivated = true;
            }

            if (_timer >= _waveDizzynessThreshold && !_waveDizzynessActivated)
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
