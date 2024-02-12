using Godot;
using System;

namespace Vertigo
{
    public class HurtboxComponent : Area2D
    {
        [Export] private readonly Affiliation _affiliation = Affiliation.NEUTRAL;

        public event Action<Projectile> OnProjectileCollide;

        public bool Immune { get; set; } = false;

        public override void _Ready()
        {
            base._Ready();

            this.UpdateHurtboxCollisionLayerBits(_affiliation);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Immune)
            {
                return;
            }

            var areas = GetOverlappingAreas();

            foreach (Area2D area in areas)
            {
                if (area is Projectile)
                {
                    OnProjectileCollide?.Invoke(area as Projectile);
                }
            }
        }
    }
}
