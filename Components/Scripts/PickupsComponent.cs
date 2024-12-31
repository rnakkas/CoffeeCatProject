using System;
using CoffeeCatProject.GlobalScripts;
using CoffeeCatProject.Players.Weapons;
using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component handles the picking up weapons and pickup items such as health, keys etc.

[GlobalClass]
public partial class PickupsComponent : Area2D
{
	[Export] public WeaponManagerComponent WeaponManagerComponent;
	
	[Signal] public delegate void PickupItemEnteredEventHandler(PickupItemsComponent item);

	private string _pickupItemType;
	private string _pickupItemName;
	
	public override void _Ready()
	{
		PickupItemEntered += ItemPickedUp;
	}

	public override void _Process(double delta)
	{
		WeaponPickedUp();
	}

	private void WeaponPickedUp()
	{
		if (_pickupItemType != Overlord.PickupItemTypes.Weapon.ToString())
			return;
		WeaponManagerComponent?.EquipWeapon(_pickupItemName);
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

	private void ItemPickedUp(PickupItemsComponent item)
	{
		GD.Print("Picked up ");
		_pickupItemType = item.ItemType.ToString();
		_pickupItemName = item.ItemName.ToString();

		// if (!item.WeaponTypes)
		//
		//
		// if (!area.HasMeta(WeaponPickupAreaMetadata))
		// {
		// 	throw new Exception("Missing metadata " + WeaponPickupAreaMetadata + " in area");
		// }
		//       
		// WeaponManagerComponent?.EquipWeapon(
		// 	area.GetMeta(WeaponPickupAreaMetadata).ToString().ToLower()
		// );
	}
}
