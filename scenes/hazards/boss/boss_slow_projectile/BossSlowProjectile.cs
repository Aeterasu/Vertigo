using Godot;
using System;

namespace Vertigo
{
    public class BossSlowProjectile : Square
    {
        [Export] private readonly float _lerpWeight = 4.0f;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            Speed = Mathf.Lerp(Speed, 0.0f, _lerpWeight * delta);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
        }
    }
}