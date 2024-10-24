using Godot;
using System;

public partial class Enemies : CharacterBody2D
{
	public float Speed = 200.0f;
    public const float Gravity = 750.0f;

	public AnimatedSprite2D animation;

	public enum State { IDLE, RUN, SHOOT, HURT, DEATH};
	public State currentState;

	Vector2 velocity;

    public AnimatedSprite2D GetSprite()
    {
		return GetNode<AnimatedSprite2D>("sprite");
    }

    public void SetState(State newState)
	{
		if (newState == currentState || currentState == State.DEATH) // Cannot exit death state
		{
			return;
		}

		ExitState();
		currentState = newState;
		EnterState();
	}

    public void ExitState()
    {
        switch (currentState)
		{
			case State.IDLE:
				break;
			case State.RUN:
				break;
			case State.SHOOT:
				break;
			case State.HURT:
				break;
		}
    }
    public void EnterState()
    {
		switch (currentState)
		{
			case State.IDLE:
				GD.Print("entered: IDLE");
				animation.Play("idle");
				break;
			case State.RUN:
                GD.Print("entered: RUN");
                animation.Play("run");
				break;
			case State.SHOOT:
                GD.Print("entered: SHOOT");
                animation.Play("shoot");
				break;
			case State.HURT:
                GD.Print("entered: HURT");
                animation.Play("hurt");
				break;
			case State.DEATH:
                GD.Print("entered: DEATH");
                animation.Play("death");
				break;
		}
    }

	public void UpdateState(float delta)
	{
		if (!IsOnFloor())
		{
			velocity.Y += Gravity * delta;
			Velocity = velocity;
			MoveAndSlide();
		}

	}
}
