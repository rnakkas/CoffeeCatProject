using System;
using Godot;

namespace CoffeeCatProject.Player.Scripts;

public partial class Player : CharacterBody2D
{
    private const float Speed = 170.0f;
    private const float JumpVelocity = -250.0f;
    private const float Gravity = 750.0f;
    private const float WallSlideGravity = 500.0f;
    private const float WallJumpVelocity = -200.0f;
    private const float FloorSnapLengthValue = 2.5f;

    private AnimatedSprite2D _animation;
    private RayCast2D _leftWallDetect, _rightWallDetect;
    private Marker2D _muzzle;
    
    // Load packed scene of bullet
    private readonly PackedScene _playerBullet = ResourceLoader.Load<PackedScene>("res://Player/Scenes/player_bullet.tscn");

    private enum State
    {
        Idle, 
        Run, 
        Jump, 
        WallSlide, 
        Fall, 
        WallJump,
        Shoot
    };

    private State _currentState;
    private float _wallJumpDirection;
    private Vector2 _velocity;
    private bool _isShooting;
    private Vector2 _muzzlePosition;
    private float _direction = 1.0f;

    public override void _Ready()
    {
        _animation = GetNode<AnimatedSprite2D>("sprite");
        _leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
        _rightWallDetect = GetNode<RayCast2D>("right_wall_detect");
        _muzzle = GetNode<Marker2D>("marker");

        _animation.FlipH = true;
        _animation.Play("idle");
        
        // Set z index high so player is in front of all other objects
        ZIndex = 100;

        // Keeps player snapped to floors on slopes
        this.FloorSnapLength = FloorSnapLengthValue;

        // Enable constant speed on floors and slopes
        this.FloorConstantSpeed = true;

        // Set velocity
        _velocity = Velocity;
        
        // Set muzzle position
        _muzzlePosition = _muzzle.GlobalPosition;
        GD.Print("Muzzle pos: " + _muzzlePosition);
    }

    // State Machine
    private void SetState(State newState)
    {
        if (newState == _currentState)
            return;
        
        ExitState();
        _currentState = newState;
        EnterState();
    }

    private async void EnterState()
    {
        switch (_currentState)
        {
            case State.Idle:
                _velocity.Y = 0;
                
                // If shooting is the current animation, wait for it to finish before doing next animation 
                if (_animation.Animation == "idle_shoot" || 
                    _animation.Animation == "run_shoot")
                {
                     await ToSignal(_animation, "animation_finished");
                }
                
                _animation.Play("idle");
                break;
            
            case State.Run:
                // If shooting is the current animation, wait for it to finish before doing next animation
                if (_animation.Animation == "idle_shoot" || 
                    _animation.Animation == "run_shoot")
                {
                    await ToSignal(_animation, "animation_finished");
                }

                _animation.Play("run");
                break;
            
            case State.Jump:
                _velocity.Y = JumpVelocity;
                
                // If shooting is the current animation, wait for it to finish before doing next animation
                if (_animation.Animation == "idle_shoot" ||
                    _animation.Animation == "run_shoot")
                {
                    await ToSignal(_animation, "animation_finished");
                }
                
                _animation.Play("jump");
                break;
            
            case State.WallSlide:
                _animation.Play("wall_slide");
                break;
            case State.Fall:
                // If shooting is the current animation, wait for it to finish before doing next animation
                if (_animation.Animation == "idle_shoot" || 
                    _animation.Animation == "run_shoot")
                {
                    await ToSignal(_animation, "animation_finished");
                }
                
                _animation.Play("fall");
                break;
            
            case State.WallJump:
                _velocity.Y = WallJumpVelocity;
                
                // If shooting is the current animation, wait for it to finish before doing next animation
                if (_animation.Animation == "idle_shoot")
                {
                    await ToSignal(_animation, "animation_finished");
                }
                
                _animation.Play("jump");
                break;
            
            case State.Shoot:
                _animation.Play("run_shoot");
                break;
        }
    }

    private void ExitState()
    {
        switch (_currentState)
        {
            case State.Idle:
                break;
            case State.Run:
                break;
            case State.Jump:
                break;
            case State.WallSlide:
                break;
            case State.Fall:
                break;
            case State.WallJump:
                break;
            case State.Shoot:
                _isShooting = false;
                break;
        }
    }

