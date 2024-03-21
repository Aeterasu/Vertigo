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
        private float _ticks = 0.0f;

        private int _currentText = 0;

        private AudioStreamPlayer _musicPlayer;
        private Tween _tween = new Tween();

        private List<IntroText> _introTexts = new List<IntroText>();

        private ScreenShakeCamera2D _screenShakeCamera2d;

        public ScreenShakeCamera2D GetScreenShakeCamera2D { get => _screenShakeCamera2d; }

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
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            //_ticks += delta;

            if (_ticks >= _textRate)
            {
                _ticks = 0.0f;
                
                if (_currentText < _introTexts.Count)
                {
                    _introTexts[_currentText].Animate();

                    _currentText += 1;
                }
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (@event is InputEventKey || @event is InputEventJoypadButton || @event is InputEventMouseButton)
            {
                if (@event.IsPressed())
                {
                    GetNode<AnimationPlayer>("AnimationPlayer").Play("Skip");
                }
            }
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