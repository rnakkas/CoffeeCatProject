using CoffeeCatProject.Player.Scripts;
using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;
public partial class WeaponShotgun : Node2D
{
	// Constants
	private const float BulletAngle = 3.5f;
	
	// Nodes
	private AnimatedSprite2D _animation;
	private Marker2D _muzzle;
	private Timer _shotCooldown;
	
	// Packed scene: bullets
	private readonly PackedScene _playerBullet = 
		ResourceLoader.Load<PackedScene>("res://Player/Scenes/player_bullet.tscn");
	
	// State enum
	private enum State
	{
		Idle,
		WallSlide,
		Shoot
	}
	
	// Variables
	private State _currentState;
	private Vector2 _muzzlePosition;
	private bool _onCooldown;
	private float _direction;
	public float Direction
	{
		get => _direction;
		set => _direction = value;
	}
	
	private bool _wallSlide;
	public bool WallSlide
	{
		get => _wallSlide;
		set => _wallSlide = value;
	}
	
	public override void _Ready()
	{
		// Get the child nodes
		_animation = GetNode<AnimatedSprite2D>("sprite");
		_muzzle = GetNode<Marker2D>("marker");
		_shotCooldown = GetNode<Timer>("shotCoolDownTimer");
		
		// Set muzzle position
		_muzzlePosition = _muzzle.Position;
		
		// Set default direction
		_direction = 1.0f;
		
		// Set timer values
		_shotCooldown.SetOneShot(true);
		_shotCooldown.SetWaitTime(0.9);
		
		// Animation to play on ready
		// _animation.Play("idle");
		// FlipSprite();
		
		// Signals/Actions
		_shotCooldown.Timeout += OnTimerTimeout;

	}
	
	// // State Machine
	// private void SetState(State newState)
	// {
	// 	if (newState == _currentState)
	// 		return;
	// 	
	// 	ExitState();
	// 	_currentState = newState;
	// 	EnterState();
	// }
	//
	// private void ExitState()
	// {
	// 	switch (_currentState)
	// 	{
	// 		case State.Idle:
	// 			break;
	// 		case State.WallSlide:
	// 			break;
	// 		case State.Shoot:
	// 			break;
	// 	}
	// }
	//
	// private async void EnterState()
	// {
	// 	switch (_currentState)
	// 	{
	// 		case State.Idle:
	// 			if (_animation.Animation == "shoot")
	// 			{
	// 				GD.Print("shoot/reload to idle");
	// 				await ToSignal(_animation, "animation_finished");
	// 			}
	// 			_animation.Play("idle");
	// 			break;
	// 		
	// 		case State.WallSlide:
	// 			_animation.Play("wall_slide");
	// 			break;
	// 		
	// 		case State.Shoot:
	// 			_onCooldown = true;
	// 			_shotCooldown.Start();
	// 			_animation.Play("shoot");
	// 			break;
	// 	}
	// }
	//
	// private void UpdateState()
	// {
	// 	switch (_currentState)
	// 	{
	// 		case State.Idle:
	// 			if (Input.IsActionJustPressed("shoot") && !_onCooldown)
	// 			{
	// 				SetState(State.Shoot);
	// 			}
	// 			else if (_wallSlide)
	// 			{
	// 				SetState(State.WallSlide);
	// 			}
	// 			
	// 			break;
	// 		
	// 		case State.WallSlide:
	// 			if (!_wallSlide)
	// 			{
	// 				SetState(State.Idle);
	// 			}
	//
	// 			break;
	// 		
	// 		case State.Shoot:
	// 			
	// 			Shoot();
	// 				
	// 			if (_wallSlide)
	// 			{
	// 				SetState(State.WallSlide);
	// 			}
	// 			else
	// 			{
	// 				SetState(State.Idle);
	// 			}
	// 			
	// 			break;
	// 	}
	// }
	//
	// public override void _Process(double delta)
	// {
	// 	UpdateState();
	// 	FlipSprite();
	// }
	//
	// private void FlipSprite()
	// {
	// 	// Flip sprite based on direction
	// 	if (_direction < 0)
	// 	{
	// 		_animation.FlipH = false;
	// 	}
	//
	// 	if (_direction > 0)
	// 	{
	// 		_animation.FlipH = true;
	// 	}
	// }

	private void Shoot()
	{
		// Instantiate the bullet scene, cast PackedScene as type PlayerBullet node
		var bulletInstance1 = (PlayerBullet)_playerBullet.Instantiate();
		var bulletInstance2 = (PlayerBullet)_playerBullet.Instantiate();
		var bulletInstance3= (PlayerBullet)_playerBullet.Instantiate();

		// Set bullet's direction based on player's direction
		bulletInstance1.Direction = _direction;
		bulletInstance2.Direction = _direction;
		bulletInstance3.Direction = _direction;
                
		// Set bullets rotations
		bulletInstance2.RotationDegrees = BulletAngle;
		bulletInstance3.RotationDegrees = -BulletAngle;
                
		// Set bullet's location to muzzle location, flip muzzle position when sprite is flipped
		if (_direction < 0)
		{
			_muzzle.Position = _muzzlePosition;
		}
                
		if (_direction > 0)
		{
			_muzzle.Position = -_muzzlePosition;
		}
                
		bulletInstance1.GlobalPosition = _muzzle.GlobalPosition;
		bulletInstance2.GlobalPosition = _muzzle.GlobalPosition;
		bulletInstance3.GlobalPosition = _muzzle.GlobalPosition;
                
		// Add bullet scene to scene tree
		GetTree().Root.AddChild(bulletInstance1);
		GetTree().Root.AddChild(bulletInstance2);
		GetTree().Root.AddChild(bulletInstance3);
	}
	
	// Signals/Actions methods
	
	private void OnTimerTimeout()
	{
		_onCooldown = false;
	}
}
