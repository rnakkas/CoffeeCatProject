using Godot;

namespace CoffeeCatProject.Players.Weapons.Shotgun.Scripts;
public partial class BulletShotgun : Area2D
{
	// Constants
	private const float BulletSpeed = 900f;
	
	// Nodes
	[Export] private AnimatedSprite2D Sprite {get; set;}

	// Variables
	public float Direction {get; set;}
	
	private Vector2 _directionVector = Vector2.Zero;
	private bool _hitStatus;
	
	public override void _Ready()
	{
		// Flip sprite based on direction
		FlipSprite();
		
		Sprite.Play("fly");

		// Connect signals
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}

	public override async void _PhysicsProcess(double delta)
	{
		// Flip sprite based on direction
		FlipSprite();

		if (!_hitStatus)
		{
			MoveLocalX(BulletSpeed * (float)delta * Direction);
		}
		else
		{
			MoveLocalX(0);
			Sprite.Play("hit");
			await ToSignal(Sprite, "animation_finished");
			QueueFree();
		}
	}
	
	private void FlipSprite()
	{
		// Flip sprite based on direction
		if (Direction < 0)
		{
			Sprite.FlipH = false;
		}

		if (Direction > 0)
		{
			Sprite.FlipH = true;
		}
		
	}
	
	// Connect signals methods
	private void OnBodyEntered(Node body)
	{
		if (body is TileMapLayer)
		{
			_hitStatus = true;
		}
	}

	private void OnAreaEntered(Node area)
	{
		if (area.Name == "enemy_hurtbox")
		{
			_hitStatus = true;
		}
	}
}