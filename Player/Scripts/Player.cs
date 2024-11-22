using Godot;
using CoffeeCatProject.Weapons.Equipped.Scripts;

namespace CoffeeCatProject.Player.Scripts;

public partial class Player : CharacterBody2D
{
    // Constants
    private const float Speed = 170.0f;
    private const float JumpVelocity = -250.0f;
    private const float Gravity = 750.0f;
    private const float WallSlideGravity = 500.0f;
    private const float WallJumpVelocity = -200.0f;
    private const float FloorSnapLengthValue = 2.5f;

    // Nodes
    private AnimatedSprite2D _animation;
    private RayCast2D _leftWallDetect, _rightWallDetect;
    private Area2D _playerArea;
    private WeaponManager _weaponManager;

    // State enum
    private enum State
    {
        Idle, 
        Run, 
        Jump, 
        WallSlide, 
        Fall, 
        WallJump
    };

    // Variables
    private State _currentState;
    private float _wallJumpDirection;
    private Vector2 _velocity;
    private Vector2 _muzzlePosition;
    private float _spriteDirection;
    private bool _onCooldown;
    private string _currentWeapon;

    public override void _Ready()
    {
        // Get the child nodes
        _animation = GetNode<AnimatedSprite2D>("sprite");
        _leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
        _rightWallDetect = GetNode<RayCast2D>("right_wall_detect");
        _playerArea = GetNode<Area2D>("player_area");
        _weaponManager = GetNode<WeaponManager>("weapon_manager");
        
        // Set z index high so player is in front of all other objects
        ZIndex = 100;

        // Keeps player snapped to floors on slopes
        FloorSnapLength = FloorSnapLengthValue;

        // Enable constant speed on floors and slopes
        FloorConstantSpeed = true;

        // Set velocity
        _velocity = Velocity;
        
        // Set default direction
        _spriteDirection = 1.0f;
        
        // Animation to play on ready
        _animation.FlipH = true;
        _animation.Play("idle");
        
        // Signals/Actions
        _playerArea.AreaEntered += OnAreaEntered;

        // // Hide mouse cursor when playing game
        // Input.SetMouseMode(Input.MouseModeEnum.Hidden);
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

    private void EnterState()
    {
        switch (_currentState)
        {
            case State.Idle:
                _velocity.Y = 0;
                _animation.Play("idle");
                break;
            
            case State.Run:
                _animation.Play("run");
                break;
            
            case State.Jump:
                _velocity.Y = JumpVelocity;
                _animation.Play("jump");
                break;
            
            case State.WallSlide:
                if (_weaponManager != null)
                {
                    _weaponManager.WallSlide = true;
                }
                
                _animation.Play("wall_slide");
                break;
            
            case State.Fall:
                _animation.Play("fall");
                break;
            
            case State.WallJump:
                _velocity.Y = WallJumpVelocity;
                _animation.Play("jump");
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
                if (_weaponManager != null)
                {
                    _weaponManager.WallSlide = false;
                }
                break;
            case State.Fall:
                break;
            case State.WallJump:
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
            _spriteDirection = -1;
        }
        else if (directionX > 0)
        {
            _animation.FlipH = true;
            _spriteDirection = 1;
        }

        if (_currentWeapon != null)
        {
            _weaponManager.SpriteDirection = _spriteDirection;
        }
        
    }
    public override void _PhysicsProcess(double delta)
    {
        UpdateState((float)delta);
    }
    
    // Signal methods
    private void OnAreaEntered(Node2D area)
    {
        if (area.Name.ToString().ToLower() == "weapon_pickups")
        {
            // If the area entered is weapon_pickups, get weapon's parent node name and equip the weapon
            _currentWeapon = area.GetParent().Name.ToString();
            _weaponManager.EquipWeapon(_currentWeapon);
        }
    }

}