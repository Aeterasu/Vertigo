using Godot;
using System;

namespace Vertigo
{
    public class SpriteTrail : Sprite
    {
        public float DisappearanceSpeed = 1f;

        public float Alpha = 1f;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, Alpha);

            Alpha -= DisappearanceSpeed * delta;

            if (Alpha <= 0)
            {
                this.SafeQueueFree();
            }
        }
    }
}
