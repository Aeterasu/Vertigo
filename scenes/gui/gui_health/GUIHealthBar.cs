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
        [Export] private float _offset = 14f;
        [Export] private float _bigHeartOffset = 22f;
        [Export] private int _textThreshold = 6;

        private int _currentHp = 0;

        private float _currentOffset = 0;

        private List<Control> _hearts = new List<Control>();

        private Label _label;

        private Player _player;
        private HealthComponent _healthComponent;

        public override void _Ready()
        {
            base._Ready();

            _label = GetNode<Label>("Label");

            _player = GetNodeOrNull<Player>("%Player");

            if (_player.IsValid())
            {
                _healthComponent = _player.GetNodeOrNull<HealthComponent>("HealthComponent");
            }
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
                _label.RectPosition = Vector2.Right * _currentOffset + Vector2.Right * 2 + Vector2.Down * 2;
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
        }
    }
}