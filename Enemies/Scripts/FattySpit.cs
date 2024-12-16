using Godot;

namespace CoffeeCatProject.Enemies.Scripts;

public partial class FattySpit : Area2D
{
	// Constants
	private const float Speed = 250f;
	
	// Nodes
	private AnimatedSprite2D _sprite;
	
	// Variables
	public Vector2 Target {get; set;}
	private bool _hitStatus;
	
	public override void _Ready()
	{
		// Get the nodes
		_sprite = GetNode<AnimatedSprite2D>("sprite");
		
		// Flip sprite based on direction
		FlipSprite();
		
		_sprite.Play("fly");

		// Area2D signals
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		
		
	}

	public override async void _PhysicsProcess(double delta)
	{
		// Flip sprite based on direction
		FlipSprite();

		if (!_hitStatus)
		{
			MoveLocalX(Speed * (float)delta * Target.X);
			MoveLocalY(Speed * (float)delta * Target.Y);
		}
		else
		{
			MoveLocalX(0);
			MoveLocalY(0);
			_sprite.Play("hit");
			// await ToSignal(_sprite, "animation_finished");
			QueueFree();
		}
	}
	
	private void FlipSprite()
	{
		// Flip sprite based on direction
		if (GlobalPosition.DirectionTo(Target).X < 0)
		{
			_sprite.FlipH = false;
		}

		if (GlobalPosition.DirectionTo(Target).X > 0)
		{
			_sprite.FlipH = true;
		}
		
	}
	
	// Projectile's area2d signals
	private void OnBodyEntered(Node body)
	{
		if (body is TileMapLayer)
		{
			_hitStatus = true;
		}
	}

	private void OnAreaEntered(Node area)
	{
		if (area.Name == "player_hitbox")
		{
			_hitStatus = true;
		}
	}
}
