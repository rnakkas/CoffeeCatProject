using Godot;

//TODO: Fix issue with shooting continuously and multiple times when shoot button is pressed or held
namespace CoffeeCatProject.Weapons.Equipped.Scripts;

public partial class WeaponShotgun : Node2D
{
	// Constants
	private const float BulletAngle = 3.5f;
	private const float CoolDownTime = 0.9f;
	
	// Exports
	[Export]
	private ShootingComponent ShootingComponent { get; set; }
	
	[Export]
	private Timer CooldownTimer { get; set; }
	
	[Export]
	private BulletShotgun BulletShotgun { get; set; }
	
	[Export]
	private Marker2D Muzzle { get; set; }
	
	// Nodes
	// private Marker2D _muzzle;
	private WeaponManager _weaponManager;
	
	// Packed scene: bullets
	private readonly PackedScene _bulletShotgun = 
		ResourceLoader.Load<PackedScene>("res://Weapons/Equipped/Scenes/bullet_shotgun.tscn");
	
	// Variables
	private Vector2 _muzzlePosition;
	private bool _shoot;
	
	public override void _Ready()
	{
		// Get the child nodes
		_weaponManager = GetNode<WeaponManager>("../weapon_manager");
		
		BulletShotgun.SetVisible(false);
		
		// Set muzzle position
		_muzzlePosition = Muzzle.Position;
		
		// Set timer values
		CooldownTimer.SetOneShot(true);
		CooldownTimer.SetWaitTime(CoolDownTime);
		
		// Connect to signals
		ShootingComponent.ShootingStart += OnShootingStart;
		ShootingComponent.ShootingEnd += OnShootingEnd;
	}
	
	public override void _Process(double delta)
	{
		if (_shoot)
		{
			ShootBullets();
		}
	}
	
	private void ShootBullets()
	{
		// Instantiate the bullet scene, cast PackedScene as type PlayerBullet node
		var bulletInstance1 = (BulletShotgun)_bulletShotgun.Instantiate();
		var bulletInstance2 = (BulletShotgun)_bulletShotgun.Instantiate();
		var bulletInstance3= (BulletShotgun)_bulletShotgun.Instantiate();

		// Set bullet's direction based on player's direction
		bulletInstance1.Direction = _weaponManager.SpriteDirection;
		bulletInstance2.Direction = _weaponManager.SpriteDirection;;
		bulletInstance3.Direction = _weaponManager.SpriteDirection;;
                
		// Set bullets rotations
		bulletInstance2.RotationDegrees = BulletAngle;
		bulletInstance3.RotationDegrees = -BulletAngle;
                
		// Set bullet's location to muzzle location, flip muzzle position when sprite is flipped
		if (_weaponManager.SpriteDirection < 0)
		{
			Muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
		}
                
		if (_weaponManager.SpriteDirection > 0)
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
	}
	
	// Signal connection methods
	private void OnShootingStart()
	{
		_shoot = true;
	}

	private void OnShootingEnd()
	{
		_shoot = false;
	}
}
