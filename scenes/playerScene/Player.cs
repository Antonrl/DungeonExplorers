using Godot;
using System;
using System.Threading;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed = 100;

    private Vector2 _inputDirection = Vector2.Zero;
    private AnimatedSprite2D _animatedSprite;
    private Node2D _sword;
    private AnimationPlayer _swordAnimPlayer;
    private AnimationPlayer _animationPlayer;
    private HealthBar _healthBar;
    private string _currentDirection = "front";
    private bool _isAttacking = false;
    public int MaxHealth = 100;
    public int _currentHealth;
    private bool _canTakeDamage = true;
    private bool _isKnockedBack = false;
    private float _knockbackTime = 0.2f; 
    private float _knockbackTimer = 0f;
    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _healthBar = GetNode<HealthBar>("../UI");
        _healthBar.Visible = true;
        _sword = GetNode<Node2D>("sword");
        _swordAnimPlayer = _sword.GetNode<AnimationPlayer>("SwordAnimationPlayer");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _currentHealth = MaxHealth;
        _sword.Visible = false;
        _currentDirection = "right";

    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isKnockedBack)
        {
            _knockbackTimer -= (float)delta;
            MoveAndSlide();

            if (_knockbackTimer <= 0f)
            {
                _isKnockedBack = false;
                Velocity = Vector2.Zero;
            }
        }
        else
        {
            HandleInput();
            ApplyMovement();
        }

        UpdateAnimation();
        HandleAttackInput();
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
        Vector2 mousePos = GetGlobalMousePosition();
        float deltaX = mousePos.X - GlobalPosition.X;

        if (deltaX < 0)
        {
            _currentDirection = "left";
            _animatedSprite.FlipH = true;  // Mira a la izquierda
        }
        else
        {
            _animatedSprite.FlipH = false; // Mira a la derecha
            _currentDirection = "right";
        }
        if (_inputDirection != Vector2.Zero)
        {
            // if (_inputDirection.X != 0)
            // {
            //     _animatedSprite.FlipH = _inputDirection.X < 0;
            // }
            // if (_inputDirection.X > 0)
            // {
            //     _animatedSprite.FlipH = false;
            //     _currentDirection = "right";
            // }
            // else
            // {
            //     if (_inputDirection.X < 0)
            //     {

            //         _animatedSprite.FlipH = true;
            //         _currentDirection = "left";
            //     }
            // }
            // if (Mathf.Abs(_inputDirection.X) > Mathf.Abs(_inputDirection.Y))
            //_currentDirection = _inputDirection.X > 0 ? "right" : "left";
            // else
            // {   
            //     _currentDirection = _inputDirection.Y > 0 ? "front" : "back";
            // }

            // _animatedSprite.Play($"{_currentDirection}_walk");
            _animatedSprite.Play("right_walk");
        }
        else
        {
            // _animatedSprite.Play($"{_currentDirection}_idle");
            _animatedSprite.Play("right_idle");
        }
    }

    private void HandleAttackInput()
    {
        if (!_isAttacking && Input.IsActionJustPressed("ui_attack"))
        {
            StartAttack();
        }
    }

    private async void StartAttack()
    {
        _isAttacking = true;
        RotateSword();
        _sword.Visible = true;

        _swordAnimPlayer.Play("new_animationattack");

        await ToSignal(_swordAnimPlayer, AnimationPlayer.SignalName.AnimationFinished);

        _sword.Visible = false;
        _isAttacking = false;
    }

    private void RotateSword()
    {
        float angle = 0f;
        if (_currentDirection == "right")
        {
            angle = 0;
        }
        else
        {
            angle = Mathf.DegToRad(180);
        }

        // switch (_currentDirection)
        //     {
        //         case "front":
        //             angle = Mathf.DegToRad(90);
        //             break;
        //         case "back":
        //             angle = Mathf.DegToRad(-90);
        //             break;
        //         case "left":
        //             angle = Mathf.DegToRad(180);
        //             break;
        //         case "right":
        //             angle = 0;
        //             break;
        //     }

        _sword.Rotation = angle;
    }
    private void OnSwordHitboxBodyEntered(Node body)
    {
        if (body is Enemy enemy && _isAttacking)
        {
            enemy.TakeDamage(1);
            enemy.ApplyKnockback(GlobalPosition);
        }
        if (body is Boss boss && _isAttacking)
        {
            boss.TakeDamage(1);
            boss.ApplyKnockback(GlobalPosition);
        }
    }
    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _healthBar.UpdateHealthBar(_currentHealth);
        //GD.Print($"Jugador recibió daño. Vida restante: {_currentHealth}");

        if (_currentHealth <= 23)
        {
            Die();
        }


    }
    private async void Die()
    {
        await ToSignal(GetTree().CreateTimer(0.6), "timeout");
        GetTree().ChangeSceneToFile("res://scenes/ui/game_over.tscn");
        //GD.Print("El jugador ha muerto.");
    }
    public async void hurt(int dmg)
    {
        if (!_canTakeDamage)
        {
            return;
        }
        TakeDamage(dmg);
        _animationPlayer.Play("hurt");
        GD.Print("Vida actual: " + _currentHealth);
        _canTakeDamage = false;
        await ToSignal(GetTree().CreateTimer(0.6), "timeout");
        _canTakeDamage = true;
    }
    public void ApplyKnockback(Vector2 fromPosition,float str)
    {
        Vector2 direction = (GlobalPosition - fromPosition).Normalized();
        Velocity = direction * str;

        _isKnockedBack = true;
        _knockbackTimer = _knockbackTime;
    }
}
