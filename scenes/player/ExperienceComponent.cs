using Godot;
using System;

namespace Vertigo
{
    public class ExperienceComponent : Node
    {
        [Export] private float _baseTargetExp = 12f; // base exp required to lvl up from lvl 1 to 2
        [Export] private float _targetExpCoeff = 1.5f; // by how much exp requirement increases with each level

        private float _currentExp = 0f;
        private float _targetExp = 0f;

        private int _level = 1;

        public event Action<ExperienceComponent> OnExperienceGranted;
        public event Action<ExperienceComponent> OnLevelUp;

        public int Level { get => _level; }
        public float CurrentExp { get => _currentExp; }
        public float TargetExp { get => _targetExp; }

        public override void _Ready()
        {
            base._Ready();

            _targetExp = _baseTargetExp;
        }

        public void GrantExp(float amount)
        {
            _currentExp += amount;

            OnExperienceGranted?.Invoke(this);

            if (_currentExp >= _targetExp)
            {
                LevelUp();
            }
        }

        public void LevelUp()
        {
            _level += 1;

            _currentExp = Mathf.Max(_currentExp - _targetExp, 0f);
            _targetExp *= _targetExpCoeff;

            OnLevelUp?.Invoke(this);
        }
    }
}
