﻿using EbonianMod.Buffs;
using EbonianMod.Items.Materials;
using EbonianMod.Projectiles.Minions;

namespace EbonianMod.Items.Weapons.Summoner;

public class CecitiorClawSummon : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
        ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 58;
        Item.DamageType = DamageClass.Summon;
        Item.mana = 10;
        Item.width = 26;
        Item.height = 28;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = SoundID.NPCHit1;
        Item.shoot = ProjectileType<CecitiorClawMinion>();
        Item.buffType = BuffType<CecitiorClawBuff>();
        Item.shootSpeed = 1;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer != player.whoAmI) return false;
        player.AddBuff(Item.buffType, 2);
        var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
        projectile.originalDamage = Item.damage;
        projectile.SyncProjectile();

        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.VampireFrogStaff).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
}
