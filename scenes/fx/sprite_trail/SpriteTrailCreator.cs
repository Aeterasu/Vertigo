using Godot;
using System;

namespace Vertigo
{
    public class SpriteTrailCreator : Node2D
    {
        [Export] private readonly float _creationRate = 0.1f;
        [Export] private readonly float _disappearanceSpeed = 1f;
        [Export] private readonly float _initialAlpha = 1f;
        [Export] private readonly Color _color;
        [Export] private readonly NodePath _parentPath;

        private float _timer = 0f;

        private Sprite _parent;

        private static Shader _colorFillShader;

        public override void _Ready()
        {
            base._Ready();

            var parent = GetParent();

            if (parent is Sprite)
            {
                _parent = parent as Sprite;
            }

            _timer = _creationRate;

            if (_colorFillShader == null)
            {
                _colorFillShader = GD.Load<Shader>("res://shaders/color_fill.tres");
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_parent == null)
            {
                return;
            }

            _timer -= delta;

            if (_timer <= 0)
            {
                _timer = _creationRate;
                SpriteTrail trail = new SpriteTrail();
                trail.Texture = _parent.Texture;
                trail.Centered = _parent.Centered;
                trail.FlipH = _parent.FlipH;
                trail.FlipV = _parent.FlipV;
                trail.DisappearanceSpeed = _disappearanceSpeed;
                trail.Alpha = _initialAlpha;
                trail.GlobalTransform = _parent.GlobalTransform;
                var shaderMaterial = new ShaderMaterial();
                shaderMaterial.Shader = _colorFillShader;
                shaderMaterial.SetShaderParam("color", _color);
                shaderMaterial.SetShaderParam("mixture", 1f);

                trail.Material = shaderMaterial;

                if (_parentPath != null)
                {
                    GetNode(_parentPath).AddChild(trail);
                }
                else if (Game.Instance.IsValid() && Game.Instance.GameLayer.IsValid())
                {
                    Game.Instance.GameLayer.AddChild(trail);
                }
                else
                {
                    trail.SafeQueueFree();
                }
            }
        }
    }
}
