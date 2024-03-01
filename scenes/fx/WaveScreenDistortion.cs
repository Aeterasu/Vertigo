using Godot;
using System;

public class WaveScreenDistortion : ColorRect
{   
    [Export] private readonly float _lerpWeight = 1f;
    [Export] private readonly float _waveWidth = 3f;
    [Export] private readonly float _waveFrequency = 10f;

    private float _currentWaveWidth = 0f;
    private float _currentWaveFrequency = 0f;

    private ShaderMaterial _material;

    public bool Enabled { get; set; } = false;

    public override void _Ready()
    {
        base._Ready();

        _material = Material as ShaderMaterial;

        _material.SetShaderParam("wave_width", 0f);
        _material.SetShaderParam("wave_freq", 0f);        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (Enabled)
        {
            _currentWaveWidth = Mathf.Lerp(_currentWaveWidth, _waveWidth, _lerpWeight * delta);
            _currentWaveFrequency = Mathf.Lerp(_currentWaveFrequency, _waveFrequency, _lerpWeight * delta);
        }
        else
        {
            _currentWaveWidth = Mathf.Lerp(_currentWaveWidth, 0f, _lerpWeight * delta);
            _currentWaveFrequency = Mathf.Lerp(_currentWaveFrequency, 0f, _lerpWeight * delta);            
        }

        _material.SetShaderParam("wave_width", _currentWaveWidth);
        _material.SetShaderParam("wave_freq", _currentWaveFrequency);
    }
}
