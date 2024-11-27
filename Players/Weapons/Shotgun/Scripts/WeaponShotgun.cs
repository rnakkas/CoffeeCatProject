using CoffeeCatProject.Players.Components.Scripts;
using CoffeeCatProject.Players.WeaponManager.Scripts;
using Godot;

//TODO: Fix issue with shooting continuously and multiple times when shoot button is pressed or held
namespace CoffeeCatProject.Players.Weapons.Shotgun.Scripts;

public partial class WeaponShotgun : Node2D
{
	// Constants
	private const float BulletAngle = 3.5f;
	private const float CoolDownTime = 0.9f;
	private const int BulletCount = 3;
	
	// Exports
	[Export] private Timer CooldownTimer { get; set; }
	[Export] private Marker2D Muzzle { get; set; }
	[Export] private AnimatedSprite2D Sprite { get; set; }
	
	// Nodes
	private WeaponManagerScript _weaponManagerScriptNode;
	private ShootingComponent _shootingComponent;
	
	// Packed scene: bullets
	private readonly PackedScene _shotgunShells = 
		ResourceLoader.Load<PackedScene>("res://Players/Weapons/Shotgun/Scenes/bullet_shotgun.tscn");
	
	// Variables
	private Vector2 _muzzlePosition;
	private bool _shooting;
	
	public override void _Ready()
	{
		// Get the nodes
		_weaponManagerScriptNode = GetParent().GetNode<WeaponManagerScript>("weapon_manager");
		_shootingComponent = GetParent().GetNode<ShootingComponent>("shooting_component");
		
		// Set muzzle position
		_muzzlePosition = Muzzle.Position;
		
		// Set timer values
		CooldownTimer.SetOneShot(true);
		CooldownTimer.SetWaitTime(CoolDownTime);
		
		// Connect signals
		_shootingComponent.IsShooting += IsShooting;
		_shootingComponent.NotShooting += NotShooting;
	}
	
	public override async void _Process(double delta)
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

		if (_shooting && !_weaponManagerScriptNode.WallSlide)
		{
			Sprite.Play("shoot");
			
		}
		else if (_weaponManagerScriptNode.WallSlide)
		{
			Sprite.Play("wall_slide");
		}
		else
		{
			if (Sprite.Animation == "shoot")
			{
				await ToSignal(Sprite, "animation_finished");
			}
			Sprite.Play("idle");
		}
		
	}
	
	private void ShootBullets()
	{
		// Instantiate the bullet scene, cast PackedScene as type PlayerBullet node
		var bulletInstance1 = (BulletShotgun)_shotgunShells.Instantiate();
		var bulletInstance4 = _shotgunShells.Instantiate<AnimatedSprite2D>();
		var bulletInstance2 = (BulletShotgun)_shotgunShells.Instantiate();
		var bulletInstance3= (BulletShotgun)_shotgunShells.Instantiate();

		// Set bullet's direction based on player's direction
		bulletInstance1.Direction = _weaponManagerScriptNode.SpriteDirection;
		bulletInstance2.Direction = _weaponManagerScriptNode.SpriteDirection;
		bulletInstance3.Direction = _weaponManagerScriptNode.SpriteDirection;
                
		// Set bullets rotations
		bulletInstance2.RotationDegrees = BulletAngle;
		bulletInstance3.RotationDegrees = -BulletAngle;
                
		// Set bullet's location to muzzle location, flip muzzle position when sprite is flipped
		if (_weaponManagerScriptNode.SpriteDirection < 0)
		{
			Muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
		}
                
		if (_weaponManagerScriptNode.SpriteDirection > 0)
		{
			Muzzle.Position = new Vector2(-_muzzlePosition.X, _muzzlePosition.Y);
		}
                
		bulletInstance1.GlobalPosition = Muzzle.GlobalPosition;
		bulletInstance2.GlobalPosition = Muzzle.GlobalPosition;
		bulletInstance3.GlobalPosition = Muzzle.GlobalPosition;
                
		// Add bullet scene to scene tree
		GetTree().Root.AddChild(bulletInstance1);
		GetTree().Root.AddChild(bulletInstance2);
		GetTree().Root.AddChild(bulletInstance3);
		GetTree().Root.AddChild(bulletInstance4);
	}
	
	//// Signal connection methods
	
	// Signal connection to ShootingComponent
	private void IsShooting()
	{
		_shooting = true;
		CooldownTimer.Start();
		_shootingComponent.CooldownTimer.SetWaitTime(CoolDownTime);
		_shootingComponent.BulletAngle = BulletAngle;
		_shootingComponent.BulletCount = BulletCount;
		_shootingComponent.MuzzlePosition = Muzzle.GlobalPosition;
		_shootingComponent.BulletScene = _shotgunShells;
	}

	private void NotShooting()
	{
		_shooting = false;
	}

}
