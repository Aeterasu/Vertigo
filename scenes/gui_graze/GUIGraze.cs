using Godot;
using System;

namespace Vertigo
{
    public class GUIGraze : Control
    {
        [Export] private readonly float _playbackSpeed = 1.6f; 

        private bool _isGrazing = false;

        private AnimationPlayer _animationPlayer;

        private Tween _tween = new Tween();

        private CPUParticles2D _cpuParticles2D;

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
            _animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");

            AddChild(_tween);

            _cpuParticles2D = GetNode<CPUParticles2D>("CPUParticles2D");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_player.IsValid())
            {
                IsGrazing = _player.IsGrazing;
                _cpuParticles2D.Emitting = _player.IsGrazing;
            }
        }

        public void OnAnimationFinished(string animName)
        {
            if (animName == "Appear")
            {
                _animationPlayer.Play("Idle");
            }
        }
    }
}