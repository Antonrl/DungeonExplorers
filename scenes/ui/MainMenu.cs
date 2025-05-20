using Godot;
using System;

public partial class MainMenu : Control
{
    private void btnPlay_click()
    {
        GetTree().ChangeSceneToFile("res://scenes/world/Tilemap.tscn");
    }
    private void btnExit_click()
    {
        GetTree().Quit();
    }

}
