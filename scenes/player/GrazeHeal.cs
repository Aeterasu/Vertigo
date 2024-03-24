using Godot;
using System;

namespace Vertigo
{
    public class GrazeHeal : Node
    {
        [Export] private bool _enabled = false;
        [Export] private readonly float _buildUp = 1f;
        [Export] private readonly float _baseRequiredGraze = 1.75f;
        [Export] private readonly float _multiplierOnEachGrantedHp = 1.5f;
        [Export] private readonly float _decay = 0.2f;
        [Export] private readonly bool _resetOnGettingHit = false;

        private int _level = 0;

        private float _currentBuildUp = 0.0f;
        private float _requiredGraze = 1.75f;

        private Player _player;

        public float Percentage { get => _currentBuildUp / _requiredGraze; }

        public bool IsEnabled 
        {
            get => _enabled;
            set
            {
                _enabled = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _player = GetParentOrNull<Player>();

            if (_player.IsValid())
            {
                _player.OnPlayerHit += OnPlayerHit;
            }

            _requiredGraze = _baseRequiredGraze;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_enabled)
            {
                return;
            }

            if (!_player.IsValid())
            {
                return;
            }

            if (_player.IsGrazing)
            {
                _currentBuildUp += _buildUp * delta;
            }
            else
            {
                _currentBuildUp = Mathf.Max(_currentBuildUp - _decay * delta, 0f);
            }

            if (_currentBuildUp >= _requiredGraze)
            {
                GrantHealth();
            }
        }

        private void GrantHealth()
        {
            _player.GetHealthComponent.Health += 1;
            _level += 1;

            _currentBuildUp = 0f;

            _requiredGraze = _baseRequiredGraze * (_level * _multiplierOnEachGrantedHp);
        }

        private void OnPlayerHit()
        {
            if (_resetOnGettingHit)
            {
                _currentBuildUp = 0f;
            }
        }
    }
}