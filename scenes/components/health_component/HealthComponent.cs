using Godot;
using System;

namespace Vertigo
{
    public class HealthComponent : Node
    {
        [Export] private int _health = 1;
        [Export] private int _healthLimit = 64;

        public event Action OnHealthDepleted;

        public bool IsDead { get; private set; } = false;

        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, _healthLimit);

                if (value <= 0 && !IsDead)
                {
                    OnHealthDepleted?.Invoke();
                    IsDead = true;
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();
        }

        public void DealDamage(int amount = 1)
        {
            Health -= amount;
        }
    }

}