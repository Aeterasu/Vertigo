using Godot;
using System;

namespace Vertigo
{
    public class PowerComponent : Node
    {
        [Export] private float _maxPower = 4f;
        [Export] private float _powerRegen = 1f;
        [Export] private readonly float _exhaustionRegenMultiplier = 0.6f; // triggers when power <1f
        [Export] private readonly float _nearExhaustionRegenMultiplier = 0.8f; // triggers when power >=1f <=2f

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

        public float CurrentPower
        {
            get => _currentPower;
            set
            {
                _currentPower = Mathf.Clamp(value, 0f, MaxPower);
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _currentPower = _maxPower;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            
            float regenMultiplier = 1f;

            if (CurrentPower < 1f)
            {
                regenMultiplier = _exhaustionRegenMultiplier;
            }
            else if (CurrentPower >= 1f && CurrentPower <= 2f)
            {
                regenMultiplier = _nearExhaustionRegenMultiplier;
            }

            CurrentPower += PowerRegen * regenMultiplier * delta;

            GD.Print(regenMultiplier);
        }

        public void ConsumePower(float amount)
        {
            CurrentPower -= amount;
        }
    }
}
