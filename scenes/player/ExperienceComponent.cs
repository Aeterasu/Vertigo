using Godot;
using System;

namespace Vertigo
{
    public class ExperienceComponent : Node
    {
        [Export] private readonly float _baseTargetExp = 12f; // base exp required to lvl up from lvl 1 to 2
        [Export] private readonly float _targetExpCoeff = 1.5f; // by how much exp requirement increases with each level
        [Export] private readonly float _grazingExpGain = 1f;
        [Export] private readonly float _grazingCumulativeRate = 1.5f;
        [Export] private readonly float _grazingCumulativeStep = 1f;
        [Export] private readonly bool _grantHpOnLevelUp = false;
        [Export(PropertyHint.Range, "0, 32")] private readonly int _hpAmountOnLevelUp = 1;

        private float _currentExp = 0f;
        private float _targetExp = 0f;

        private int _level = 1;
        
        private float _grazingTimer = 0f;
        private float _currentGrazingGain = 0f;

        private Player _player;

        public event Action<ExperienceComponent> OnExperienceGranted;
        public event Action<ExperienceComponent> OnLevelUp;

        public int Level { get => _level; }
        public float CurrentExp { get => _currentExp; }
        public float TargetExp { get => _targetExp; }

        public bool Enabled { get; set; } = true;

        public override void _Ready()
        {
            base._Ready();

            _targetExp = _baseTargetExp;

            _player = GetParentOrNull<Player>();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_player.IsValid())
            {
                if (_player.IsGrazing)
                {
                    GrantExp(_currentGrazingGain * delta);

                    _grazingTimer += delta;

                    if (_grazingTimer >= _grazingCumulativeRate)
                    {
                        _grazingTimer = 0;
                        _currentGrazingGain += _grazingCumulativeStep;
                    }
                }
                else
                {
                    _currentGrazingGain = _grazingExpGain;
                }
            }
        }

        public void GrantExp(float amount)
        {
            if (!Enabled)
            {
                return;
            }

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

            if (_grantHpOnLevelUp)
            {
                _player.GetNode<HealthComponent>("HealthComponent").Health += _hpAmountOnLevelUp;
            }
        }
    }
}
