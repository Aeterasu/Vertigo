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

    private Vector2 _defaultGlobalPosition = Vector2.Zero;

    private Vector2 _offset = Vector2.Zero;

    public override void _Ready()
    {
        base._Ready();

        _defaultGlobalPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (!Enabled)
        {
            return;
        }

        _time += _speed * delta;

        _offset = _offset.LinearInterpolate(new Vector2(Mathf.Cos(_time), Mathf.Sin(_time)) * _strength, _strengthLerpWeight * delta);

        GlobalPosition = _defaultGlobalPosition + _offset;
    }
}
