using Godot;
using System;

public class DrunkNode2D : Node2D
{
    [Export] public bool Enabled { get; set; } = false;
    [Export] private readonly float _speed = 2f;
    [Export] private readonly float _strength = 6f;
    [Export] private readonly float _strengthLerpWeight = 4f;

    private float _currentStrength = 0f;

    private float _time = 0;

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!Enabled)
        {
            return;
        }

        _time += _speed * delta;

        _currentStrength = Mathf.Lerp(_currentStrength, _strength, _strengthLerpWeight * delta);

        RotationDegrees = Mathf.Sin(_time) * _currentStrength;
    }
}
