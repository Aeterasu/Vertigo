using Godot;
using System;

namespace Vertigo
{
    public class Hazard : Area2D
    {
        [Export] public Affiliation Affiliation { get; private set; } = Affiliation.ENEMY;

        public override void _Ready()
        {
            base._Ready();

            this.UpdateAttackCollisionMaskBits(Affiliation);
        }
    }
}
