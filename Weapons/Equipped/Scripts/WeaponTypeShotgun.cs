using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;

public partial class WeaponTypeShotgun : Weapons
{
    // Weapon class
    private Weapons _weapons;
    
    // Consts
    private const float CooldownTime = 1.1f;
    private const int BulletCount = 3;
    private const float BulletAngleDegree = 3.5f;
    
    // Animated sprite
    [Export]
    private AnimatedSprite2D Animation {get; set;}
    
    // Cooldown timer
    [Export]
    private Timer CooldownTimer {get; set;}
    
    // Muzzle
    [Export]
    private Marker2D Muzzle  {get; set;}
    
    // Bullet
    private BulletShotgun _bulletType;
    
    public override void _Ready()
    {
        GD.Print("Ready");
        _weapons = new Weapons();
    }

    public override void _Process(double delta)
    {
        _weapons.ShootWeapon(_bulletType, Animation, CooldownTimer, Muzzle, BulletCount, BulletAngleDegree);
    }
    
}
