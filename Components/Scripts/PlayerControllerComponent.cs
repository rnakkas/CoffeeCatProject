using Godot;
using System;

namespace CoffeeCatProject.Components.Scripts;

// This component handles all player inputs for all possible player movement options

[GlobalClass]
public partial class PlayerControllerComponent : Node2D
{
	[Export] private CharacterBody2D _playerCharacter;
	[Export] private RunComponent _runComponent;
	[Export] private FallComponent _fallComponent;
	[Export] private JumpComponent _jumpComponent;
	[Export] private WallJumpComponent _wallJumpComponent;
	[Export] private AnimationComponent _animationComponent;
	
	private Vector2 _direction = Vector2.Zero;
	
	public override void _PhysicsProcess(double delta)
	{
		_direction = Input.GetVector(
			"move_left", 
			"move_right", 
			"move_up", 
			"move_down"
			);
		
		_animationComponent.FlipSprite(_direction.X);
		
		_runComponent?.Run((float)delta, _direction);
		_animationComponent.RunAnimation(_runComponent);
		
		_jumpComponent?.JumpAndFall((float)delta, _playerCharacter);
		_animationComponent.JumpAndFallAnimations(_jumpComponent, _fallComponent);
		
		// _fallComponent?.Fall((float)delta,_playerCharacter);
		
		
		
		
		
		
		_wallJumpComponent?.WallJump((float)delta, _playerCharacter);

	}
}
