using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;
public partial class BulletShotgun : Area2D
{
	// Constants
	private const float BulletSpeed = 100f;
	
	// Nodes
	private AnimatedSprite2D _animation;

	// Variables
	private float _direction;
	public float Direction
	{
		get => _direction;
		set => _direction = value;
	}
	
	private Vector2 _directionVector = Vector2.Zero;
	private bool _hitStatus;
	
	public override void _Ready()
	{
		// Get nodes
		_animation = GetNode<AnimatedSprite2D>("sprite");
		
		// Flip sprite based on direction
		FlipSprite();
		
		_animation.Play("fly");

		// Connect signals
		BodyEntered += OnBodyEntered;
	}

	public override async void _PhysicsProcess(double delta)
	{
		// Flip sprite based on direction
		FlipSprite();

		if (!_hitStatus)
		{
			MoveLocalX(BulletSpeed * (float)delta * _direction);
		}
		else
		{
			MoveLocalX(0);
			_animation.Play("hit");
			await ToSignal(_animation, "animation_finished");
			QueueFree();
		}
	}

	private void FlipSprite()
	{
		// Flip sprite based on direction
		if (_direction < 0)
		{
			_animation.FlipH = false;
		}

		if (_direction > 0)
		{
			_animation.FlipH = true;
		}
		
	} 
	
	// Connect signals methods
	private async void OnBodyEntered(Node body)
	{
		if (body is TileMapLayer || body.Name.ToString().ToLower().Contains("enemy"))
		{
			_hitStatus = true;
		}
	}
}