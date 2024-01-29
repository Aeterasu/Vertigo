using Godot;
using System;

namespace Vertigo
{
    public class Aegis : Upgrade
    {
        [Export] private readonly float _initialImmunityTime = 4f;
        [Export] private readonly float _additionImmunityTimePerStack = 2f;

        private float _immunityTime = 0f;

        public override void _Ready()
        {
            base._Ready();

            _immunityTime += _initialImmunityTime;

            ActivateImmunity();
        }

        public override void AddStack()
        {
            base.AddStack();

            _immunityTime += _additionImmunityTimePerStack;
        }

        public void ActivateImmunity()
        {
            var player = Game.Instance.GetNodeOrNull<Player>("%Player");

            player.ImmunityTimer += _immunityTime;
        }
    }
}