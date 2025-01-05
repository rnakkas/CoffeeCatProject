using Godot;

namespace CoffeeCatProject.GlobalScripts;

// Hold player related stuff that can be called by other nodes such as enemies
public partial class Overlord: Node
{
    public static Overlord Instance { get; private set; }
    public Vector2 PlayerGlobalPosition {get; private set;}
    public Vector2 PlayerHeadTargetGlobalPosition {get; private set;}

    public enum PickupItemTypes
    {
        Coffee,
        Weapon,
        Ammo,
        Collectible,
        Key
    }

    public enum PickupItemNames
    {
        CoffeeMug,
        CoffeeJar,
        Pistol,
        Shotgun,
        PlasmaRifle,
        RocketLauncher,
        Bullets,
        Shells,
        PlasmaCells,
        Rockets,
        Catnip,
        RedKey,
        BlueKey,
        YellowKey
    }
    
    // Packed scene: shotgun
    public static readonly PackedScene WeaponShotgunScene = 
        ResourceLoader.Load<PackedScene>("res://Players/Weapons/Shotgun/Scenes/weapon_shotgun.tscn");
    
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
    ///
    
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