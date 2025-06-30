﻿using EbonianMod.Items.Materials;
using System;
using System.Collections.Generic;

namespace EbonianMod.Items.Weapons.Magic;

internal class CursedToy : ModItem
{
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Magic;
        Item.damage = 40;
        Item.useTime = 1;
        Item.useAnimation = 10;
        Item.reuseDelay = 25;
        Item.shoot = ProjectileType<CursedToyP>();
        Item.shootSpeed = 4f;
        Item.mana = 10;
        Item.rare = ItemRarityID.LightRed;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.autoReuse = true;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.noMelee = true;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-5, 2);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.GuideVoodooDoll).AddIngredient(ItemType<TerrortomaMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = new Vector2(Main.MouseWorld.X - Main.rand.NextFloat(-150, 150), player.Center.Y - 900);
        velocity = Helper.FromAToB(position, Main.MouseWorld).RotatedByRandom(MathHelper.PiOver4 * 0.1f) * Item.shootSpeed * 4;
    }
}
public class CursedToyP : ModProjectile
{
    public override string Texture => Helper.Placeholder;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 50;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.tileCollide = true;
        Projectile.aiStyle = 0;
        Projectile.extraUpdates = 4;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1000;
        Projectile.Size = new(5, 5);
    }
    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = MathHelper.Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = MathHelper.Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i] != Projectile.position)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                float _off = MathF.Abs(__off + mult);
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(40 * mult, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(40 * mult, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2), col, new Vector2(_off, 1)));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count > 2)
        {
            Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.laser2.Value, false);
        }
        Main.spriteBatch.ApplySaved(sbParams);
        return false;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.timeLeft > 100 * 4)
        {
            Projectile.timeLeft = 100 * 4;
            Projectile.ai[2]++;
        }
        if (Projectile.Center.Y > Main.player[Projectile.owner].Center.Y) return false;
        Projectile.velocity *= 0.5f;
        Projectile.tileCollide = false;
        return false;
    }
    public override bool? CanDamage() => Projectile.ai[2] <= 0;
    public override void AI()
    {
        if (Projectile.ai[2] > 0)
        {
            Projectile.velocity *= 0.98f;
        }
        else if (Projectile.velocity.Length() > 0)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft % 5 == 0 && Projectile.velocity.Length() > 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Projectile.velocity * Main.rand.NextFloat(), Scale: 2).noGravity = true;
        }
    }
}
