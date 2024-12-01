using Godot;

namespace CoffeeCatProject.Singletons;
public partial class WeaponTypes : Node
{
    public enum PlayerWeaponTypes
    {
        Revolver,
        Shotgun,
        MachineGun,
        PlasmaRifle
    }

    public const string ShotgunScene = "res://Players/Weapons/Shotgun/Scenes/weapon_shotgun.tscn";
}
