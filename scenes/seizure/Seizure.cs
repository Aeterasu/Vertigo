using Godot;
using System;

namespace Vertigo
{
    public class Seizure : Control
    {
        [Export] private readonly MainState _targetState = MainState.MAIN_MENU;
        [Export] private readonly float _duration = 3.0f;
        [Export] private readonly float _fadeIn = 0.5f;
        [Export] private readonly float _fadeOut = 0.8f;

        private float _ticks = 0.0f;

        private bool _fadeOutLock = false;

        private Tween _tween = new Tween();

        private RichTextLabel _label;

        public override void _Ready()
        {
            base._Ready();

            _ticks = _duration;

            AddChild(_tween);

            _tween.InterpolateProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), _fadeIn);
            _tween.Start();

            _label = GetNode<RichTextLabel>("Label");
            _label.BbcodeText = "[center]" + Tr("SEIZURE_WARNING") + "[/center]";
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM) || Input.IsActionJustPressed(InputActions.INPUT_MENU_CONFIRM))
            {
                if (_ticks > _fadeOut)
                {
                    _ticks = _fadeOut;
                }
            }

            _ticks -= delta;

            if (_ticks <= _fadeOut  && !_fadeOutLock)
            {
                _tween.InterpolateProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 0.0f), _fadeOut);
                _tween.Start();

                _fadeOutLock = true;
            }

            if (!Main.Instance.IsValid())
            {
                return;
            }

            if (_ticks <= 0)
            {
                Main.Instance.State = _targetState;
            }
        }
    }
}