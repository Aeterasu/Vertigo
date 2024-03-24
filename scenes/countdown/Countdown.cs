using Godot;
using System;

namespace Vertigo
{
    public class Countdown : Control
    {
        [Export] private readonly float _animationFinishThreshold = 4.5f;
        [Export] private readonly float _speedUpRatio = 3f;

        private AnimationPlayer _animationPlayer;

        private float _ticks = 0;

        private bool _animLock = false;

        public event Action OnCountdownFinished;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

            _animationPlayer.Play("Countdown");
            GetNode<Label>("AnimationPlayer/LabelFluff").Text = Tr("COUNTDOWN_TEXT_1");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _ticks += delta;

            if (_animationPlayer.CurrentAnimationPosition >= _animationFinishThreshold && !_animLock)
            {
                OnAnimationFinished("Countdown");
                _animLock = true;
            }

            if ((Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM) || Input.IsActionJustPressed(InputActions.INPUT_MENU_CONFIRM)) && (_animationPlayer.CurrentAnimationPosition < 3.6f && _ticks >= 0.3f))
            {
                _animationPlayer.PlaybackSpeed = _speedUpRatio;
            }
        }

        private void OnAnimationFinished(string animName)
        {
            if (animName == "Countdown")
            {
                OnCountdownFinished?.Invoke();
            }
        }

        public void PlayTickAudio()
        {
            var audioStream = GetNode<AudioStreamPlayer>("TickAudio");
            DeconflictedSoundPlayer.PlayUnconflictingSound(audioStream.Stream, this, audioStream.VolumeDb, audioStream.PitchScale);
        }

        public void PlayVertigoAudio()
        {
            var audioStream = GetNode<AudioStreamPlayer>("VertigoAudio");
            DeconflictedSoundPlayer.PlayUnconflictingSound(audioStream.Stream, this, audioStream.VolumeDb, audioStream.PitchScale);
        }
    }
}