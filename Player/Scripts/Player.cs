using Godot;
using System;
using System.Text.RegularExpressions;
using static Godot.TextServer;
using static Godot.WebSocketPeer;

public partial class Player : CharacterBody2D
{
    public const float Speed = 170.0f;
    public const float JumpVelocity = -250.0f;
    public const float Gravity = 750.0f;
    public const float WallSlideGravity = 500.0f;
    public const float WallJumpVelocity = -200.0f;
    public const float FloorSnapLengthValue = 2.5f;

    AnimatedSprite2D Animation;
    RayCast2D LeftWallDetect, RightWallDetect;

    enum State { IDLE, RUN, JUMP, WALL_SLIDE, FALL, WALL_JUMP };
    State CurrentState;

    float WallJumpDirection;

    Vector2 velocity;

    public override void _Ready()
    {
        Animation = GetNode<AnimatedSprite2D>("sprite");
        LeftWallDetect = GetNode<RayCast2D>("left_wall_detect");
        RightWallDetect = GetNode<RayCast2D>("right_wall_detect");

        Animation.FlipH = true;
        SetState(State.IDLE);

        // Keeps player snapped to floors on slopes
        this.FloorSnapLength = FloorSnapLengthValue;

        // Enable constant speed on floors and slopes
        this.FloorConstantSpeed = true;

        // Set velocity
        velocity = Velocity;
    }

    // State Machine
    private void SetState(State NewState)
    {
        if (NewState == CurrentState)
            return;

        ExitState();
        CurrentState = NewState;
        EnterState();
    }

    private void EnterState()
    {
        switch (CurrentState)
        {
            case State.IDLE:
                velocity.Y = 0;
                Animation.Play("idle");
                break;
            case State.RUN:
                Animation.Play("run");
                break;
            case State.JUMP:
                velocity.Y = JumpVelocity;
                Animation.Play("jump");
                break;
            case State.WALL_SLIDE:
                Animation.Play("wall_slide");
                break;
            case State.FALL:
                Animation.Play("fall");
                break;
            case State.WALL_JUMP:
                velocity.Y = JumpVelocity;
                Animation.Play("jump");
                break;

        }
    }

    private void ExitState()
    {
        switch (CurrentState)
        {
            case State.IDLE:
                break;
            case State.RUN:
                break;
            case State.JUMP:
                break;
            case State.WALL_SLIDE:
                break;
            case State.FALL:
                break;
            case State.WALL_JUMP:
                break;
        }
    }

    private void UpdateState(float delta)
    {
        Vector2 Direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        switch (CurrentState)
        {
            case State.IDLE:
                if (Direction.X != 0)
                {
                    SetState(State.RUN);
                }
                else if (Input.IsActionJustPressed("jump") && IsOnFloor())
                {
                    SetState(State.JUMP);
                }
                else if (!IsOnFloor())
                {
                    SetState(State.FALL);
                }
                break;

            case State.RUN:
                velocity.X = Direction.X * Speed;
                FlipSprite(Direction.X);

                if (Direction.X == 0)
                {
                    SetState(State.IDLE);
                }
                else if (Input.IsActionJustPressed("jump") && IsOnFloor())
                {
                    SetState(State.JUMP);
                }
                else if (!IsOnFloor())
                {
                    SetState(State.FALL);
                }

                Velocity = velocity;
                MoveAndSlide();
                break;

            case State.JUMP:
                velocity.X = Direction.X * Speed;
                FlipSprite(Direction.X);

                if (!IsOnFloor())
                {
                    velocity.Y += Gravity * delta;
                    if (Velocity.Y > 0)
                    {
                        SetState(State.FALL);
                    }
                    // Only detect wall tile (collision layer 5)
                    else if (LeftWallDetect.IsColliding() || RightWallDetect.IsColliding())
                    {
                        SetState(State.WALL_SLIDE);
                    }
                }

                Velocity = velocity;
                MoveAndSlide();
                break;

            case State.FALL:
                velocity.X = Direction.X * Speed;
                FlipSprite(Direction.X);

                if (IsOnFloor())
                {
                    SetState(State.IDLE);
                }
                // Only detect wall tile (collision layer 5)
                else if (LeftWallDetect.IsColliding() || RightWallDetect.IsColliding())
                {
                    SetState(State.WALL_SLIDE);
                }
                else
                {
                    velocity.Y += Gravity * delta;
                }

                Velocity = velocity;
                MoveAndSlide();
                break;

            case State.WALL_SLIDE:
                velocity.Y += WallSlideGravity * delta;

                if (Input.IsActionJustPressed("jump"))
                {
                    if (LeftWallDetect.IsColliding())
                    {
                        WallJumpDirection = 1.0f;
                    }
                    else if (RightWallDetect.IsColliding())
                    {
                        WallJumpDirection = -1.0f;
                    }

                    SetState(State.WALL_JUMP);
                    
                }
                else if (IsOnFloor())
                {
                    SetState(State.IDLE);
                }

                Velocity = velocity;
                MoveAndSlide();
                break;

            case State.WALL_JUMP:
                velocity.X = WallJumpDirection * Speed;
                FlipSprite(WallJumpDirection);

                if (!IsOnFloor())
                {
                    velocity.Y += Gravity * delta;
                    if (velocity.Y > 0)
                    {
                        SetState(State.FALL);
                    }
                }

                Velocity = velocity;
                MoveAndSlide();
                break;
        }
    }

    private void FlipSprite(float DirectionX)
    {
        if (DirectionX < 0)
        {
            Animation.FlipH = false;
        }
        else if (DirectionX > 0)
        {
            Animation.FlipH = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateState((float)delta);
    }
}