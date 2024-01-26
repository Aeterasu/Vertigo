using Godot;
using System;

namespace Vertigo
{
    public class JumpRope : Area2D
    {
        [Export] public GravityMode GravityMode { get; private set; }

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
        }
    }
}
