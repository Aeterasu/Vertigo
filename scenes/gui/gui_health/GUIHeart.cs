using Godot;
using System;

namespace Vertigo
{
    public class GUIHeart : Control
    {
        [Export] private readonly Texture _passionTexture;

        private Sprite _sprite;

        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Play("Appear");

            _sprite = GetNode<Sprite>("Sprite");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (_passionTexture != null && Passion.Instance.IsValid() && _sprite.Texture != _passionTexture)
            {
                _sprite.Texture = _passionTexture;
            }
        }

        public void Delete()
        {
            _animationPlayer.Play("Deletion");
        }

        public void OnDeletionFinished()
        {
            this.SafeQueueFree();
        }

        public void PlayHealSound()
        {
            Game.Instance.PlayHealSound();
        }

        public void PlayHeartbreakSound()
        {
            Game.Instance.PlayHeartbreakSound();
        }
    }
}