using Godot;
using System;

namespace Vertigo
{
    public class Interlude : Control
    {
        private AnimationPlayer _animationPlayer;

        public event Action OnInterludeAnimationFinished;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
            _animationPlayer.Play("Interlude");
            _animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM))
            {
                _animationPlayer.Play("Skip");
            }
        }

        private void OnAnimationFinished(string animName)
        {
            OnInterludeAnimationFinished?.Invoke();
        }
    }
}