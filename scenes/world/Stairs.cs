using Godot;
using System;

public partial class Stairs : Area2D
{
    private Node2D _player;
    private async void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            await ToSignal(GetTree().CreateTimer(0.6), "timeout");
            GD.Print("Entro en el aawdawdarea");
            GetTree().ChangeSceneToFile("res://scenes/world/world_2.tscn");

        }
    }
}
