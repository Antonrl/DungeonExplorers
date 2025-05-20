using Godot;
using System;

public partial class SceneTransitor : CanvasLayer
{
    private string new_scene;
    private AnimationPlayer _animationPlayer;


    public override void _Ready()
    {

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void start_transition(string scene)
    {
        new_scene = scene;
        _animationPlayer.Play("change_scene");
    }


    // [Callable]
    // public void change_scene()
    // {

    // }
}