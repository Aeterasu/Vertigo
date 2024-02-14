using Godot;
using System;

namespace Vertigo
{
    public class GrazeVisual : Sprite
    {
        [Export] private readonly float _lerpWeight = 6f;
        [Export] private readonly float _deletionSpeed = 3f;
        [Export] private readonly float _alphaSpeed = 0.8f;

        private float _currentScale = 1f;
        private float _currentAlpha = 1f;

        private Square _parent;

        public float TargetScale { get; set; } = 1f;
        public float TargetAlpha { get; set; } = 1f;
        public bool IsParentDestroyed { get; set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _parent = GetParentOrNull<Square>();

            if (_parent.IsValid())
            {
                _parent.OnDestroyed += OnParentDestroyed;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!IsParentDestroyed)
            {
                _currentScale = Mathf.Lerp(_currentScale, TargetScale, _lerpWeight * delta);
                _currentAlpha = TargetAlpha;
            }
            else
            {
                _currentScale += _deletionSpeed * delta;
                _currentAlpha = Mathf.Max(_currentAlpha - _alphaSpeed * delta, 0f);

                if (_currentAlpha <= 0f)
                {
                    this.SafeQueueFree();
                }
            }

            Scale = Vector2.One * _currentScale;

            Modulate = new Color(1f, 1f, 1f, _currentAlpha);
        }

        private void OnParentDestroyed(Hazard hazard)
        {
            IsParentDestroyed = true;

            _parent.RemoveChild(this);
            GlobalPosition = _parent.GlobalPosition;

            if (Game.Instance.IsValid())
            {
                if (Game.Instance.GameLayer.IsValid())
                {
                    Game.Instance.GameLayer.AddChild(this);
                }
            }
        }
    }
}