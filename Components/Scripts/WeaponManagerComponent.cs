using System;
using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component will handle equipping weapons, switching weapons, tracking ammo count
[GlobalClass]
public partial class WeaponManagerComponent : Node2D
{
	// Variables
	private string _currentWeapon;
	public float SpriteDirection { get; set; }
	public bool WallSlide { get; set; }

	private Node _weapon;
	private Area2D _bullets;
	
	public override void _Ready()
	{
		GD.Print("weapon manager ready");
	}

	// Instantiate the weapon and add as sibling, i.e. child of player scene
	public void EquipWeapon(Overlord.PickupItemNames weaponName)
	{
		switch (weaponName)
		{
			case Overlord.PickupItemNames.Shotgun:
				_weapon = Overlord.WeaponShotgunScene.Instantiate();
				AddSibling(_weapon);
				break;
            
			case Overlord.PickupItemNames.Pistol:
				GD.Print("pistol equipped");
				break;
            
			case Overlord.PickupItemNames.PlasmaRifle:
				GD.Print("PlasmaRifle equipped");
				break;
			
			case Overlord.PickupItemNames.RocketLauncher:
				GD.Print("rocket launcher equipped");
				break;
            
			default:
				throw new Exception("weapon type " + weaponName + "not found");
		}
	}

	private void SwapWeapon()
	{
		if (_currentWeapon != null)
		{
			GD.Print("swap");
		}
		
	}
}
