using System;
using Godot;

namespace CoffeeCatProject.Player.WeaponManager.Scripts;

public partial class WeaponManagerScript : Node
{
	// Variables
	private string _currentWeapon;
	public float SpriteDirection { get; set; }
	public bool WallSlide { get; set; }
	private Node _weapon;

	private enum WeaponTypes
	{
		Shotgun,
		Revolver,
		MachineGun,
		PlasmaRifle
	}
	
	// Packed scene: shotgun
	private readonly PackedScene _weaponShotgun = 
		ResourceLoader.Load<PackedScene>("res://Player/Weapons/Shotgun/Scenes/weapon_shotgun.tscn");
	
	// Packed scene: revolver
	/// <summary>
	/// TODO
	/// </summary>
	///
	/// // Packed scene: machinegun
	/// <summary>
	/// TODO
	/// </summary>
	///
	/// /// // Packed scene: plasmarifle
	/// <summary>
	/// TODO
	/// </summary>
	
	public override void _Ready()
	{
		GD.Print("weapon manager ready");
	}

	public void EquipWeapon(string weaponName)
	{
		weaponName = weaponName.ToLower();
		_currentWeapon = weaponName;
		
		switch (weaponName)
		{
			case not null when weaponName.Contains(WeaponTypes.Shotgun.ToString().ToLower()):
				
				// Instantiate the weapon scene, set direction based on player's direction, add scene as child of player
				_weapon = _weaponShotgun.Instantiate();
				GetParent().AddChild(_weapon);
				break;
            
			case not null when weaponName.Contains(WeaponTypes.MachineGun.ToString().ToLower()):
				
				GD.Print("machine gun");
				break;
            
			case not null when weaponName.Contains(WeaponTypes.Revolver.ToString().ToLower()):
				
				GD.Print("revolver picked up");
				break;
			
			case not null when weaponName.Contains(WeaponTypes.PlasmaRifle.ToString().ToLower()):
				
				GD.Print("plasma-rifle picked up");
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
	
	public override void _Process(double delta)
	{
		
	}
}
