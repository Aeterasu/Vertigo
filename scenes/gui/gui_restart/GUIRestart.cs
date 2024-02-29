using Godot;
using System;

namespace Vertigo
{
    public class GUIRestart : Control
    {
        private Label _label;

        public TextureProgress TextureProgress { get; private set; }

        public ColorRect ColorFlash { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            TextureProgress = GetNode<TextureProgress>("TextureProgress");

            ColorFlash = GetNode<ColorRect>("ColorFlash");

            _label = GetNode<Label>("Label");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _label.Text = Tr("GUI_RESTART");
        }
    }
}