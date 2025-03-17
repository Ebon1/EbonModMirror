﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Projectiles.Friendly.Crimson;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace EbonianMod.Items.Weapons.Melee
{
    public class Spinax : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 80;
            Item.crit = 10;
            Item.damage = 70;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<SpinaxP>();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        int attack = -2;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attack++;
            if (attack > 2)
                attack = -1;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: attack);
            return false;
        }
    }
}
