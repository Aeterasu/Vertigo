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

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM) || Input.IsActionJustPressed(InputActions.INPUT_MENU_CONFIRM))
            {
                var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

                if (animationPlayer.CurrentAnimationPosition < 2.5f)
                {
                    animationPlayer.Stop();
                    animationPlayer.Play("Boot");
                    animationPlayer.Advance(2.5f);
                }
            }
        }

        public void NextState()
        {
            Main.Instance.State = _targetState;
        }
    }
}