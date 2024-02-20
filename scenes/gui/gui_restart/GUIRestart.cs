using Godot;
using System;

namespace Vertigo
{
    public class GUIRestart : Control
    {
        public TextureProgress TextureProgress { get; private set; }

        public ColorRect ColorFlash { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            TextureProgress = GetNode<TextureProgress>("TextureProgress");

            ColorFlash = GetNode<ColorRect>("ColorFlash");
        }
    }
}