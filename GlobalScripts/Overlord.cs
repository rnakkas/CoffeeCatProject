using Godot;

namespace CoffeeCatProject.GlobalScripts;

// Hold player related stuff that can be called by other nodes such as enemies
public partial class Overlord: Node
{
    public static Overlord Instance { get; private set; }
    public Vector2 PlayerGlobalPosition {get; private set;}
    public Vector2 PlayerHeadTargetGlobalPosition {get; private set;}

    public enum EnemyProjectileTypes
    {
        AttackProjectile,
        DeathProjectile
    }

    public enum EnemyMetadataTypes
    {
        AttackType
    }
    
    public enum EnemyAttackTypes
    {
        MeleeAttack,
        ProjectileAttack,
        FattySpikeAttack
    }

    public enum PickupItemTypes
    {
        Coffee,
        Collectible,
        Weapon,
        Ammo,
        Key
    }

    public enum PickupItemNames
    {
        CoffeeMug,
        CoffeeJar,
        Catnip,
        Pistol,
        Shotgun,
        MachineGun,
        PlasmaRifle,
        RocketLauncher,
        Bullets,
        Shells,
        Cartridges,
        PlasmaCells,
        Rockets,
        RedKey,
        BlueKey,
        YellowKey
    }
    
    public void UpdatePlayerGlobalPosition(Vector2 globalPosition)
    {
        PlayerGlobalPosition =  globalPosition;
    }

    public void UpdatePlayerHeadTargetGlobalPosition(Vector2 globalPosition)
    {
        PlayerHeadTargetGlobalPosition = globalPosition;
    }
    
    public override void _Ready()
    {
        Instance = this;
    }
    
}