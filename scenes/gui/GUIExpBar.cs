using Godot;
using System;

namespace Vertigo
{
    public class GUIExpBar : TextureProgress
    {
        [Export] private readonly float _animationSpeed = 4f;

        private ExperienceComponent _experienceComponent;

        private TextureProgress _textureProgress;

        private Label _label;

        private readonly Tween _tween = new Tween();

        public override void _Ready()
        {
            base._Ready();

            Player player = GetNodeOrNull<Player>("%Player");

            if (player.IsValid())
            {
                _experienceComponent = player.GetNodeOrNull<ExperienceComponent>("ExperienceComponent");
            }

            if (_experienceComponent.IsValid())
            {
                _experienceComponent.OnExperienceGranted += OnExperienceGranted;
                _experienceComponent.OnLevelUp += OnLevelUp;
            }

            _textureProgress = GetNode<TextureProgress>("TextureProgress");
            _label = GetNode<Label>("Label");

            AddChild(_tween);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_experienceComponent.IsValid())
            {
                return;
            }

            if (_label.IsValid())
            {
                _label.Text = $"Lvl. {_experienceComponent.Level}";
            }

            Value = Mathf.Min((float)(Value + _animationSpeed * delta), (float)_textureProgress.Value);
        }

        private void OnExperienceGranted(ExperienceComponent exp)
        {
            _textureProgress.MinValue = 0f;
            _textureProgress.MaxValue = exp.TargetExp;
            _textureProgress.Value = exp.CurrentExp;

            MinValue = 0f;
            MaxValue = exp.TargetExp;
        }

        private void OnLevelUp(ExperienceComponent exp)
        {
            _tween.RemoveAll();

            _textureProgress.MinValue = 0f;
            _textureProgress.MaxValue = exp.TargetExp;
            _textureProgress.Value = exp.CurrentExp;

            MinValue = 0f;
            MaxValue = exp.TargetExp;
            Value = exp.CurrentExp;
        }
    }
}
