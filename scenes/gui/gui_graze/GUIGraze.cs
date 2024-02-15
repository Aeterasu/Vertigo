using Godot;
using System;

namespace Vertigo
{
    public class GUIGraze : Control
    {
        [Export] private readonly float _playbackSpeed = 1.6f;
        [Export] private readonly float _gradientTargetAlpha = 0.8f;

        private bool _isGrazing = false;

        private AnimationPlayer _animationPlayer;

        private Tween _tween = new Tween();

        private CPUParticles2D _cpuParticles2D;

        private TextureRect _gradient;
        private float _currentGradientAlpha = 0f;

        private Player _player;

        public bool IsGrazing
        {
            get => _isGrazing;
            set
            {
                if (!_isGrazing && value)
                {
                    Modulate = new Color(1, 1, 1, 1);
                    _animationPlayer.Play("Appear");
                    _animationPlayer.PlaybackSpeed = _playbackSpeed;
                }

                if (_isGrazing && value)
                {
                    _tween.InterpolateProperty(this, "modulate", new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 0.3f);
                    _tween.InterpolateProperty(_animationPlayer, "playback_speed", _playbackSpeed, 0f, 0.3f);
                    _tween.Start();
                }

                _isGrazing = value;
            }
        }

        public override void _Ready()
        {
            _player = GetNodeOrNull<Player>("%Player");

            Modulate = new Color(1, 1, 1, 0);

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            AddChild(_tween);

            _cpuParticles2D = GetNode<CPUParticles2D>("CPUParticles2D");

            _gradient = GetNode<TextureRect>("Gradient");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_player.IsValid())
            {
                IsGrazing = _player.IsGrazing;
                _cpuParticles2D.Emitting = _player.IsGrazing;
            }

            if (IsGrazing)
            {
                _currentGradientAlpha = Mathf.Lerp(_currentGradientAlpha, _gradientTargetAlpha, 5 * delta);
            }
            else
            {
                _currentGradientAlpha = Mathf.Lerp(_currentGradientAlpha, 0f, 5 * delta);
            }
            
            _gradient.Modulate = new Color(1, 1, 1, _currentGradientAlpha);
        }

        public void OnAppearAnimationFinished()
        {
            _animationPlayer.Play("Idle");
        }
    }
}