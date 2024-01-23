using Godot;
using System;

namespace Vertigo
{
    public class HurtboxComponent : Area2D
    {
        [Export] private readonly Affiliation _affiliation = Affiliation.NEUTRAL;

        public event Action<Projectile> OnProjectileCollide;

        public override void _Ready()
        {
            base._Ready();

            this.UpdateHurtboxCollisionLayerBits(_affiliation);

            Connect("area_entered", this, "OnAreaEntered");
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is Projectile)
            {
                OnProjectileCollide?.Invoke(area as Projectile);
            }
        }
    }
}
