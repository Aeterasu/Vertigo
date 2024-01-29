using Godot;
using System;

namespace Vertigo
{
    public class ImmunityBubble : Sprite
    {
        private bool _isImmune = false;

        private AnimationPlayer _animationPlayer;
        private AnimationPlayer _flashAnimationPlayer;

        private int _animStage = 0;

        public bool IsImmune
        {
            get => _isImmune;
            set
            {
                if (!value && _isImmune)
                {
                    _animationPlayer.Play("Disappear");
                }
                else if (value && !_isImmune)
                {
                    _animationPlayer.Play("Appear");
                }

                _isImmune = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            Visible = false;

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _flashAnimationPlayer = GetNode<AnimationPlayer>("FlashAnimationPlayer");
        }

        public void PlayIdleAnimation()
        {
            _flashAnimationPlayer.Play("Idle");
            _animStage = 0;        
        }

        public void PlayFlashAnimation()
        {
            if (_animStage < 1)
            {
                _flashAnimationPlayer.Stop(true);
                _flashAnimationPlayer.Play("Flash");
                _animStage = 1;
            }
        }

        public void PlayFastFlashAnimation()
        {
            if (_animStage < 2)
            {
                _flashAnimationPlayer.Stop(true);
                _flashAnimationPlayer.Play("FastFlash");
                _animStage = 2;
            }
        }
    }
}