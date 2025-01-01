using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component handles the picking up weapons and pickup items such as health, keys etc.

[GlobalClass]
public partial class PickupsComponent : Area2D
{
	[Export] public WeaponManagerComponent WeaponManagerComponent;
	
	private PickupItemsComponent _pickupItemsComponent; 
	
	public override void _Ready()
	{
		AreaEntered += ItemPickedUp;
	}

	private void ItemPickedUp(Area2D area)
	{
		if (area is PickupItemsComponent pickupItemsComponent)
		{
			PickupItemLogic(pickupItemsComponent);
		}
	}

	private void PickupItemLogic(PickupItemsComponent pickupItemsComponent)
	{
		switch (pickupItemsComponent.ItemType)
		{
			case Overlord.PickupItemTypes.Coffee:
				CoffeePickedUp(pickupItemsComponent);
				break;
			case Overlord.PickupItemTypes.Weapon:
				WeaponPickedUp(pickupItemsComponent);
				break;
			case Overlord.PickupItemTypes.Ammo:
				AmmoPickedUp(pickupItemsComponent);
				break;
			case Overlord.PickupItemTypes.Collectible:
				CollectiblePickupUp(pickupItemsComponent);
				break;
			case Overlord.PickupItemTypes.Key:
				KeyPickedUp(pickupItemsComponent);
				break;
		}
	}
	
	private void CoffeePickedUp(Area2D area)
	{
		GD.Print("Coffee Picked Up: ");
	}
	
	private void WeaponPickedUp(PickupItemsComponent pickupItemsComponent)
	{
		WeaponManagerComponent.EquipWeapon(pickupItemsComponent.ItemName);
	}
	
	private void AmmoPickedUp(Area2D area)
	{
		GD.Print("ammo picked up");
	}

	private void CollectiblePickupUp(Area2D area)
	{
		GD.Print("collectible picked up");
	}

	private void KeyPickedUp(Area2D area)
	{
		GD.Print("key picked up");
	}
	
}
