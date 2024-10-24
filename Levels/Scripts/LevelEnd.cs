using Godot;

namespace CoffeeCatProject.Levels.Scripts;

public partial class LevelEnd : Area2D
{
	private AnimatedSprite2D _animation;

	private bool _playerEntered;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("sprite");
		_animation.Play("idle");

		// Signals
		this.BodyEntered += OnBodyEntered;
		this.BodyExited += OnBodyExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PlayerInteraction();
	}

	//Signal
	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "player")
		{
			_playerEntered = true;
			_animation.Play("player_entered");
		}
	}

	// Signal
	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "player")
		{
			_playerEntered = false;
			_animation.Play("idle");
		}
	}

	private void PlayerInteraction()
	{
		if (_playerEntered && Input.IsActionJustPressed("move_up"))
		{
			GD.Print("entered");
			//TODO: scene transition to level complete scene
		}
	}
}