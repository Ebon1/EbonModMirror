﻿using EbonianMod.Dusts;
using System;
using System.Collections.Generic;
namespace EbonianMod.Projectiles.Terrortoma;

public class TFlameThrower : ModProjectile //BREATH
{
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hit)
    {
        target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
    }

    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = 1f;
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - PiOver2), col, new Vector2(_off, 1)));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count > 2)
        {
            Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.FlamesSeamless.Value, false);
        }
        Main.spriteBatch.ApplySaved(sbParams);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(101);
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        AIType = 101;
    }

    public override void PostAI()
    {

        for (int i = 0; i < 2; i++)
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X, Projectile.velocity.Y, Scale: 4).noGravity = true;
    }
    public override bool PreKill(int timeLeft)
    {
        Projectile.active = false;
        return false;
    }
}
public class TFlameThrower2 : ModProjectile //FALL
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hit)
    {
        target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
    }

    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = 1f;
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - PiOver2), col, new Vector2(_off, 1)));
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
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(95);
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        AIType = -1;
        Projectile.timeLeft = 200;
        Projectile.aiStyle = 2;
    }

    public override void PostAI()
    {
        for (int i = 0; i < 2; i++)
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X, Projectile.velocity.Y, Scale: 4).noGravity = true;
        //
    }
}

public class TFlameThrower2_Inverted : ModProjectile //FALL
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hit)
    {
        target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
    }

    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = 1f;
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - PiOver2), col, new Vector2(_off, 1)));
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
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(95);
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        AIType = -1;
        Projectile.timeLeft = 200;
        Projectile.aiStyle = 2;
    }
    public override bool ShouldUpdatePosition() => false;

    public override void PostAI()
    {
        Projectile.Center -= new Vector2(0, Projectile.velocity.Y);
        if (Projectile.velocity.Y > 0)
            Projectile.Center += new Vector2(Projectile.velocity.X * 5, 0);
        for (int i = 0; i < 2; i++)
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X, Projectile.velocity.Y, Scale: 4).noGravity = true;
        //
    }
}
public class TFlameThrowerHoming : ModProjectile //HOME
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hit)
    {
        target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
    }

    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = 1f;
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(50 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - PiOver2), col, new Vector2(_off, 1)));
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
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(95);
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        AIType = -1;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 200;
    }
    public override bool ShouldUpdatePosition() => false;
    Vector2 target;
    public override void AI()
    {
        if (Projectile.timeLeft > 150)
        {
            Projectile.Center += Projectile.velocity;
            if (Projectile.timeLeft < 120)
                Projectile.velocity *= 0.95f;
            if (Projectile.timeLeft < 110)
                Projectile.velocity *= 0.9f;
            Projectile.velocity *= 0.99f;
            target = Main.player[Projectile.owner].Center;
        }
        else if (Projectile.timeLeft > 130)
        {
            Projectile.velocity += Helper.FromAToB(Projectile.Center, target) * 1.7f;
        }

        Projectile.ai[2]++;
        Projectile.Center += Projectile.velocity.RotatedBy(MathF.Sin(Projectile.ai[2] * 3) * 0.1f);

        //for (int i = 0; i < 2; i++)
        //  Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, Scale: 4).noGravity = true;
        //
    }
}
public class TFlameThrower3 : ModProjectile //NORMAL
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = 1f;
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - PiOver2), col, new Vector2(_off, 1)));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count > 2)
        {
            Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.FlamesSeamless.Value, false);
        }
        Main.spriteBatch.ApplySaved(sbParams);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hit)
    {
        target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
    }
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(96);
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 400;
        AIType = 96;
    }

    public override void PostAI()
    {
        for (int i = 0; i < 2; i++)
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X, Projectile.velocity.Y, Scale: 4).noGravity = true;
    }
}
public class TFlameThrower4 : ModProjectile //ACCELERATE
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hit)
    {
        target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
    }

    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = 1f;
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - PiOver2), col, new Vector2(_off, 1)));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count > 2)
        {
            Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.FlamesSeamless.Value, false);
        }
        Main.spriteBatch.ApplySaved(sbParams);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(96);
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }
    public override bool ShouldUpdatePosition()
    {
        return ++Projectile.ai[2] > 30;
    }
    public override void PostAI()
    {

        if (Projectile.ai[2] > 30)
            Projectile.velocity *= 1.025f;
        else
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0, 0, Scale: 4).noGravity = true;
    }
}
public class TFlameThrowerSine : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo hit)
    {
        target.AddBuff(BuffID.CursedInferno, Main.rand.Next(20, 100));
    }

    float vfxOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
        Main.spriteBatch.Reload(BlendState.Additive);
        vfxOffset -= 0.015f;
        if (vfxOffset <= 0)
            vfxOffset = 1;
        vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float s = 0;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float mult = (1f - fadeMult * i);

            if (mult < 0.5f)
                s = Clamp(mult * 3.5f, 0, 0.5f) * 3;
            else
                s = Clamp((-mult + 1) * 2, 0, 0.5f) * 3;

            if (i > 0 && Projectile.oldPos[i] != Vector2.Zero)
            {
                Color col = Color.LawnGreen * mult * 2 * s;

                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() + PiOver2), col, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(30 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i - 1], Projectile.oldPos[i]).ToRotation() - PiOver2), col, new Vector2(_off, 1)));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count > 2)
        {
            Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.FlamesSeamless.Value, false);
        }
        Main.spriteBatch.ApplySaved(sbParams);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(96);
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }
    public override bool ShouldUpdatePosition() => false;
    Vector2 startP, startV;
    public override void PostAI()
    {
        if (startP == Vector2.Zero)
        {
            startP = Projectile.Center;
            startV = Projectile.velocity;
        }
        else
            Projectile.SineMovement(startP, startV, 0.3f, 5f);

        if (Projectile.ai[0] != 0 && Projectile.timeLeft % 2 == 0)
            Dust.NewDustPerfect(Projectile.Center - startV * 2, DustType<LineDustFollowPoint>(), -Projectile.velocity.RotatedByRandom(PiOver4 * 0.1f) * Main.rand.NextFloat(), 0, Color.LawnGreen, Main.rand.NextFloat(0.07f, 0.15f));
    }
}
