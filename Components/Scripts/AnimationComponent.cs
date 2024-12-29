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

	public void RunAnimation()
	{
		_sprite.Play("run");
	}
	
	public void JumpAnimation()
	{
		_sprite.Play("jump");
	}
	
	public void FallAnimation()
	{
		_sprite.Play("fall");
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
