using Godot;
using System;
using System.Diagnostics;

namespace Vertigo
{
    public class TimeManager : Node
    {
        [Export] private readonly float _difficultyStepThreshold = 7.5f;
        [Export] private readonly float _upgradeStepThreshold = 15f;
        [Export] private readonly bool _addDifficultyOnUpgrade = true;

        private float _timer = 0f; 

        private bool _difficultyStepLock = false;
        private bool _upgradeStepLock = false;

        public event Action OnDifficultyStepReached;
        public event Action OnUpgradeStepReached;

        public float GetTimer { get => _timer; }
        public float GetMaxTimer
        {
            get
            {
                return _upgradeStepThreshold > _difficultyStepThreshold ? _upgradeStepThreshold : _difficultyStepThreshold;
            }
        }

        public float GetMinTimer
        {
            get
            {
                return _upgradeStepThreshold > _difficultyStepThreshold ? _difficultyStepThreshold : _upgradeStepThreshold;
            }
        }

        public float GetDifficultyStepThreshold
        {
            get => _difficultyStepThreshold;
        }

        public float GetUpgradeStepThreshold
        {
            get => _upgradeStepThreshold;
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

            if (_timer >= _difficultyStepThreshold && !_difficultyStepLock)
            {
                OnDifficultyStepReached?.Invoke();
                _difficultyStepLock = true;
            }

            if (_timer >= _upgradeStepThreshold && !_upgradeStepLock)
            {
                OnUpgradeStepReached?.Invoke();
                _upgradeStepLock = true;

                if (_addDifficultyOnUpgrade)
                {
                    OnDifficultyStepReached?.Invoke();
                }
            }

            if (_timer >= GetMaxTimer)
            {
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            _timer = 0f;
            _difficultyStepLock = false;
            _upgradeStepLock = false;
        }
    }
}
