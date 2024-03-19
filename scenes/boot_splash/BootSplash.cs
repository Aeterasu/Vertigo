using Godot;
using System;

namespace Vertigo
{
    public class BootSplash : Node
    {
        [Export] private readonly MainState _targetState = MainState.INTRO;

        public override void _Ready()
        {
            base._Ready();

            GetNode<AnimationPlayer>("AnimationPlayer").Play("Boot");
        }

        public void NextState()
        {
            Main.Instance.State = _targetState;
        }
    }
}