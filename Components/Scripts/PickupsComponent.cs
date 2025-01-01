using System;
using CoffeeCatProject.Players.Weapons;
using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component handles the picking up weapons and pickup items such as health, keys etc.

[GlobalClass]
public partial class PickupsComponent : Area2D
{
	[Export] public WeaponManagerComponent WeaponManagerComponent;
	
	private const string WeaponPickupAreaMetadata = "WeaponPickupType";
	
	public override void _Ready()
	{
		AreaEntered += WeaponPickedUp;
		AreaEntered += CoffeePickedUp;
		AreaEntered += KeyPickedUp;
		AreaEntered += CatnipPickupUp;
		AreaEntered += AmmoPickedUp;
	}

	private void AmmoPickedUp(Area2D area)
	{
		
	}

	private void CatnipPickupUp(Area2D area)
	{
		
	}

	private void KeyPickedUp(Area2D area)
	{
		
	}

	private void CoffeePickedUp(Area2D area)
	{
		
	}

	private void WeaponPickedUp(Area2D area)
	{
		if (!area.HasMeta(WeaponPickupAreaMetadata))
		{
			throw new Exception("Missing metadata " + WeaponPickupAreaMetadata + " in area");
		}
        
		WeaponManagerComponent.EquipWeapon(
			area.GetMeta(WeaponPickupAreaMetadata).ToString().ToLower()
		);
	}
}
