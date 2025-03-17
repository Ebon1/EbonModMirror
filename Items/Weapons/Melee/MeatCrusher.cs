﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Projectiles;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.Audio;
using EbonianMod.Items.Materials;
using Terraria.Graphics.CameraModifiers;
//using EbonianMod.Worldgen.Subworlds;
//

namespace EbonianMod.Items.Weapons.Melee
{
    public class MeatCrusher : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = Item.height = 80;
            Item.crit = 10;
            Item.damage = 185;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<MeatCrusherP>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.BreakerBlade).AddIngredient(ItemType<TerrortomaMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.UnitX * player.direction, type, damage, knockback, player.whoAmI, 0, -player.direction, 1);
            return false;
        }
    }
    public class MeatCrusherP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/MeatCrusher";
        public override void SetExtraDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            swingTime = 60;
            Projectile.ignoreWater = true;
            minSwingTime = 52;
            holdOffset = 50;
            Projectile.tileCollide = false;
        }
        public override float Ease(float x)
        {
            return x == 0
    ? 0
    : x == 1
    ? 1
    : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
    : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
                Projectile.NewProjectile(null, Projectile.Center, Vector2.UnitX * player.direction, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, -player.direction, 1);
        }
        float lerpProg = 1, swingProgress, rotation;
        public override void ExtraAI()
        {
            if (lerpProg != 1 && lerpProg != -1)
                lerpProg = MathHelper.SmoothStep(lerpProg, 1, 0.1f);
            //          if (lerpProg < 0)
            //                Projectile.timeLeft++;
            //if (lerpProg < 0.2f)
            //lerpProg += 0.025f;
            //if (lerpProg < 1)
            //  lerpProg -= 0.2f;
            if (swingProgress > 0.25f && swingProgress < 0.85f)
                if (Projectile.ai[0] == 0 && Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 100) < 15)
                {
                    Projectile.ai[0] = 1;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 5, 6, 30, 1000));
                    Projectile.timeLeft = 15;
                    SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center - new Vector2(0, 20), new Vector2(Main.rand.NextFloat(-4.5f, 4.5f), Main.rand.NextFloat(-2, -3)), ProjectileType<VileMeatChunk>(), Projectile.damage / 2, 0, Projectile.owner);
                    }

                    lerpProg = -1;
                }
            Player player = Main.player[Projectile.owner];
            int direction = (int)Projectile.ai[1];
            if (lerpProg >= 0)
                swingProgress = MathHelper.Lerp(swingProgress, Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft)), lerpProg);
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            if (lerpProg >= 0)
                rotation = MathHelper.Lerp(rotation, direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress, lerpProg);
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.heldProj = Projectile.whoAmI;
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] < 1 && swingProgress > 0.35f && swingProgress < 0.65f;
        }
        bool _hit;
        public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 2, 6, 30, 1000));
            if (!_hit)
            {
                lerpProg = -.25f;
                _hit = true;
            }
        }
    }
}
