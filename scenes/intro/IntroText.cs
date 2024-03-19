using Godot;
using System;

namespace Vertigo
{
    public class IntroText : Control
    {
        [Export] public float TimeQueue { get; private set; } = 0.0f;
        [Export] private readonly string _introTextKey = "";
        [Export] private readonly string _breakdownTextKey = "";
        [Export] private readonly float _screenShakeStrength = 2.0f;

        private bool _animated = false;
        private bool _animatedBreakdown = false;

        public override void _Ready()
        {
            base._Ready();

            GetNode<Label>("Label").Text = Tr(_introTextKey);
            GetNode<Label>("Label/Label2").Text = Tr(_introTextKey);
            GetNode<Label>("Label/Label3").Text = Tr(_introTextKey);

            Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        public void Animate()
        {
            if (_animated)
            {
                return;
            }

            GetNode<AnimationPlayer>("AnimationPlayer").Play("Appear");

            _animated = true;

            var intro = GetParentOrNull<Intro>();

            DeconflictedSoundPlayer.PlayUnconflictingSound(intro.SfxTextAppear, GetTree().Root);            
        }

        public void AnimateBreakdown()
        {
            if (_animatedBreakdown)
            {
                return;
            }

            GetNode<AnimationPlayer>("AnimationPlayer").Play("AnimateBreakdown");

            GetNode<Label>("Label").Text = Tr(_breakdownTextKey);
            GetNode<Label>("Label/Label2").Text = Tr(_breakdownTextKey);
            GetNode<Label>("Label/Label3").Text = Tr(_breakdownTextKey);

            _animatedBreakdown = true;

            var intro = GetParentOrNull<Intro>();

            intro.GetScreenShakeCamera2D.AddShake(_screenShakeStrength);

            DeconflictedSoundPlayer.PlayUnconflictingSound(intro.SfxTextBreakdown, GetTree().Root);
        }
    }
}