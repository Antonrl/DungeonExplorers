using Godot;
using System;

public partial class PauseMenu : Control
{
    private AnimationPlayer _animationPlayer;
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    public void resume()
    {
        GetTree().Paused = false;
        _animationPlayer.PlayBackwards("blur");
    }
    public void pause()
    {
        GetTree().Paused = true;
        _animationPlayer.Play("blur");
    }


}
