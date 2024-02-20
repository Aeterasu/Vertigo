using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class RandomBackdrop : Sprite
    {
        [Export] private List<Texture> _textures = new List<Texture>();

        public override void _Ready()
        {
            base._Ready();

            Random random = new Random();
            int r = random.Next(_textures.Count);

            Texture = _textures[r];
        }
    }
}