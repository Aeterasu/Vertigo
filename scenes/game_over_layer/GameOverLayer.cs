using Godot;
using System;

namespace Vertigo
{
    public class GameOverLayer : CanvasLayer
    {
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            base._Ready();

            Visible = false;

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }

        public void Animate(Player player)
        {
            Visible = true;

            _animationPlayer.Play("GameOver");

            GetNode<Sprite>("Sprite").GlobalTransform = player.GetNode<Sprite>("SpritePivot/Sprite").GlobalTransform;
        }
    }
}