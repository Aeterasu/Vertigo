using Godot;
using System;

public class SpiralDrunkNode2D : Node2D
{
    [Export] public bool Enabled { get; set; } = false;
    [Export] private readonly float _speed = 2f;
    [Export] private readonly float _strength = 6f;
    [Export] private readonly float _strengthLerpWeight = 4f;

    private float _currentStrength = 0f;

    private float _time = 0;

    private Vector2 _defaultPosition = Vector2.Zero;

    private Vector2 _offset = Vector2.Zero;

    public override void _Ready()
    {
        base._Ready();

        _defaultPosition = Position;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (!Enabled)
        {
            return;
        }

        _time += _speed * delta;

        _currentStrength = Mathf.Lerp(_currentStrength, _strength, _strengthLerpWeight);

        _offset = new Vector2(Mathf.Cos(_time), Mathf.Sin(_time)) * _currentStrength;

        Position = _defaultPosition + _offset;
    }
}
