﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using EbonianMod.Common.Systems;
using EbonianMod.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XRift : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.velocity.SafeNormalize(Vector2.UnitX);
            if (Projectile.ai[0] > 100 && Projectile.ai[0] < 200 && Projectile.ai[0] % 10 == 0 && Projectile.damage != 0)
            {
                Vector2 pos = Projectile.Center - new Vector2(2, Main.rand.NextFloat(-15, 15)).RotatedBy(Projectile.velocity.ToRotation());
                Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.7f);
                SoundEngine.PlaySound(EbonianSounds.xSpirit, Projectile.Center);
                for (int i = 0; i < 10; i++)
                {
                    if (i % 2 == 0)
                        Dust.NewDustPerfect(pos, DustType<SparkleDust>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                    else
                        Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                }
                pos = Projectile.Center - new Vector2(10, Main.rand.NextFloat(-15, 15)).RotatedBy(Projectile.velocity.ToRotation());
                Projectile a = Projectile.NewProjectileDirect(null, pos, vel, ProjectileType<XTentacle>(), Projectile.damage, 0);
                a.ai[0] = Main.rand.Next(50, 90);
                a.ai[1] = Main.rand.NextFloat(0.5f, 2f);
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 60)
                Projectile.ai[2] = MathHelper.SmoothStep(0, 1, Projectile.ai[0] / 60);
            else if (Projectile.ai[0] > 300 - 60)
                Projectile.ai[2] = MathHelper.SmoothStep(1, 0, (Projectile.ai[0] - (300 - 60)) / 60);

            if (Projectile.ai[0] == 25)
                SoundEngine.PlaySound(EbonianSounds.cursedToyCharge.WithPitchOffset(-0.1f), Projectile.Center);

            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], MathF.Sin(Projectile.ai[0] * .05f) * 0.05f, 0.25f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ExtraTextures.vortex;
            Texture2D s_tex2 = ExtraTextures.cone7;

            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(s_tex2, Projectile.Center - Projectile.velocity * 30 - Main.screenPosition, null, Color.Indigo * Projectile.ai[2] * 0.5f, Projectile.velocity.ToRotation(), new Vector2(0, s_tex2.Height / 2), new Vector2(1, 2) * Projectile.ai[2] * 2, SpriteEffects.None, 0);
            Vector2 scale = new Vector2(1f + Projectile.ai[1], 0.25f - Projectile.ai[1] * 0.5f);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 125));
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f * Projectile.ai[2]);
            Vector4 col = (Color.DarkOrchid * Projectile.ai[2]).ToVector4();
            col.W = Projectile.ai[2] * 0.15f;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
            for (int i = 0; i < 75; i++)
            {
                float s = MathHelper.SmoothStep(Projectile.ai[2], 0, (float)i / 75);
                Vector2 pos = Projectile.Center - new Vector2(i * s, 0).RotatedBy(Projectile.velocity.ToRotation());
                /*if (i % 5 == 0)
                    tex = ExtraTextures.vortex3");
                else tex = ExtraTextures.vortex");*/
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, /*(i % 5 == 0 ? s * 0.25f : s)*/s, i % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0); ;
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Reload(effect: null);
            return false;
        }
    }
    public class XRiftSmall : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 350;
            Projectile.extraUpdates = 2;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 60)
                Projectile.ai[2] = MathHelper.SmoothStep(0, 1, Projectile.ai[0] / 60);
            else if (Projectile.ai[0] > 350 - 60)
                Projectile.ai[2] = MathHelper.SmoothStep(1, 0, (Projectile.ai[0] - (350 - 60)) / 60);

            if (Projectile.ai[0] == 25)
                SoundEngine.PlaySound(EbonianSounds.cursedToyCharge.WithPitchOffset(-0.1f), Projectile.Center);

            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], MathF.Sin(Projectile.ai[0] * .05f) * 0.05f, 0.25f);

            if (Projectile.timeLeft % 16 == 0 && Projectile.timeLeft < 280 && Projectile.timeLeft > 140)
            {
                Projectile a = Projectile.NewProjectileDirect(null, Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f), ProjectileType<XTentacle>(), 15, 0);
                a.ai[0] = Main.rand.Next(50, 90);
                a.ai[1] = Main.rand.NextFloat(2.5f, 5f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ExtraTextures.vortex;
            Vector2 scale = new Vector2(1f + Projectile.ai[1], 0.25f - Projectile.ai[1] * 0.5f);
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 125));
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f * Projectile.ai[2]);
            Vector4 col = (Color.DarkOrchid * Projectile.ai[2]).ToVector4();
            col.W = Projectile.ai[2] * 0.15f;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
            for (int i = 0; i < 75; i++)
            {
                float s = MathHelper.SmoothStep(Projectile.ai[2] * 0.5f, 0, (float)i / 75);
                Vector2 pos = Projectile.Center - new Vector2(i * s, 0).RotatedBy(Projectile.velocity.ToRotation());
                /*if (i % 5 == 0)
                    tex = ExtraTextures.vortex3");
                else tex = ExtraTextures.vortex");*/
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, /*(i % 5 == 0 ? s * 0.25f : s)*/s, i % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0); ;
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Reload(effect: null);
            return false;
        }
    }
}
