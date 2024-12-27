using Godot;
using System;

namespace CoffeeCatProject.Components.Scripts;

// This component handles all player inputs to allow control of a character

[GlobalClass]
public partial class PlayerControllerComponent : Node2D
{
	[Export] private CharacterBody2D _playerCharacter;
	[Export] private VelocityComponent _velocityComponent;
	
	private Vector2 _direction = Vector2.Zero;

	private void Run()
	{
		if (_direction.X != 0)
		{
			
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		_direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
	}
}
