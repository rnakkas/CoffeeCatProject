using System;
using Godot;

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
    private const float BulletAngle = 3.5f;

    // Nodes
    private AnimatedSprite2D _animation;
    private RayCast2D _leftWallDetect, _rightWallDetect;
    private Marker2D _muzzle;
    private Timer _shotCooldown;
    private Area2D _playerArea;
    
    // Packed scene: bullets
    private readonly PackedScene _playerBullet = 
        ResourceLoader.Load<PackedScene>("res://Player/Scenes/player_bullet.tscn");
    
    // Packed scene: weapons
    private readonly PackedScene _weaponShotgun = 
        ResourceLoader.Load<PackedScene>("res://Player/Scenes/weapon_shotgun.tscn");

    // State enum
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

    // Variables
    private State _currentState;
    private float _wallJumpDirection;
    private Vector2 _velocity;
    private Vector2 _muzzlePosition;
    private float _spriteDirection;
    private bool _onCooldown;
    private string _currentWeapon;
    private WeaponShotgun _weaponInstance;

    public override void _Ready()
    {
        // Get the child nodes
        _animation = GetNode<AnimatedSprite2D>("sprite");
        _leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
        _rightWallDetect = GetNode<RayCast2D>("right_wall_detect");
        _muzzle = GetNode<Marker2D>("marker");
        _shotCooldown = GetNode<Timer>("shotCoolDownTimer");
        _playerArea = GetNode<Area2D>("player_area");
        
        // Set z index high so player is in front of all other objects
        ZIndex = 100;

        // Keeps player snapped to floors on slopes
        FloorSnapLength = FloorSnapLengthValue;

        // Enable constant speed on floors and slopes
        FloorConstantSpeed = true;

        // Set velocity
        _velocity = Velocity;
        
        // Set muzzle position
        _muzzlePosition = _muzzle.Position;
        
        // Set default direction
        _spriteDirection = 1.0f;
        
        // Set timer values
        _shotCooldown.SetOneShot(true);
        _shotCooldown.SetWaitTime(0.9);
        
        // Animation to play on ready
        _animation.FlipH = true;
        _animation.Play("idle");
        
        // Signals/Actions
        // _shotCooldown.Timeout += OnTimerTimeout;
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
                _onCooldown = true;
                _shotCooldown.Start();
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
                else if (Input.IsActionJustPressed("shoot") && !_onCooldown)
                {
                    SetState(State.Shoot);
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
                else if (Input.IsActionJustPressed("shoot") && !_onCooldown)
                {
                    SetState(State.Shoot);
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
                    else if (Input.IsActionJustPressed("shoot") && !_onCooldown)
                    {
                        SetState(State.Shoot);
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

                    if (Input.IsActionJustPressed("shoot") && !_onCooldown)
                    {
                        SetState(State.Shoot);
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
                    else if (Input.IsActionJustPressed("shoot") && !_onCooldown)
                    {
                        SetState(State.Shoot);
                    }
                }

                Velocity = _velocity;
                MoveAndSlide();
                break;

            case State.Shoot:
                FlipSprite(direction.X);
                
                //TODO: Remove
                // // Instantiate the bullet scene, cast PackedScene as type PlayerBullet node
                // var bulletInstance1 = (PlayerBullet)_playerBullet.Instantiate();
                // var bulletInstance2 = (PlayerBullet)_playerBullet.Instantiate();
                // var bulletInstance3= (PlayerBullet)_playerBullet.Instantiate();
                //
                // // Set bullet's direction based on player's direction
                // bulletInstance1.Direction = _spriteDirection;
                // bulletInstance2.Direction = _spriteDirection;
                // bulletInstance3.Direction = _spriteDirection;
                //
                // // Set bullets rotations
                // bulletInstance2.RotationDegrees = BulletAngle;
                // bulletInstance3.RotationDegrees = -BulletAngle;
                //
                // // Set bullet's location to muzzle location, flip muzzle position when sprite is flipped
                // if (_spriteDirection < 0)
                // {
                //     _muzzle.Position = _muzzlePosition;
                // }
                //
                // if (_spriteDirection > 0)
                // {
                //     _muzzle.Position = -_muzzlePosition;
                // }
                //
                // bulletInstance1.GlobalPosition = _muzzle.GlobalPosition;
                // bulletInstance2.GlobalPosition = _muzzle.GlobalPosition;
                // bulletInstance3.GlobalPosition = _muzzle.GlobalPosition;
                //
                // // Add bullet scene to scene tree
                // GetTree().Root.AddChild(bulletInstance1);
                // GetTree().Root.AddChild(bulletInstance2);
                // GetTree().Root.AddChild(bulletInstance3);
                
                if (!IsOnFloor())
                {
                    _velocity.Y += Gravity * delta;
                }
                
                if (direction.X != 0)
                {
                    SetState(State.Run);
                }
                else if (direction.X == 0 && Input.IsActionJustPressed("shoot") && !_onCooldown)
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
            _spriteDirection = -1;
        }
        else if (directionX > 0)
        {
            _animation.FlipH = true;
            _spriteDirection = 1;
        }

        if (_weaponInstance != null)
        {
            _weaponInstance.Direction = _spriteDirection;
        }
        
    }
    public override void _PhysicsProcess(double delta)
    {
        UpdateState((float)delta);
    }
    
    // Equip weapon based on which weapon was picked up
    private void EquipWeapon(string weaponName)
    {
        switch (weaponName)
        {
            case not null when weaponName.Contains("shotgun"):
                // Instantiate the weapon scene, set direction based on player's direction, add scene as child of player
                _weaponInstance = (WeaponShotgun)_weaponShotgun.Instantiate();
                _weaponInstance.Direction = _spriteDirection;
                AddChild(_weaponInstance);
                break;
            
            case not null when weaponName.Contains("machine_gun"):
                GD.Print("machine gun");
                break;
            
            default:
                throw new Exception("weapon type " + weaponName + "not found");
        }
    }
    
    // Signals/Actions methods
    
    // private void OnTimerTimeout()
    // {
    //     _onCooldown = false;
    // }
    
    private void OnAreaEntered(Node2D area)
    {
        // If the area entered is weapon, get weapon's node name
        if (area.Name.ToString().ToLower().Contains("weapon"))
        {
            _currentWeapon = area.Name.ToString();
            EquipWeapon(_currentWeapon);
        }
    }
}