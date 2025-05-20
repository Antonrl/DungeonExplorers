using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] public int Speed = 40;

    private Node2D _player;
    private bool _isChasing = false;
    public int MaxHealth = 3;
    private int _currentHealth;
    private AnimatedSprite2D _animatedSprite;
    private AnimationPlayer _animationPlayer;
    private bool _isKnockedBack = false;
    private float _knockbackTime = 0.2f;
    private float _knockbackTimer = 0f;
    private Vector2 _knockbackVelocity = Vector2.Zero;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        var detectionArea = GetNode<Area2D>("DetectionArea");
        //detectionArea.BodyEntered += bodyDetected;
        detectionArea.BodyExited += OnBodyExited;
        _currentHealth = MaxHealth;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isKnockedBack)
        {
            _knockbackTimer -= (float)delta;
            Velocity = _knockbackVelocity;
            MoveAndSlide();

            if (_knockbackTimer <= 0f)
            {
                _isKnockedBack = false;
                Velocity = Vector2.Zero;
            }
            // Mientras esté en knockback no se ejecuta el resto
            return;
        }

        if (_isChasing && IsInstanceValid(_player))
        {
            Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
            Velocity = direction * Speed;
            MoveAndSlide();

            UpdateAnimation(direction);
        }
        else
        {
            Velocity = Vector2.Zero;
            UpdateAnimation(Vector2.Zero);
        }
    }

    //Función que gestiona las animaciones de movimiento del enemigo
    private void UpdateAnimation(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            _animatedSprite.FlipH = direction.X < 0;
            _animatedSprite.Play("move");
        }
        else
        {
            _animatedSprite.Play("idle");
        }
    }

    //Función que gestiona cuando el jugador entra del área de detección
    private void bodyDetected(Node body)
    {
        if (body is CharacterBody2D player)
        {

            //GD.Print("Entro en el area");
            _player = (Node2D)body;
            _isChasing = true;
        }
    }

    //Función que gestiona cuando el jugador sale del área de detección
    private void OnBodyExited(Node body)
    {
        if (body == _player)
        {
            _isChasing = false;
        }
    }

    //Función que gestiona el recibo de daño
    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _animationPlayer.Play("hurt");
        //GD.Print($"Enemigo recibió daño. Vida restante: {_currentHealth}");
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    //Función que gestiona el daño aplicado al jugador al entrar en la hitbox
    private void attack(Node body)
    {
        if (body is Player player)
        {
            //GD.Print("Entro en el area");
            _animationPlayer.Play("applyDamage");
            player.hurt(8);
            player.ApplyKnockback(GlobalPosition, 200f);
        }
    }

    //Función que gestiona la muerte de los enemigos
    private void Die()
    {
        QueueFree(); // Elimina el enemigo de la escena
    }

    //Función de gestión del knockback
    public void ApplyKnockback(Vector2 fromPosition)
    {
        Vector2 direction = (GlobalPosition - fromPosition).Normalized();
        float knockbackStrength = 120f; // Más suave que el del jugador
        _knockbackVelocity = direction * knockbackStrength;

        _isKnockedBack = true;
        _knockbackTimer = _knockbackTime;
    }
}