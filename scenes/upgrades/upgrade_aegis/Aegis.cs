using Godot;
using System;

namespace Vertigo
{
    public class Aegis : Upgrade
    {
        [Export] private readonly float _initialImmunityTime = 4f;
        [Export] private readonly float _additionImmunityTimePerStack = 2f;

        private float _immunityTime = 0f;

        private Player _player;

        public override void _Ready()
        {
            base._Ready();

            _immunityTime += _initialImmunityTime;

            _player = Game.Instance.GetNodeOrNull<Player>("%Player");

            if (!_player.IsValid())
            {
                return;
            }

            _player.ImmunityTimer = _immunityTime;
        }

        public override void AddStack()
        {
            base.AddStack();

            _immunityTime += _additionImmunityTimePerStack;
        }

        public void ActivateImmunity()
        {
            if (!_player.IsValid())
            {
                return;
            }

            _player.ImmunityTimer = _immunityTime;
        }
    }
}