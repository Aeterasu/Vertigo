using Godot;
using System;

namespace Vertigo
{
    public class GUIHeart : Control
    {
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Play("Appear");

            _animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");
        }

        public void Delete()
        {
            _animationPlayer.Play("Deletion");
        }

        private void OnAnimationFinished(string animation)
        {
            if (animation == "Deletion")
            {
                this.SafeQueueFree();
            }
        }
    }
}