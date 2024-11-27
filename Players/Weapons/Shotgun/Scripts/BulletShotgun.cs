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
	private float _direction;
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
			MoveLocalX(BulletSpeed * (float)delta * _direction);
		}
		else
		{
			MoveLocalX(0);
			Sprite.Play("hit");
			await ToSignal(Sprite, "animation_finished");
			QueueFree();
		}
	}

	//TODO: create a bullet animation or movement component and move this code there
	private void FlipSprite()
	{
		// Flip sprite based on direction
		if (_direction < 0)
		{
			Sprite.FlipH = false;
		}

		if (_direction > 0)
		{
			Sprite.FlipH = true;
		}
		
	}

	//TODO: create a bullet animation or movement component and move this code there
	private void SetBulletDirection()
	{
		GD.Print("parent " + GetParent().GetChild(0));
		foreach (Node child in GetParent().GetChild(0).GetChildren())
		{
			if (child == this ||
			    !child.HasMeta("role") ||
			    child.GetMeta("role").ToString() != "Player") 
				continue;
			if (!child.HasNode(_weaponManagerNodeName)) 
				continue;
			_weaponManagerNodeScript = child.GetNode<WeaponManagerScript>(_weaponManagerNodeName);
			GD.Print("node: " + _weaponManagerNodeScript);
		}
		
		_direction = _weaponManagerNodeScript.SpriteDirection;
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