    private void UpdateState(float delta)
    {
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        
        switch (_currentState)
        {
            case State.Idle:
                if (direction.X != 0)
                {
                    SetState(State.Run);
                }
                else if (Input.IsActionJustPressed("jump") && IsOnFloor())
                {
                    SetState(State.Jump);
                }
                else if (!IsOnFloor())
                {
                    SetState(State.Fall);
                }
                else if (Input.IsActionJustPressed("shoot"))
                {
                    _isShooting = true;
                    SetState(State.Shoot);
                }
                else if (Input.IsActionJustReleased("shoot"))
                {
                    _isShooting = false;
                }
                
                break;

            case State.Run:
                _velocity.X = direction.X * Speed;
                FlipSprite(direction.X);

                if (direction.X == 0)
                {
                    SetState(State.Idle);
                }
                else if (Input.IsActionJustPressed("jump") && IsOnFloor())
                {
                    SetState(State.Jump);
                }
                else if (!IsOnFloor())
                {
                    SetState(State.Fall);
                }
                else if (Input.IsActionJustPressed("shoot"))
                {
                    _isShooting = true;
                    SetState(State.Shoot);
                }
                else if (Input.IsActionJustReleased("shoot"))
                {
                    _isShooting = false;
                }

                Velocity = _velocity;
                MoveAndSlide();
                break;

            case State.Jump:
                _velocity.X = direction.X * Speed;
                FlipSprite(direction.X);

                if (!IsOnFloor())
                {
                    _velocity.Y += Gravity * delta;
                    if (Velocity.Y > 0)
                    {
                        SetState(State.Fall);
                    }
                    // Only detect wall tile (collision layer 5)
                    else if (_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
                    {
                        SetState(State.WallSlide);
                    }
                    else if (Input.IsActionJustPressed("shoot"))
                    {
                        _isShooting = true;
                        SetState(State.Shoot);
                    }
                    else if (Input.IsActionJustReleased("shoot"))
                    {
                        _isShooting = false;
                    }
                }

                Velocity = _velocity;
                MoveAndSlide();
                break;

            case State.Fall:
                _velocity.X = direction.X * Speed;
                FlipSprite(direction.X);

                if (IsOnFloor())
                {
                    SetState(State.Idle);
                }
                // Only detect wall tile (collision layer 5)
                else if (_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
                {
                    SetState(State.WallSlide);
                }
                else
                {
                    _velocity.Y += Gravity * delta;

                    if (Input.IsActionJustPressed("shoot"))
                    {
                        _isShooting = true;
                        SetState(State.Shoot);
                    }
                    else if (Input.IsActionJustReleased("shoot"))
                    {
                        _isShooting = false;
                    }
                }

                Velocity = _velocity;
                MoveAndSlide();
                break;

            case State.WallSlide:
                _velocity.Y += WallSlideGravity * delta;

                if (Input.IsActionJustPressed("jump"))
                {
                    if (_leftWallDetect.IsColliding())
                    {
                        _wallJumpDirection = 1.0f;
                    }
                    else if (_rightWallDetect.IsColliding())
                    {
                        _wallJumpDirection = -1.0f;
                    }

                    SetState(State.WallJump);
                    
                }
                else if (IsOnFloor())
                {
                    SetState(State.Idle);
                }

                Velocity = _velocity;
                MoveAndSlide();
                break;

            case State.WallJump:
                _velocity.X = _wallJumpDirection * Speed;
                FlipSprite(_wallJumpDirection);

                if (!IsOnFloor())
                {
                    _velocity.Y += Gravity * delta;
                    if (_velocity.Y > 0)
                    {
                        SetState(State.Fall);
                    }
                    else if (Input.IsActionJustPressed("shoot"))
                    {
                        _isShooting = true;
                        SetState(State.Shoot);
                    }
                    else if (Input.IsActionJustReleased("shoot"))
                    {
                        _isShooting = false;
                    }
                }

                Velocity = _velocity;
                MoveAndSlide();
                break;

            case State.Shoot:
                
                // Instantiate the bullet scene, cast PackedScene as type Node
                var bulletInstance = (PlayerBullet)_playerBullet.Instantiate();
                
                // Set bullet's direction based on player's direction
                bulletInstance.Direction = _direction;
                
                // Set bullet's location to muzzle location
                //TODO: Figure out the muzzle flipping 
                if (_direction < 0)
                {
                    bulletInstance.GlobalPosition = _muzzle.GlobalPosition;
                }

                if (_direction > 0)
                {
                    bulletInstance.GlobalPosition = -_muzzle.GlobalPosition;
                }
                
                // Add bullet scene to scene tree
                GetTree().Root.AddChild(bulletInstance);
                
                _velocity.X = direction.X * Speed;
                FlipSprite(direction.X);

                if (!IsOnFloor())
                {
                    _velocity.Y += Gravity * delta;
                }
                
                if (direction.X != 0)
                {
                    SetState(State.Run);
                }
                else if (direction.X == 0 && Input.IsActionJustPressed("shoot"))
                {
                    SetState(State.Shoot);
                }
                else if (Input.IsActionJustPressed("jump") && IsOnFloor())
                {
                    SetState(State.Jump);
                }
                else
                {
                    SetState(State.Idle);
                }
                
                Velocity = _velocity;
                MoveAndSlide();
                
                break;
        }
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
        {
            _animation.FlipH = false;
            _direction = -1;
        }
        else if (directionX > 0)
        {
            _animation.FlipH = true;
            _direction = 1;
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        UpdateState((float)delta);
    }
}