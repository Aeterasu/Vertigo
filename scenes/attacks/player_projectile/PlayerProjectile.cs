using Godot;
using System;
using Vertigo;

public class PlayerProjectile : Projectile
{
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        base._Ready();

        _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        _animationPlayer.Play("Muzzle");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }
}
