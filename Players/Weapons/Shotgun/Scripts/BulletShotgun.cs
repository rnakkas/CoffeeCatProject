using CoffeeCatProject.Players.WeaponManager.Scripts;
using Godot;

namespace CoffeeCatProject.Players.Weapons.Shotgun.Scripts;
public partial class BulletShotgun : Area2D
{
	// Constants
	private const float BulletSpeed = 800f;
	
	// Nodes
	[Export] private AnimatedSprite2D Sprite {get; set;}
	private WeaponManagerScript _weaponManagerNodeScript;
	private string _weaponManagerNodeName = "weapon_manager";

	// Variables
	public float Direction {get; set;}
	
	private Vector2 _directionVector = Vector2.Zero;
	private bool _hitStatus;
	
	public override void _Ready()
	{
		// Set the bullet's direction
		SetBulletDirection();
		
		// Flip sprite based on direction
		FlipSprite();
		
		Sprite.Play("fly");

		// Connect signals
		BodyEntered += OnBodyEntered;
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

	private void SetBulletDirection()
	{
		foreach (var child in GetParent().GetChild(0).GetChildren())
		{
			if (child == this ||
			    !child.HasMeta("role") ||
			    child.GetMeta("role").ToString() != "Player" ||
			    !child.HasNode(_weaponManagerNodeName)) 
				continue;
			
			_weaponManagerNodeScript = child.GetNode<WeaponManagerScript>(_weaponManagerNodeName);
		}
		Direction = _weaponManagerNodeScript.SpriteDirection;
	}
	
	// Connect signals methods
	private void OnBodyEntered(Node body)
	{
		if (body is TileMapLayer || body.Name.ToString().ToLower().Contains("enemy"))
		{
			_hitStatus = true;
		}
	}
}