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

        public static AudioStream HitSound { get; private set; }
        public static AudioStream DestroyedSound { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            if (HitSound == null)
            {
                HitSound = GD.Load<AudioStream>("res://assets/audio/sfx_hazard_hit.wav");
            }

            if (DestroyedSound == null)
            {
                DestroyedSound = GD.Load<AudioStream>("res://assets/audio/sfx_hazard_destroyed_1.wav");
            }

            this.UpdateAttackCollisionMaskBits(Affiliation);
        }
    }
}
