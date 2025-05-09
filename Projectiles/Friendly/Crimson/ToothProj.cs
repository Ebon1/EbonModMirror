﻿using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Projectiles.Friendly.Crimson;

public class ToothProj : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 10;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 290;
        Projectile.frame = 2;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 14; i++)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Scale: 1.5f);
        }
    }
    float Gravity;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(SoundID.NPCHit9.WithPitchOffset(Main.rand.NextFloat(-0.5f, 0f)), Projectile.Center);
        SoundEngine.PlaySound(EbonianSounds.chomp1.WithPitchOffset(Main.rand.NextFloat(0f, 1f)), Projectile.Center);
        for (int i = 0; i < 4; i++)
        {
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, (Projectile.rotation + Main.rand.NextFloat(-Pi / 8, Pi / 8)).ToRotationVector2() * Main.rand.Next(6, 10), ProjectileType<Gibs>(), 15, 0);
        }
    }
    public override void AI()
    {
        Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Main.rand.NextFloat(0, Pi * 2).ToRotationVector2(), Scale: 1.5f).noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Gravity > -0.5f)
            Gravity -= 0.01f;
        Projectile.velocity = new Vector2(Projectile.velocity.X, Projectile.velocity.Y - Gravity);
    }
}
