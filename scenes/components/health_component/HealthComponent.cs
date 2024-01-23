using Godot;
using System;

namespace Vertigo
{
    public class HealthComponent : Node
    {
        [Export] private int _maxHealth = 1;

        private int _currentHealth = 1;

        public event Action OnHealthDepleted;

        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                if (value <= 0)
                {
                    value = 1;
                }

                _maxHealth = value;
            }
        }

        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

                if (value <= 0)
                {
                    OnHealthDepleted?.Invoke();
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();

            CurrentHealth = MaxHealth;
        }

        public void DealDamage(int amount = 1)
        {
            CurrentHealth -= amount;
        }
    }

}