using Godot;

//TODO: Fix issue with shooting continuously and multiple times when shoot button is pressed or held
namespace CoffeeCatProject.Weapons.Equipped.Scripts;

public partial class WeaponShotgun : Node2D
{
	// Constants
	private const float BulletAngle = 3.5f;
	private const float CoolDownTime = 0.9f;
	
	// Nodes
	private Marker2D _muzzle;
	private ShootingComponent _shootingComponent;
	private WeaponManager _weaponManager;
	private Timer _cooldownTimer;
	
	// Packed scene: bullets
	private readonly PackedScene _bulletShotgun = 
		ResourceLoader.Load<PackedScene>("res://Weapons/Equipped/Scenes/bullet_shotgun.tscn");
	
	// Variables
	private Vector2 _muzzlePosition;
	private bool _shoot;
	
	public override void _Ready()
	{
		// Get the child nodes
		_muzzle = GetNode<Marker2D>("marker");
		_shootingComponent = GetNode<ShootingComponent>("shooting_component");
		_weaponManager = GetNode<WeaponManager>("../weapon_manager");
		_cooldownTimer = GetNode<Timer>("shotCoolDownTimer");
		
		// Set muzzle position
		_muzzlePosition = _muzzle.Position;
		
		// Set timer values
		_cooldownTimer.SetOneShot(true);
		_cooldownTimer.SetWaitTime(CoolDownTime);
		
		// Connect to signals
		_shootingComponent.ShootingStart += OnShootingStart;
		_shootingComponent.ShootingEnd += OnShootingEnd;
	}
	
	public override void _Process(double delta)
	{
		if (_shoot & !_shootingComponent.OnCooldown)
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
			_muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
		}
                
		if (_weaponManager.SpriteDirection > 0)
		{
			_muzzle.Position = new Vector2(-_muzzlePosition.X, _muzzlePosition.Y);
		}
                
		bulletInstance1.GlobalPosition = _muzzle.GlobalPosition;
		bulletInstance2.GlobalPosition = _muzzle.GlobalPosition;
		bulletInstance3.GlobalPosition = _muzzle.GlobalPosition;
                
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
