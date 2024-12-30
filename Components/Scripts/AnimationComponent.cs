using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component animates the characters based on their actions or statuses

[GlobalClass]
public partial class AnimationComponent : Node2D
{
	[Export] private AnimatedSprite2D _sprite;
	[Export] private MovementComponent _movementComponent;
	[Export] private CharacterBody2D _characterBody;

	public override void _Ready()
	{
		_sprite.FlipH = true;
	}

	public void IdleAnimation()
	{
		if (_characterBody.Velocity == Vector2.Zero)
		{
			_sprite.Play("idle");
		}
	}

	public void RunAnimation()
	{
		if (_characterBody.Velocity.X != 0 && _characterBody.IsOnFloor())
		{
			_sprite.Play("run");
		}
	}
	
	public void JumpAnimation()
	{
		if (!_characterBody.IsOnFloor() && _characterBody.Velocity.Y < 0)
		{
			if (_sprite.Animation != "jump")
			{
				_sprite.Play("jump");
			}
		}
	}
	
	public void FallAnimation()
	{
		if (!_characterBody.IsOnFloor() && _characterBody.Velocity.Y > 0)
		{
			if (_sprite.Animation != "fall" || _sprite.Animation == "jump")
			{
				_sprite.Play("fall");
			}
		}
	}

	public void HurtAnimation()
	{
		//TODO
	}

	public void DeathAnimation()
	{
		//TODO
	}

	public void ChaseAnimation()
	{
		//TODO
	}
	
	public void MeleeAttackAnimation()
	{
		//TODO
	}

	public void RangedAttackAnimation()
	{
		//TODO
	}

	public void SpawnAnimation()
	{
		//TODO
	}

	public void SkidAnimation()
	{
		//TODO
	}

	public void WallSlideAnimation()
	{
		//TODO
	}
	
	public void FlipSprite()
	{
		if (_characterBody.Velocity.X < 0)
		{
			_sprite.FlipH = false;
			// SpriteDirection = -1;
		}
		else if (_characterBody.Velocity.X > 0)
		{
			_sprite.FlipH = true;
			// SpriteDirection = 1;
		}
	
		// if (_currentWeapon != null)
		// {
		// 	_weaponManager.SpriteDirection = SpriteDirection;
  //           
		// }
        
	}
}
