using Godot;
using System;

namespace Vertigo
{
    public class CreditsButton : MenuButton
    {
        [Export] private readonly string _creditsTextKey = "";
        [Export] public string CreditsLink { get; private set; } = "";

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            GetLabel.Text = Tr(_creditsTextKey);
        }
    }
}