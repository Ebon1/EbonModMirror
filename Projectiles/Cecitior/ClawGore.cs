﻿using EbonianMod.Common.Systems.Verlets;

namespace EbonianMod.Projectiles.Cecitior;

public class ClawGore : ModProjectile
{
    public override string Texture => "EbonianMod/NPCs/Cecitior/Hook/CecitiorHook_8";
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
    }
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.Size = new Vector2(50, 50);
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.Opacity = 1f;
        Projectile.timeLeft = 400;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = Vector2.Zero;
        for (int i = 0; i < 3; i++)
        {
            if (claw[i].verlet is not null)
            {
                claw[i].verlet.lastP.locked = false;
                claw[i].verlet.firstP.locked = false;
            }
        }
        return false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (claw[0].verlet is null)
        {
            for (int i = 0; i < 3; i++)
            {
                claw[i] = new CecitiorClaw(new Vector2(Projectile.ai[0], Projectile.ai[1]), new Verlet(Projectile.Center, 12, 22, 15, true, true, 5, true, 20));
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                claw[i].verlet.Update(claw[i].position, Projectile.Center);

                claw[i].verlet.Draw(Main.spriteBatch, new VerletDrawData(new VerletTextureData("NPCs/Cecitior/Hook/CecitiorHook_0", _endTex: "NPCs/Cecitior/Hook/CecitiorHook_8"), _color: lightColor * Projectile.Opacity));
            }
        }
        return false;
    }
    public struct CecitiorClaw
    {
        public Vector2 position;
        public Verlet verlet;
        public CecitiorClaw(Vector2 _position, Verlet _verlet)
        {
            position = _position;
            verlet = _verlet;
        }
    }
    public override void AI()
    {
        if (Projectile.timeLeft < 50)
            Projectile.Opacity -= 0.025f;
        for (int i = 0; i < 3; i++)
        {
            claw[i].position += Projectile.velocity * 0.45f;
        }
        Projectile.velocity.Y += 0.5f;
    }
    public CecitiorClaw[] claw = new CecitiorClaw[3];
    public override void OnSpawn(IEntitySource source)
    {
        if (Projectile.ai[0] == 0)
        {
            Projectile.ai[0] = Projectile.Center.X;
            Projectile.ai[1] = Projectile.Center.Y;
        }
        SoundEngine.PlaySound(EbonianSounds.fleshHit with { Pitch = -0.3f, PitchVariance = 0.2f }, Projectile.Center);
    }
}
