using Godot;
using System;

namespace Vertigo
{
    public class UpgradeButton : Control
    {
        [Export] private readonly int _id = 0;
        [Export] private readonly float _focusAnimationDuration = 0.2f;
        [Export] private readonly float _focusAnimationOffset = 12f;
        [Export] private readonly float _unfocusAnimationDuration = 0.1f;
        [Export] private readonly float _confirmAnimationDuration = 0.15f;
        [Export] private readonly float _confirmAnimationOffset = 16f;
        [Export] private readonly float _discardAnimationDuration = 0.15f;
        [Export] private readonly float _discardAnimationOffset = -16f;

        private UpgradeCard _upgradeCard;

        private bool _isFocused = false;

        private Sprite _sprite;
        private Tween _tween = new Tween();

        public int Id
        {
            get => _id;
        }

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (value && !_isFocused)
                {
                    Focus();
                }
                else if (!value && _isFocused)
                {
                    Unfocus();
                }

                _isFocused = value;
            }
        }

        public UpgradeCard UpgradeCard
        {
            get => _upgradeCard;
            set
            {
                _upgradeCard = value;     
                
                if (_sprite.IsValid())
                {
                    _sprite.Frame = value.TextureAtlasId;
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _sprite = GetNode<Sprite>("Sprite");

            AddChild(_tween);
        }

        public void Focus()
        {
            _tween.InterpolateProperty(_sprite, "position", Vector2.Zero, Vector2.Up * _focusAnimationOffset, _focusAnimationDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();
        }

        public void Unfocus()
        {
            _tween.InterpolateProperty(_sprite, "position", _sprite.Position, Vector2.Zero, _unfocusAnimationDuration, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
            _tween.Start();
        }

        public void Confirm()
        {
            _tween.InterpolateProperty(_sprite, "position", _sprite.Position, Vector2.Up * _confirmAnimationOffset, _confirmAnimationDuration, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();            
        }

        public void Discard()
        {
            _tween.InterpolateProperty(_sprite, "position", _sprite.Position, Vector2.Up * _discardAnimationOffset, _discardAnimationDuration, Tween.TransitionType.Linear);
            _tween.InterpolateProperty(_sprite, "modulate", new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), _discardAnimationDuration, Tween.TransitionType.Linear);
            _tween.Start();            
        }
    }
}