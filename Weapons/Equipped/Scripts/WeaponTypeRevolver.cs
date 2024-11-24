using Godot;

namespace CoffeeCatProject.Weapons.Equipped.Scripts;

public partial class WeaponTypeRevolver : Weapons
{
    // Weapon class
    private Weapons _weapons;

    // Consts
    private const float CooldownTime = 0.6f;
    private const int BulletCount = 1;
    private const float BulletAngleDegree = 1.0f;

    // Animated sprite
    [Export] private AnimatedSprite2D Animation { get; set; }

    // Cooldown timer
    [Export] private Timer CooldownTimer { get; set; }

    // Muzzle
    [Export] private Marker2D Muzzle { get; set; }

    // Bullet (change later to revolver)
    private BulletShotgun _bulletType;

    public override void _Ready()
    {
        GD.Print("READY");
        _weapons = new Weapons();
    }

    public override void _Process(double delta)
    {
        _weapons.ShootWeapon(_bulletType, Animation, CooldownTimer, Muzzle, BulletCount, BulletAngleDegree);
    }
}
