using Godot;
using System;
// using CoffeeCatProject.Player.Scripts;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;

public partial class WeaponManager : Node
{
	
	// Variables
	private string _currentWeapon;
	private bool _idle;
	private bool _wallSlide;
	private float _spriteDirection;
	public float Test;
	
	// Packed scene: shotgun
	private readonly PackedScene _weaponShotgun = 
		ResourceLoader.Load<PackedScene>("res://Weapons/Equipped/Scenes/weapon_shotgun.tscn");
	
	public override void _Ready()
	{
		GD.Print("weapon manager ready");
	}

	private void EquipWeapon(string weaponName)
	{
		_currentWeapon = weaponName;
		
		switch (weaponName)
		{
			case not null when weaponName.Contains("shotgun"):
				// Instantiate the weapon scene, set direction based on player's direction, add scene as child of player
				var weaponInstance = (WeaponShotgun)_weaponShotgun.Instantiate();
				weaponInstance.Direction = _spriteDirection;
				AddChild(weaponInstance);
				break;
            
			case not null when weaponName.Contains("machine_gun"):
				GD.Print("machine gun");
				break;
            
			case not null when weaponName.Contains("revolver"):
				GD.Print("revolver picked up");
				break;
            
			default:
				throw new Exception("weapon type " + weaponName + "not found");
		}
	}

	private void ShootWeapon()
	{
		if (_currentWeapon != null)
		{
			GD.Print("Shoot");
		}
		
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("shoot"))
		{
			GD.Print("Shooting");
		}
	}
}
