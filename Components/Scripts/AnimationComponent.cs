using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component animates the characters based on their actions or statuses

[GlobalClass]
public partial class AnimationComponent : Node2D
{
	[Export] private AnimatedSprite2D _sprite;

	public void IdleAnimation()
	{
		_sprite.Play("idle");
	}

	public void RunAnimation(RunComponent runComponent)
	{
		var isRunning = runComponent?.IsRunning ?? false;
		
		if (isRunning)
		{
			_sprite.Play("run");
		}
		else
		{
			IdleAnimation();
		}
	}
	
	public void JumpAndFallAnimations(JumpComponent jumpComponent, FallComponent fallComponent)
	{
		var isJumping = jumpComponent?.IsJumping ?? false;
		var isFalling = jumpComponent?.IsFalling ?? false;
	
		if (isJumping)
		{
			_sprite.Play("jump");
		}
		
		if (isFalling)
		{
			_sprite.Play("fall");
		}
		
		
	}
	
	public void FallAnimation(FallComponent fallComponent)
	{
		var isFalling = fallComponent?.IsFalling ?? false;

		if (isFalling)
		{
			_sprite.Play("fall");
		}
		else
		{
			IdleAnimation();
		}
	}
	
	public void FlipSprite(float directionX)
	{
		if (directionX < 0)
		{
			_sprite.FlipH = false;
			// SpriteDirection = -1;
		}
		else if (directionX > 0)
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
