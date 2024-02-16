using Godot;
using System;

public class HueShiftLayer : CanvasLayer
{
    [Export] public GradientTexture2D Gradient { get; private set; }

    private ShaderMaterial _shaderMaterial;

    public override void _Ready()
    {
        base._Ready();

        _shaderMaterial = GetNode<ColorRect>("HueShift").Material as ShaderMaterial;
        UpdateGradient();
    }

    public void UpdateGradient()
    {
        _shaderMaterial.SetShaderParam("gradient", Gradient);
    }
}
