﻿using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly.Crimson;

namespace EbonianMod.Items.Weapons.Magic
{
    public class Latcher : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Harpoon);
            Item.damage = 80;
            Item.shoot = ProjectileType<LatcherP>();
            Item.DamageType = DamageClass.Magic;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.mana = 25;
            Item.shootSpeed = 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Vertebrae, 20).AddIngredient(ItemID.Hook).AddTile(TileID.Anvils).Register();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<LatcherP>()] < 1;
        }
    }
}
