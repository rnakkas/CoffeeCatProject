using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class Enemies : CharacterBody2D
{
	public float Speed = 200.0f;
	public const float Gravity = 750.0f;

	public AnimatedSprite2D Animation;

	public enum State { Idle, Run, Shoot, Hurt, Death};
	public State CurrentState;

	private Vector2 _velocity;

	public AnimatedSprite2D GetSprite()
	{
		return GetNode<AnimatedSprite2D>("sprite");
	}

	public void SetState(State newState)
	{
		if (newState == CurrentState || CurrentState == State.Death) // Cannot exit death state
		{
			return;
		}

		ExitState();
		CurrentState = newState;
		EnterState();
	}

	public void ExitState()
	{
		switch (CurrentState)
		{
			case State.Idle:
				break;
			case State.Run:
				break;
			case State.Shoot:
				break;
			case State.Hurt:
				break;
		}
	}
	public void EnterState()
	{
		switch (CurrentState)
		{
			case State.Idle:
				GD.Print("entered: IDLE");
				Animation.Play("idle");
				break;
			case State.Run:
				GD.Print("entered: RUN");
				Animation.Play("run");
				break;
			case State.Shoot:
				GD.Print("entered: SHOOT");
				Animation.Play("shoot");
				break;
			case State.Hurt:
				GD.Print("entered: HURT");
				Animation.Play("hurt");
				break;
			case State.Death:
				GD.Print("entered: DEATH");
				Animation.Play("death");
				break;
		}
	}

	public void UpdateState(float delta)
	{
		if (!IsOnFloor())
		{
			_velocity.Y += Gravity * delta;
			Velocity = _velocity;
			MoveAndSlide();
		}

	}
}