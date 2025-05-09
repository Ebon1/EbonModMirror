﻿using EbonianMod.Dusts;

namespace EbonianMod.Projectiles.Garbage;

public class GarbageBag : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 22;
        Projectile.timeLeft = 500;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.height = 24;
        Projectile.extraUpdates = 1;
    }
    public override bool? CanDamage() => Projectile.Opacity > 0.5f;
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
            fallThrough = false;
        return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.frame == 0 && Projectile.timeLeft > 100)
        {
            SoundEngine.PlaySound(SoundID.Item177, Projectile.Center);
            Projectile.timeLeft = 100;
        }
        Projectile.velocity.Y *= 0.5f;
        Projectile.velocity.X = 0;
        Projectile.frame = 1;
        Projectile.SyncProjectile();
        return false;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.Opacity = 1;
        Projectile.SyncProjectile();
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
    public override void AI()
    {
        if (Projectile.frame == 0)
        {
            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustType<ColoredFireDust>(), Projectile.velocity * 0.4f, 0, Color.OrangeRed, 0.1f).noGravity = true;
        }
        Projectile.tileCollide = Projectile.Center.Y > Main.player[Projectile.owner].Center.Y - 20;
        Projectile.velocity *= 1.01f;
        if (Projectile.velocity.Y > 0)
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 0, 0.03f);
        if (Projectile.frame == 0)
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        else
        {
            Projectile.rotation = 0;
            if (Projectile.timeLeft < 40)
                Projectile.Opacity -= 0.03f;
        }

    }
}
