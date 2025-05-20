using Godot;
using System;

public partial class Spikes : Area2D
{
    [Export]
    private AnimationPlayer _animationPlayer;
    private Node2D _player;
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    public override void _PhysicsProcess(double delta)
    {
        _animationPlayer.Play("pierce");
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            GD.Print("Entro en el area");
            player.hurt(8);
            player.ApplyKnockback(GlobalPosition,50f);
        }
    }
}
