using CoffeeCatProject.GlobalScripts;
using Godot;

namespace CoffeeCatProject.Components.Scripts;

// This component handles the picking up weapons and pickup items such as health, keys etc.

[GlobalClass]
public partial class PickupsComponent : Area2D
{
	[Export] private WeaponManagerComponent _weaponManagerComponent;
	[Export] private HealthComponent _healthComponent;
	
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
	
	private void CoffeePickedUp(PickupItemsComponent pickupItemsComponent)
	{
		_healthComponent.Heal(pickupItemsComponent);
	}
	
	private void WeaponPickedUp(PickupItemsComponent pickupItemsComponent)
	{
		pickupItemsComponent.ItemGetsPickedUp();
		_weaponManagerComponent.EquipWeapon(pickupItemsComponent.ItemName);
	}
	
	private void AmmoPickedUp(Area2D area)
	{
		GD.Print("ammo picked up: talk to WeaponManagerComponent to increase ammo");
	}

	private void CollectiblePickupUp(Area2D area)
	{
		GD.Print("collectible picked up: talk to Overlord to increase collectible count");
	}

	private void KeyPickedUp(Area2D area)
	{
		GD.Print("key picked up: talk to Overlord to set key bools");
	}
	
}
