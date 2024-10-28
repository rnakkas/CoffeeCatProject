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

    private enum State { Idle, Run, Jump, WallSlide, Fall, WallJump };

    private State _currentState;

    private float _wallJumpDirection;

    private Vector2 _velocity;

    public override void _Ready()
    {
        _animation = GetNode<AnimatedSprite2D>("sprite");
        _leftWallDetect = GetNode<RayCast2D>("left_wall_detect");
        _rightWallDetect = GetNode<RayCast2D>("right_wall_detect");

        _animation.FlipH = true;
        SetState(State.Idle);
        
        // Set z index high so player is in front of all other objects
        ZIndex = 100;

        // Keeps player snapped to floors on slopes
        this.FloorSnapLength = FloorSnapLengthValue;

        // Enable constant speed on floors and slopes
        this.FloorConstantSpeed = true;

        // Set velocity
        _velocity = Velocity;
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
        }
        else if (directionX > 0)
        {
            _animation.FlipH = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateState((float)delta);
    }
}