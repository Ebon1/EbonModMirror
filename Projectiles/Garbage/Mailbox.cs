﻿using System;

namespace EbonianMod.Projectiles.Garbage;

public class Mailbox : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 44;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 200;
    }
    public override bool? CanDamage() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        float scale = Math.Clamp(MathHelper.Lerp(0, 1, Projectile.scale * 2), 0, 1);
        Rectangle frame = new Rectangle(0, Projectile.frame * 46, 30, 46);
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, new Vector2(tex.Width / 2, Projectile.height), new Vector2(Projectile.scale, 1), Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        return false;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.scale = 0;
        Projectile.SyncProjectile();
    }
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            Projectile.Center = Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, 1000, true);
            Projectile.ai[0] = 1;
            Projectile.SyncProjectile();
        }
        //if (Projectile.timeLeft == 150)
        //  Projectile.NewProjectileDirect(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
        if (Projectile.timeLeft < 100 && Projectile.ai[1] == 0)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center - new Vector2(0, 6), DustID.Smoke, Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center).RotatedByRandom(PiOver4) * Main.rand.NextFloat(2, 10));
            }
            Projectile.direction = Projectile.spriteDirection = Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center).X > 0 ? 1 : -1;

            SoundEngine.PlaySound(SoundID.Item156, Projectile.Center);
            MPUtils.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center - new Vector2(0, 44), Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center) * 10, ProjectileType<Pipebomb>(), Projectile.damage, 0, Projectile.owner);
            Projectile.ai[1] = 1;
            Projectile.SyncProjectile();
        }

        Projectile.frame = (int)Projectile.ai[1];
        float progress = Utils.GetLerpValue(0, 200, Projectile.timeLeft);
        Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
    }
}
