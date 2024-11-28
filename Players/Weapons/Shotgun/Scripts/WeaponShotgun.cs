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
	[Export] private ShootingComponent ShootingComponent  { get; set; }
	[Export] private WeaponAnimationComponent WeaponAnimationComponent  { get; set; }
	
	// Nodes
	private WeaponManagerScript _weaponManagerScriptNode;
	
	// Packed scene: bullets
	private readonly PackedScene _shotgunShells = 
		ResourceLoader.Load<PackedScene>("res://Players/Weapons/Shotgun/Scenes/bullet_shotgun.tscn");
	
	public override void _Ready()
	{
		// Get the nodes   
		_weaponManagerScriptNode = GetParent().GetNode<WeaponManagerScript>("weapon_manager");
		
		// Set the properties for shooting and animation
		ShootingComponent.BulletScene = _shotgunShells;
		ShootingComponent.BulletAngle = BulletAngle;
		ShootingComponent.BulletCount = BulletCount;
		ShootingComponent.CooldownTimer.SetWaitTime(CoolDownTime);
		WeaponAnimationComponent.CooldownTimer.SetWaitTime(CoolDownTime);
	}
	
	public override void _Process(double delta)
	{
		// Set the properties for shooting and animation
		ShootingComponent.SpriteDirection = _weaponManagerScriptNode.SpriteDirection;
		ShootingComponent.WallSlide = _weaponManagerScriptNode.WallSlide;
		WeaponAnimationComponent.SpriteDirection = _weaponManagerScriptNode.SpriteDirection;
		WeaponAnimationComponent.WallSlide = _weaponManagerScriptNode.WallSlide;
		
		// Fuckin' SHOOT!
		ShootingComponent._Process(delta);
		WeaponAnimationComponent._Process(delta);
	}
}
