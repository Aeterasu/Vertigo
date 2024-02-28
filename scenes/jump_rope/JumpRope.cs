using Godot;
using System;

namespace Vertigo
{
    public class JumpRope : Area2D
    {
        [Export] public GravityMode GravityMode { get; private set; }
        [Export] private readonly AudioStream _activationSound;

        private JumpRopeSmoothPath _smoothPath;

        public Action<JumpRope> OnJumpRopeActivated;

        public Vector2 LastPlayerPosition { get; set; }

        public override void _Ready()
        {
            base._Ready();

            _smoothPath = GetNode<JumpRopeSmoothPath>("JumpRopeSmoothPath");
        }

        public void AnimateSmoothPath(float positionX, float direction)
        {
            _smoothPath.Animate(positionX, direction);

            if (_activationSound != null)
            {
                float pitch = GravityMode == GravityMode.UPWARDS ? 1f : 0.8f;
                DeconflictedSoundPlayer.PlayUnconflictingSound(_activationSound, this, 0, pitch);
            }
        }
    }
}
