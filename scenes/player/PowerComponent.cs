using Godot;
using System;

namespace Vertigo
{
    public class PowerComponent : Node
    {
        [Export] private float _maxPower = 4f;
        [Export] private float _powerRegen = 1f;
        [Export] private float _grazeMultiplier = 1.8f;
        [Export] private readonly float _exhaustionRegenMultiplier = 0.6f; // triggers when power <1f
        [Export] private readonly float _nearExhaustionRegenMultiplier = 0.8f; // triggers when power >=1f <=2f

        private float _currentRegen = 0f;

        private float _currentPower = 0f;

        public float MaxPower
        {
            get => _maxPower;
            set
            {
                if (value <= 0)
                {
                    value = 1;
                }

                _maxPower = value;
            }
        }

        public float PowerRegen
        {
            get => _powerRegen;
            set
            {
                _powerRegen = value;
            }
        }

        public float CurrentRegen
        {
            get => _currentRegen;
            private set
            {
                _currentRegen = value;
            }
        }

        public float CurrentPower
        {
            get => _currentPower;
            set
            {
                _currentPower = Mathf.Clamp(value, 0f, MaxPower);
            }
        }

        public float HasteRegenSpeedMultiplier { get; set; } = 1.0f;

        public bool IsGrazing { get; set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _currentPower = _maxPower;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (CurrentPower >= MaxPower)
            {
                return;
            }
            
            float regenMultiplier = 1f;

            if (CurrentPower < 1f)
            {
                regenMultiplier = _exhaustionRegenMultiplier;
            }
            else if (CurrentPower >= 1f && CurrentPower <= 2f)
            {
                regenMultiplier = _nearExhaustionRegenMultiplier;
            }

            float grazeMultiplier = IsGrazing ? _grazeMultiplier : 1f;

            _currentRegen += PowerRegen * regenMultiplier * grazeMultiplier * HasteRegenSpeedMultiplier * delta;

            if (_currentRegen >= 1f)
            {
                _currentRegen = 0f;
                CurrentPower += 1f;
            }
        }

        public void ConsumePower(float amount)
        {
            CurrentPower -= amount;
            _currentRegen = 0f;
        }
    }
}
