﻿using System.Collections.Generic;

namespace EbonianMod.Projectiles.Friendly.Generic;

public class PotatoP : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    bool Fire;
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
        Projectile.Size = new Vector2(14, 20);
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.extraUpdates = Projectile.velocity.Length() > 12.1f ? 2 : 1;
        Fire = Projectile.extraUpdates == 2;
        if (Fire)
            Projectile.velocity *= 1.3f;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Fire)
            target.AddBuff(BuffID.OnFire, 400);
    }
    public override void AI()
    {
        if (Fire)
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (Main.rand.NextFloat(0, Pi * 2)).ToRotationVector2() * Main.rand.NextFloat(0.7f, 2f), Scale: Main.rand.NextFloat(0.7f, 1.3f));
        else if (Main.rand.NextBool())
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 0, default, Main.rand.NextFloat(0.25f, 1));
        if (Projectile.wet)
        {
            Fire = false;
            Projectile.extraUpdates = 0;
        }
    }
    float animationOffset;
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.Center);
        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
            if (Projectile.extraUpdates > 1)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Fire)
            lightColor = Color.White;
        animationOffset -= 0.05f;
        if (animationOffset <= 0)
            animationOffset = 1;
        animationOffset = MathHelper.Clamp(animationOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>(ProjectileID.Sets.TrailCacheLength[Type]);
        for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
        {
            if (Projectile.oldPos[i] == Vector2.Zero) continue;
            float mult = 1f - (1f / Projectile.oldPos.Length) * i;
            mult *= mult;

            float __off = animationOffset;
            if (__off > 1) __off = -__off + 1;
            float _off = __off + (float)i / Projectile.oldPos.Length;
            Color color = Color.OrangeRed;
            if (mult > 0)
            {
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 6 - Main.screenPosition + Projectile.Size / 2 + new Vector2(10 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i], Projectile.oldPos[i + 1]).ToRotation() + MathHelper.PiOver2), color * mult, new Vector2(_off, 0)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 6 - Main.screenPosition + Projectile.Size / 2 + new Vector2(10 * mult, 0).RotatedBy(Helper.FromAToB(Projectile.oldPos[i], Projectile.oldPos[i + 1]).ToRotation() - MathHelper.PiOver2), color * mult, new Vector2(_off, 1)));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count > 2 && Fire)
        {
            for (int i = 0; i < 2; i++)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.LintyTrail.Value, false);
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.FlamesSeamless.Value, false);
            }
        }
        Main.spriteBatch.ApplySaved(sbParams);
        return true;
    }
    public override void PostDraw(Color lightColor)
    {
        Texture2D tex = Assets.Extras.fireball.Value;
        if (Fire)
            Main.spriteBatch.Draw(tex, Projectile.Center - Projectile.velocity.ToRotation().ToRotationVector2() * 14 - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, Projectile.rotation, tex.Size() / 2, 0.55f, SpriteEffects.None, 0);
    }
}
