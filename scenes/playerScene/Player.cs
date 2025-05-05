using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed = 150;

    private Vector2 _inputDirection = Vector2.Zero;
    private AnimatedSprite2D _animatedSprite;
    private string _currentDirection = "front";

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleInput();
        ApplyMovement();
        UpdateAnimation();
    }

    private void HandleInput()
    {
        _inputDirection = Vector2.Zero;

        if (Input.IsActionPressed("ui_right"))
            _inputDirection.X += 1;
        if (Input.IsActionPressed("ui_left"))
            _inputDirection.X -= 1;
        if (Input.IsActionPressed("ui_down"))
            _inputDirection.Y += 1;
        if (Input.IsActionPressed("ui_up"))
            _inputDirection.Y -= 1;

        _inputDirection = _inputDirection.Normalized();
    }

    private void ApplyMovement()
    {
        Velocity = _inputDirection * Speed;
        MoveAndSlide();
    }

    private void UpdateAnimation()
    {
        // Cambiar la dirección según el último input
        if (_inputDirection != Vector2.Zero)
        {
            if (Mathf.Abs(_inputDirection.X) > Mathf.Abs(_inputDirection.Y))
            {
                _currentDirection = _inputDirection.X > 0 ? "right" : "left";
            }
            else
            {
                _currentDirection = _inputDirection.Y > 0 ? "front" : "back";
            }

            _animatedSprite.Play($"{_currentDirection}_walk");
        }
        else
        {
            _animatedSprite.Play($"{_currentDirection}_idle");
        }
    }
}