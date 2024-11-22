using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;
public partial class AnimationComponent : Node
{
	// Const
	private const string WeaponManagerPath = "../../weapon_manager";
	
	// Vars
	private bool _shooting;
	
	// Node
	private WeaponManager _weaponManager;
	
	[Export]
	private AnimatedSprite2D Animation { get; set; }
	
	[Export]
	private ShootingComponent ShootingComponent { get; set; }
	
	public override void _Ready()
	{
		// Get node
		_weaponManager = GetNode<WeaponManager>(WeaponManagerPath);
		
		// Connect signals
		ShootingComponent.ShootingStart += OnShootingStart;
		ShootingComponent.ShootingEnd += OnShootingEnd;

		Animation.Play("idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		//Flip sprite based on direction
		if (_weaponManager.SpriteDirection < 0)
		{
			Animation.FlipH = false;
		}
		else if (_weaponManager.SpriteDirection > 0)
		{
			Animation.FlipH = true;
		}
			
		//Play different animations based on shooting, idle or wall sliding
		if (_shooting)
		{
			Animation.Play("shoot");
		}

		if (!_shooting && Animation.Animation == "shoot") 
		{
			//If current animation is shoot wait for it to finish
			await ToSignal(Animation, "animation_finished");
			Animation.Play("idle");
		}
	}
	
	// Connect signals methods
	private void OnShootingStart()
	{
		_shooting = true;
	}

	private void OnShootingEnd()
	{
		_shooting = false;
	}
}
