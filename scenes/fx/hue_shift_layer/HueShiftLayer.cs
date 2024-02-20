using Godot;
using System;

public class HueShiftLayer : CanvasLayer
{
    [Export] private GradientTexture2D _gradient;

    private ShaderMaterial _shaderMaterial;

    public GradientTexture2D Gradient
    {
        get => _gradient;
        set
        {
            _gradient = value;
            UpdateGradient();
        }
    }

    public override void _Ready()
    {
        base._Ready();

        _shaderMaterial = GetNode<ColorRect>("HueShift").Material as ShaderMaterial;
    }

    public void UpdateGradient()
    {
        if (Gradient == null)
        {
            return;
        }

        _shaderMaterial.SetShaderParam("gradient", Gradient);
    }
}
