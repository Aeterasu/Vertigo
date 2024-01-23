using Godot;
using System;

namespace Vertigo
{
    public static class Area2DExtensions
    {
        public static void UpdateAttackCollisionMaskBits(this Area2D area, Affiliation affiliation)
        {
            // 0 - player layer
            // 2 - enemy layer
            // 3 - player attacks layer
            // 4 - enemy attack layer

            switch (affiliation)
            {
                case Affiliation.NEUTRAL:
                    area.SetCollisionMaskBit(0, true);
                    area.SetCollisionMaskBit(2, true);

                    area.SetCollisionLayerBit(3, true);
                    area.SetCollisionLayerBit(4, true);
                    break;
                case Affiliation.PLAYER:
                    area.SetCollisionMaskBit(0, false);
                    area.SetCollisionMaskBit(2, true);

                    area.SetCollisionLayerBit(3, true);
                    area.SetCollisionLayerBit(4, false);
                    break;
                case Affiliation.ENEMY:
                    area.SetCollisionMaskBit(0, true);
                    area.SetCollisionMaskBit(2, false);

                    area.SetCollisionLayerBit(3, false);
                    area.SetCollisionLayerBit(4, true);
                    break;
            }        
        }

        public static void UpdateHurtboxCollisionLayerBits(this Area2D area, Affiliation affiliation)
        {
            // 0 - player layer
            // 2 - enemy layer
            // 3 - player attacks layer
            // 4 - enemy attack layer

            switch (affiliation)
            {
                case Affiliation.NEUTRAL:
                    area.SetCollisionLayerBit(0, true);
                    area.SetCollisionLayerBit(2, true);
                    break;
                case Affiliation.PLAYER:
                    area.SetCollisionLayerBit(0, true);
                    area.SetCollisionLayerBit(2, false);
                    break;
                case Affiliation.ENEMY:
                    area.SetCollisionLayerBit(0, false);
                    area.SetCollisionLayerBit(2, true);
                    break;
            }        
        }
    }
}