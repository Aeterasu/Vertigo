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
        [Export] private readonly float _gradientTargetAlpha = 0.65f;
        [Export] private readonly float _gradientAlphaChangeSpeed = 0.5f;

        private UpgradeCard _upgradeCard;

        private bool _isFocused = false;

        private Sprite _sprite;
        private Tween _tween = new Tween();

        private Sprite _gradient;
        private float _gradientAlpha = 0f;

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

        public int Stack { get; set; } = 1;

        public override void _Ready()
        {
            base._Ready();

            _sprite = GetNode<Sprite>("Sprite");
            _gradient = GetNode<Sprite>("Gradient");

            AddChild(_tween);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (IsFocused)
            {
                _gradientAlpha = Mathf.Min(_gradientAlpha + _gradientAlphaChangeSpeed * delta, _gradientTargetAlpha);
            }
            else
            {
                _gradientAlpha = Mathf.Max(_gradientAlpha - _gradientAlphaChangeSpeed * delta, 0f);
            }

            _gradient.Modulate = new Color(1f, 1f, 1f, _gradientAlpha);
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