using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class Enemies : CharacterBody2D
{
	public float Speed = 200.0f;
	public const float Gravity = 750.0f;

	private  AnimatedSprite2D _animation;

	protected enum State
	{ 
		Idle, 
		Run, 
		Attack, 
		Hurt, 
		Death
	};
	protected State CurrentState;

	private Vector2 _velocity;

	protected AnimatedSprite2D GetSprite()
	{
		return GetNode<AnimatedSprite2D>("sprite");
	}

	protected void SetState(State newState)
	{
		// Cannot exit death state
		if (newState == CurrentState || CurrentState == State.Death) 
		{
			return;
		}

		ExitState();
		CurrentState = newState;
		EnterState();
	}

	private void ExitState()
	{
		switch (CurrentState)
		{
			case State.Idle:
				break;
			case State.Run:
				break;
			case State.Attack:
				break;
			case State.Hurt:
				break;
		}
	}
	private void EnterState()
	{
		switch (CurrentState)
		{
			case State.Idle:
				GD.Print("entered: IDLE");
				_animation.Play("idle");
				break;
			case State.Run:
				GD.Print("entered: RUN");
				_animation.Play("run");
				break;
			case State.Attack:
				GD.Print("entered: ATTACK");
				_animation.Play("attack");
				break;
			case State.Hurt:
				GD.Print("entered: HURT");
				_animation.Play("hurt");
				break;
			case State.Death:
				GD.Print("entered: DEATH");
				_animation.Play("death");
				break;
		}
	}

	protected void UpdateState(float delta)
	{
		if (!IsOnFloor())
		{
			_velocity.Y += Gravity * delta;
			Velocity = _velocity;
			MoveAndSlide();
		}

	}
}