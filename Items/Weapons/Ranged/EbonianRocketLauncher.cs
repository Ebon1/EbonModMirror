﻿using EbonianMod.Items.Materials;
using EbonianMod.Projectiles.Bases;
using EbonianMod.Projectiles.Friendly.Corruption;

namespace EbonianMod.Items.Weapons.Ranged;


public class EbonianRocketLauncher : ModItem
{
    public override bool CanConsumeAmmo(Item ammo, Player player) => false;
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 40;
        Item.crit = 10;
        Item.damage = 96;
        Item.DamageType = DamageClass.Ranged;
        Item.shoot = ProjectileType<EbonianRocketLauncherGraphics>();
        Item.value = Item.buyPrice(0, 17, 0, 0);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.noMelee = true;
        Item.useStyle = 5;
        Item.useAmmo = AmmoID.Rocket;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        velocity = Vector2.Zero;
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.RocketLauncher).AddIngredient(ItemType<TerrortomaMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
}

public class EbonianRocketLauncherGraphics : HeldProjectileGun
{
    float HoldOffset;
    Vector2 Scale = new Vector2(0.2f, 0.4f);
    public override bool? CanDamage() => false;
    public override string Texture => "EbonianMod/Items/Weapons/Ranged/EbonianRocketLauncher";
    public override void SetDefaults()
    {
        base.SetDefaults();
        ItemType = ItemType<EbonianRocketLauncher>();
        RotationSpeed = 0.25f;
        Projectile.Size = new(50, 40);
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Helper.FromAToB(Main.player[Projectile.owner].Center, Main.MouseWorld).ToRotation();
    }

    public override void AI()
    {
        base.AI();
        Player player = Main.player[Projectile.owner];

        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);

        HoldOffset = Lerp(HoldOffset, 24, 0.1f);

        Projectile.ai[0]++;
        if (Projectile.ai[0] == 110)
        {
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 4;
            Projectile.ai[2] = 0.1f;
        }
        if (Projectile.ai[1] > 0)
        {
            Projectile.ai[2] -= 0.01f;
            if (Projectile.ai[2] <= 0)
            {
                Shoot();
                Projectile.ai[2] = 0.1f;
                Projectile.ai[1]--;
            }
        }
        if (!Main.player[Projectile.owner].channel)
            Projectile.Kill();
    }
    void Shoot()
    {
        Player player = Main.player[Projectile.owner];
        Projectile.UseAmmo(AmmoID.Rocket);
        SoundEngine.PlaySound(SoundID.NPCDeath13.WithPitchOffset(Main.rand.NextFloat(-1, -0.5f)), Projectile.Center);
        HoldOffset = 5;
        Vector2 SpawnPosition = Projectile.Center + new Vector2(Projectile.rotation.ToRotationVector2().X, Projectile.rotation.ToRotationVector2().Y) * 42 + (Projectile.rotation + 90 * -Main.player[Projectile.owner].direction).ToRotationVector2() * 20;
        for (int i = 0; i < 10; i++)
            Dust.NewDustPerfect(SpawnPosition, DustID.CorruptGibs, (Projectile.rotation + Main.rand.NextFloat(PiOver4, -PiOver4)).ToRotationVector2() * Main.rand.NextFloat(2, 10), 150, Scale: Main.rand.NextFloat(1, 3)).noGravity = true;
        Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), SpawnPosition, Projectile.rotation.ToRotationVector2() * 4, ProjectileType<EbonianRocket>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        Scale = new Vector2(1f, 1.8f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Main.player[Projectile.owner];
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(Projectile.width / 2 - HoldOffset, Projectile.height / 2 + 12 * player.direction), Scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}