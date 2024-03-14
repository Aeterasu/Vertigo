using Godot;
using System;

namespace Vertigo
{
    public class ShockwaveEffect : Sprite
    {
        private const float DEFAULT_SIZE = 0.0f;
        private const float DEFAULT_FORCE = 1.0f;

        private const float TARGET_SIZE = 1.0f;
        private const float TARGET_FORCE = 0.0f;

        [Export] private readonly NodePath _playerNodePath;
        [Export] private readonly float _sizeSpeed = 1f;
        [Export] private readonly float _forceSpeed = 0.5f;

        private float _size = DEFAULT_SIZE;
        private float _force = DEFAULT_FORCE;

        private Player _player;

        private ShaderMaterial _shaderMaterial;

        public override void _Ready()
        {
            base._Ready();

            _player = GetNodeOrNull<Player>(_playerNodePath);

            _shaderMaterial = Material as ShaderMaterial;

            _size = TARGET_SIZE;
            _force = TARGET_FORCE;
        }

        public override void _Process(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_player.IsValid())
            {
                return;
            }

            _size = Mathf.Min(_size + _sizeSpeed * delta, TARGET_SIZE);
            _force = Mathf.Max(_force - _forceSpeed * delta, TARGET_FORCE);

            _shaderMaterial.SetShaderParam("global_position", _player.GlobalPosition);
            _shaderMaterial.SetShaderParam("size", _size);
            _shaderMaterial.SetShaderParam("force", _force);
        }

        public void ResetSize()
        {
            _size = DEFAULT_SIZE;
            _force = DEFAULT_FORCE;
        }
    }
}