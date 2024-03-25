using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class GUIHealthBar : Control
    {
        [Export] private readonly PackedScene _heartScene;
        [Export] private readonly PackedScene _bigHeartScene;
        [Export] private int _targetHp = 1;
        [Export] private readonly float _offset = 14f;
        [Export] private readonly float _bigHeartOffset = 22f;
        [Export] private readonly int _textThreshold = 6;
        [Export] private readonly float _grazeHealHeartLerpWeight = 8f;
        [Export] private readonly float _onHitGrazeHeartAnimationDelay = 1.5f;
        private float _onHitGrazeHeartAnimationCurrentDelay = 0f;
        private bool _setGrazeHeart = false;
        
        private TextureProgress _grazeBuildup;

        private int _currentHp = 0;

        private float _currentOffset = 0;

        private List<Control> _hearts = new List<Control>();

        private Label _label;

        private Player _player;
        private GrazeHeal _playerGrazeHeal;
        private HealthComponent _healthComponent;

        public override void _Ready()
        {
            base._Ready();

            _label = GetNode<Label>("Label");

            _player = GetNodeOrNull<Player>("%Player");
            _player.OnPlayerHit += OnPlayerHit;

            _grazeBuildup = GetNode<TextureProgress>("GrazeProgress");

            if (_player.IsValid())
            {
                _healthComponent = _player.GetNodeOrNull<HealthComponent>("HealthComponent");
                _playerGrazeHeal = _player.GetNodeOrNull<GrazeHeal>("GrazeHeal");
            }

            _grazeBuildup.Visible = _playerGrazeHeal.IfValid().IsEnabled;

            GetNode<Label>("HealthLabel").Text = Tr("GUI_VITALITY");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            
            if (_healthComponent.IsValid())
            {
                _targetHp = _healthComponent.Health;
            }

            if (_targetHp > _textThreshold)
            {
                _label.Text = $"+{_targetHp - _textThreshold}";
                _label.RectPosition = Vector2.Right * _currentOffset + Vector2.Right * 2 + Vector2.Down * 2 + (_playerGrazeHeal.IfValid().IsEnabled ? Vector2.Right * 16 : Vector2.Zero); // this is the worst possible use of a ternary operator ever
                _label.Visible = true;
            }
            else
            {
                _label.Visible = false;
            }

            if (_currentHp < _targetHp && _currentHp < _textThreshold)
            {
                if (_currentHp > 0)
                {
                    var heart = _heartScene.Instance() as Control;
                    heart.RectPosition = Vector2.Right * _currentOffset;
                    _currentOffset += _offset;
                    _hearts.Add(heart);
                    _currentHp += 1;
                    CallDeferred("add_child", heart);
                }
                else if (_currentHp == 0)
                {
                    var bigHeart = _bigHeartScene.Instance() as Control;
                    bigHeart.RectPosition = Vector2.Right * _currentOffset + Vector2.Up * 6;
                    _currentOffset += _bigHeartOffset;
                    _hearts.Add(bigHeart);
                    _currentHp += 1;
                    CallDeferred("add_child", bigHeart);                    
                }
            }
            else if (_currentHp > _targetHp && _targetHp >= 0 && _targetHp < _textThreshold)
            {
                var heart = _hearts[_currentHp - 1];
                
                if (heart is GUIHeart)
                {
                    (heart as GUIHeart).Delete();
                }
                else
                {
                    heart.SafeQueueFree();
                }

                _hearts.Remove(heart);
                _currentHp -= 1;
                _currentOffset -= _offset;
            }

            _onHitGrazeHeartAnimationCurrentDelay = Mathf.Max(_onHitGrazeHeartAnimationCurrentDelay - delta, 0.0f);

            if (!_setGrazeHeart)
            {
                _grazeBuildup.RectPosition = Vector2.Right * _currentOffset;
                _setGrazeHeart = true;
            }

            if (_onHitGrazeHeartAnimationCurrentDelay <= 0)
            {
                _grazeBuildup.RectPosition = _grazeBuildup.RectPosition.LinearInterpolate(Vector2.Right * _currentOffset, _grazeHealHeartLerpWeight * delta);
            }

            _grazeBuildup.Value = _playerGrazeHeal.Percentage;
        }

        public void ShowGrazeHeart()
        {
            _grazeBuildup.Visible = true;
        }

        private void OnPlayerHit()
        {
            _onHitGrazeHeartAnimationCurrentDelay = _onHitGrazeHeartAnimationDelay;
        }
    }
}