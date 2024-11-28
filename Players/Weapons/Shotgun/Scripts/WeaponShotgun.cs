using CoffeeCatProject.Players.Components.Scripts;
using CoffeeCatProject.Players.WeaponManager.Scripts;
using Godot;

namespace CoffeeCatProject.Players.Weapons.Shotgun.Scripts;

public partial class WeaponShotgun : Node2D
{
	// Constants
	private const float BulletAngle = 6.5f;
	private const float CoolDownTime = 1.2f;
	private const int BulletCount = 6;
	
	// Exports
	[Export] private Marker2D Muzzle { get; set; }
	[Export] private AnimatedSprite2D Sprite { get; set; }
	[Export] private ShootingComponent ShootingComponent  { get; set; }
	[Export] private WeaponAnimationComponent WeaponAnimationComponent  { get; set; }
	
	// Nodes
	private WeaponManagerScript _weaponManagerScriptNode;
	
	// Packed scene: bullets
	private readonly PackedScene _shotgunShells = 
		ResourceLoader.Load<PackedScene>("res://Players/Weapons/Shotgun/Scenes/bullet_shotgun.tscn");
	
	// Variables
	private Vector2 _muzzlePosition;
	private bool _shootingStatus;
	
	public override void _Ready()
	{
		// Get the nodes
		_weaponManagerScriptNode = GetParent().GetNode<WeaponManagerScript>("weapon_manager");
		
		// Set muzzle position
		_muzzlePosition = Muzzle.Position;

		_shootingStatus = false;
	}
	
	public override void _Process(double delta)
	{
		// Flip weapon sprite and muzzle position based on player's direction 
		if (_weaponManagerScriptNode.SpriteDirection < 0)
		{
			Sprite.FlipH = false;
			Muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
		}
		
		if (_weaponManagerScriptNode.SpriteDirection > 0)
		{
			Sprite.FlipH = true;
			Muzzle.Position = new Vector2(-_muzzlePosition.X, _muzzlePosition.Y);
		}

		// Shooting
		if ((Input.IsActionJustPressed("shoot") || Input.IsActionPressed("shoot")) && 
		    !_weaponManagerScriptNode.WallSlide)
		{
			_shootingStatus = true;
			ProvideShootingDataToComponents();
			
		}
		else if (_weaponManagerScriptNode.WallSlide)
		{
			_shootingStatus = false;
			ProvideShootingDataToComponents();
		}
		else
		{
			_shootingStatus = false;
			ProvideShootingDataToComponents();
		}
	}

	private void ProvideShootingDataToComponents()
	{
		// To shooting component
		ShootingComponent.Shooting = _shootingStatus;
		ShootingComponent.WallSlide = _weaponManagerScriptNode.WallSlide;
		ShootingComponent.CooldownTimer.SetWaitTime(CoolDownTime);
		ShootingComponent.BulletAngle = BulletAngle;
		ShootingComponent.BulletCount = BulletCount;
		ShootingComponent.MuzzlePosition = Muzzle.GlobalPosition;
		ShootingComponent.BulletScene = _shotgunShells;

		// To weapon animation component
		WeaponAnimationComponent.Shooting = _shootingStatus;
		WeaponAnimationComponent.WallSlide = _weaponManagerScriptNode.WallSlide;
		WeaponAnimationComponent.CooldownTimer.SetWaitTime(CoolDownTime);
	}

}
