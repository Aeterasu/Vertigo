using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class Intro : CanvasLayer
    {
        [Export] public AudioStream SfxTextAppear { get; private set; }
        [Export] public AudioStream SfxTextBreakdown { get; private set; }
        [Export] private readonly MainState _targetState = MainState.MAIN_MENU;
        [Export] private readonly float _textRate = 0.8f;
        [Export] private readonly float _timeToSkip = 1f;
        private float _ticks = 0.0f;

        private int _currentText = 0;

        private float _skipCurrentTicks = 0.0f;
        private bool _showSkipLabel = false;

        private AudioStreamPlayer _musicPlayer;
        private Tween _tween = new Tween();

        private List<IntroText> _introTexts = new List<IntroText>();

        private RichTextLabel _skipRichTextLabel;
        private TextureProgress _skipProgress;

        private ScreenShakeCamera2D _screenShakeCamera2d;

        public ScreenShakeCamera2D GetScreenShakeCamera2D { get => _screenShakeCamera2d; }

        public bool IsSkipped { get; private set; } = false;

        public override void _Ready()
        {
            base._Ready();

            GetNode<AnimationPlayer>("AnimationPlayer").Play("Intro");
            GetNode<AnimationPlayer>("AnimationPlayer").Connect("animation_finished", this, "OnAnimationFinished");

            _musicPlayer = GetNode<AudioStreamPlayer>("Music");
            
            AddChild(_tween);

            _musicPlayer.Play();

            _screenShakeCamera2d = GetNode<ScreenShakeCamera2D>("%ScreenShakeCamera2D");

            foreach (Node node in GetChildren())
            {
                if (node is IntroText)
                {
                    _introTexts.Add(node as IntroText);
                }
            }

            _skipRichTextLabel = GetNode<RichTextLabel>("HoldToSkipLabel");
            _skipProgress = GetNode<TextureProgress>("SkipProgress");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_ticks >= _textRate)
            {
                _ticks = 0.0f;
                
                if (_currentText < _introTexts.Count)
                {
                    _introTexts[_currentText].Animate();

                    _currentText += 1;
                }
            }

            _skipRichTextLabel.BbcodeText = InputDeviceChecker.InputDevice == InputDevice.CONTROLLER ? "[right]" + Tr("INTRO_HOLD_TO_SKIP_GAMEPAD") : "[right]" + Tr("INTRO_HOLD_TO_SKIP_KEYBOARD");

            if (Input.IsActionPressed(InputActions.INPUT_ACTION_CONFIRM) || Input.IsActionPressed(InputActions.INPUT_MENU_CONFIRM))
            {
                _skipCurrentTicks += delta;
                _showSkipLabel = true;
            }
            else
            {
                _skipCurrentTicks = 0.0f;
            }

            if (_skipCurrentTicks >= _timeToSkip && !IsSkipped)
            {
                GetNode<AnimationPlayer>("AnimationPlayer").Play("Skip");
                IsSkipped = true;
            }

            _skipProgress.MaxValue = _timeToSkip;
            _skipProgress.Value = _skipCurrentTicks;

            _skipRichTextLabel.Modulate = _showSkipLabel ? _skipRichTextLabel.Modulate.LinearInterpolate(new Color(1.0f, 1.0f, 1.0f, 1.0f), 5.0f * delta) : new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        private void OnAnimationFinished(string animName)
        {
            Main.Instance.State = _targetState;
        }

        public void StopMusic()
        {
            _tween.InterpolateProperty(_musicPlayer, "volume_db", GD.Linear2Db(1.0f), GD.Linear2Db(0.0f), 2.0f);
            _tween.Start();            
        }
    }
}