using CoffeeCatProject.Player.Components.Scripts;
using Godot;
using CoffeeCatProject.Player.WeaponManager.Scripts;

//TODO: Fix issue with shooting continuously and multiple times when shoot button is pressed or held
namespace CoffeeCatProject.Player.Weapons.Shotgun.Scripts;

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
	private readonly PackedScene _bulletShotgun = 
		ResourceLoader.Load<PackedScene>("res://Player/Weapons/Shotgun/Scenes/bullet_shotgun.tscn");
	
	// Variables
	private Vector2 _muzzlePosition;
	private bool _shoot;
	
	public override void _Ready()
	{
		// Get the child nodes
		_weaponManagerScriptNode = GetParent().GetNode<WeaponManagerScript>("weapon_manager");
		_shootingComponent = GetParent().GetNode<ShootingComponent>("ShootingComponent");
		
		// Set muzzle position
		_muzzlePosition = Muzzle.Position;
		
		// Set timer values
		CooldownTimer.SetOneShot(true);
		CooldownTimer.SetWaitTime(CoolDownTime);
		
		// Connect signals
		_shootingComponent.ShootingStart += OnShootingStart;
		CooldownTimer.Timeout += OnCooldownTimeeout;

	}
	
	public override void _Process(double delta)
	{
		// Flip weapon sprite based on player's direction5
		if (_weaponManagerScriptNode.SpriteDirection < 0)
		{
			Sprite.FlipH = false;
		}

		if (_weaponManagerScriptNode.SpriteDirection > 0)
		{
			Sprite.FlipH = true;
		}
	}
	
	private void ShootBullets()
	{
		// Instantiate the bullet scene, cast PackedScene as type PlayerBullet node
		var bulletInstance1 = (BulletShotgun)_bulletShotgun.Instantiate();
		var bulletInstance4 = _bulletShotgun.Instantiate<AnimatedSprite2D>();
		var bulletInstance2 = (BulletShotgun)_bulletShotgun.Instantiate();
		var bulletInstance3= (BulletShotgun)_bulletShotgun.Instantiate();

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
	private void OnShootingStart()
	{
		CooldownTimer.Start();
		_shootingComponent.OnCooldown = true;
		_shootingComponent.BulletAngle = BulletAngle;
		_shootingComponent.BulletCount = BulletCount;
		_shootingComponent.MuzzlePosition = _muzzlePosition;
	}

	private void OnCooldownTimeeout()
	{
		_shootingComponent.OnCooldown = false;
	}
}
