using Godot;
using System;

namespace Vertigo
{
    public class SquareDestroyedFX : Node2D
    {
        public Sprite SquareSprite;

        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            base._Ready();

            SquareSprite = GetNode<Sprite>("SquareSprite");

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }

        public void Animate()
        {
            _animationPlayer.Play("Destroy");
        }
    }
}