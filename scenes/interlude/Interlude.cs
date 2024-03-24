using Godot;
using System;

namespace Vertigo
{
    public class Interlude : Control
    {
        private Label _label1;
        private Label _label2;

        private float _delay = 0.1f;

        private bool _isSkipped = false;

        private AnimationPlayer _animationPlayer;

        public event Action OnInterludeAnimationFinished;

        public override void _Ready()
        {
            base._Ready();

            _label1 = GetNode<Label>("%Label1");
            _label2 = GetNode<Label>("%Label2");

            _label1.Text = Tr(ColorManager.GetCurrentColor.InterludeTextBeginningKey);
            _label2.Text = Tr(ColorManager.GetCurrentColor.InterludeTextEndingKey);

            _label1.GetNode<Label>("GhostLabel1").Text = _label1.Text;
            _label2.GetNode<Label>("GhostLabel2").Text = _label2.Text;

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
            _animationPlayer.Play("Interlude");
            _animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _delay = Mathf.Max(_delay - delta, 0f);

            if ((Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM) || Input.IsActionJustPressed(InputActions.INPUT_MENU_CONFIRM)) && _delay <= 0.0f)
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