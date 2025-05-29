﻿
using EbonianMod.Projectiles.Enemy.Snow;
using System;

namespace EbonianMod.NPCs.Snow;

public class BorealDancer : ModNPC
{
    public override void SetDefaults()
    {
        NPC.width = 20;
        NPC.height = 50;
        NPC.HitSound = SoundID.Item49;
        NPC.DeathSound = SoundID.Item27;
        NPC.damage = 1;
        NPC.defense = 0;
        NPC.lifeMax = 20;
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 14;
    }

    public override void AI()
    {
        NPC.TargetClosest(true);
        Player player = Main.player[NPC.target];

        Vector2 VectorDistance = player.Center - NPC.Center;
        bool SeesAPlayer = Helper.TRay.CastLength(NPC.Center - Vector2.UnitY*9, VectorDistance, VectorDistance.Length(), false) >= VectorDistance.Length()-0.4f;
        NPC.ai[1] = 2;
        if (SeesAPlayer)
        {
            if (NPC.ai[0] == 0)
            {
                NPC.ai[0] = 2;
            }
            if (NPC.ai[0] == 1)
            {
                NPC.velocity.X += NPC.direction * 0.08f;
                NPC.velocity.X = Clamp(NPC.velocity.X, -11, 11);
                if (MathF.Abs(player.Center.X - NPC.Center.X) < 62 && MathF.Abs(player.Center.Y - NPC.Center.Y) < 30)
                {
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), Helper.TRay.Cast(NPC.Center - new Vector2(-NPC.direction * 15, 35), Vector2.UnitY, 5000, true) + new Vector2(0, 3), Vector2.Zero, ProjectileType<BorealSpike>(), NPC.damage, 0).ai[1] = NPC.direction;
                    NPC.ai[0] = 2;
                    NPC.velocity.X = -NPC.direction * 2.4f;
                }
            }
            else
            {
                NPC.velocity.X *= 0.9f;
                NPC.spriteDirection = player.Center.X > NPC.Center.X ? 1 : -1;
            }
        }
        float XVelocityModule = MathF.Abs(NPC.velocity.X);
        if (NPC.ai[0] != 1)
        {
            NPC.ai[1] = 2;
        }
        else
        {
            NPC.ai[1] = Clamp((int)(XVelocityModule*0.8f), 1, 3);
        }
        if (Helper.TRay.CastLength(NPC.Center - Vector2.UnitY*12, new Vector2(NPC.velocity.X, 0), 45, false) < 12 && XVelocityModule > 0.4f)
        {
            for (int u = 0; u < 15; u++)
            {
                Dust.NewDustPerfect(NPC.Center, DustID.Snow, -NPC.velocity.X/4 * Main.rand.NextFloat(-Pi/3, Pi/3).ToRotationVector2() * Main.rand.NextFloat(2, 7), Scale: Main.rand.NextFloat(0.7f, 1.3f)).noGravity = true;
            }
            NPC.velocity.X *= -0.6f;
        }
        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
    }

    public override bool CheckDead()
    {
        for (int i = 1; i < 4; i++)
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + Main.rand.NextVector2Circular(7, 7), NPC.velocity, Find<ModGore>("EbonianMod/BorealDancer" + i).Type, NPC.scale);
            for (int u = 0; u < 15; u++)
            {
                Dust.NewDustPerfect(NPC.Center, DustID.Snow, Main.rand.NextFloat(-Pi, Pi).ToRotationVector2() * Main.rand.NextFloat(2, 10), Scale: Main.rand.NextFloat(0.7f, 1.3f)).noGravity = true;
            }
        }
        return true;
    }

    public override void FindFrame(int frameHeight) 
    {
        NPC.frameCounter++;
        if (NPC.frameCounter * NPC.ai[1] > 10)
        {
            NPC.frameCounter = 0;
            if(NPC.ai[0] != 0)
                NPC.frame.Y += frameHeight;
            if(NPC.ai[0] == 1 && NPC.frame.Y > 7 * frameHeight)
            {
                NPC.frame.Y = frameHeight;
            }
            if (NPC.ai[0] == 2)
            {
                SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(Main.rand.NextFloat(0f, 1f)), NPC.Center);
                NPC.frame.Y = 8 * frameHeight;
                NPC.ai[0] = -1;
            }
            if (NPC.frame.Y > 13 * frameHeight)
            {
                NPC.ai[0] = 1;
                NPC.frame.Y = frameHeight;
            }
        }
    }
}
