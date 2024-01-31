using Godot;
using System;

namespace Vertigo
{
    public class Hazard : Area2D
    {
        [Export] public Affiliation Affiliation { get; private set; } = Affiliation.ENEMY;

        public float Slowdown { get; set; } = 1.0f;

        public Action<Hazard> OnLeftThePlayArea;
        public Action<Hazard> OnDestroyed;

        public override void _Ready()
        {
            base._Ready();

            this.UpdateAttackCollisionMaskBits(Affiliation);
        }
    }
}
