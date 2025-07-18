﻿using EbonianMod.Common.Systems.Verlets;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Linq;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.NPCs.Cecitior;

public class CecitiorEye : ModNPC
{
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<Cecitior>()], quickUnlock: true);
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Cecitoma"),
            new FlavorTextBestiaryInfoElement(NPC.BestiaryKey()),
        });
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 19;
    }
    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        NPC.lifeMax = 3000;
        NPC.dontTakeDamage = true;
        NPC.damage = 0;
        NPC.noTileCollide = true;
        NPC.defense = 30;
        NPC.knockBackResist = 0;
        NPC.width = 32;
        NPC.height = 32;
        NPC.npcSlots = 1f;
        NPC.lavaImmune = true;
        NPC.noGravity = true;
        NPC.buffImmune[24] = true;
        NPC.buffImmune[BuffID.Ichor] = true;
        NPC.buffImmune[BuffID.Confused] = true;
        SoundStyle hit = EbonianSounds.fleshHit;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = hit;
        NPC.netAlways = true;
    }
    Verlet verlet;
    int animationOffsetTimer;
    float randRot;
    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (projectile.penetrate > 1)
            modifiers.FinalDamage.Scale(0.65f);
    }
    public override void OnSpawn(IEntitySource source)
    {
        randRot = Main.rand.NextFloat(MathHelper.Pi * 2);
        animationOffsetTimer = Main.rand.Next(60);
        NPC.frame.Y = NPC.frame.Height * 13;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC center = Main.npc[(int)NPC.ai[0]];
        if (NPC.IsABestiaryIconDummy || !center.active || center.type != NPCType<Cecitior>())
            return true;

        if (verlet is not null)
        {
            if (leftSiders.Contains((int)NPC.ai[1]))
                verlet.Update(NPC.Center, center.Center + new Vector2(center.localAI[0], center.localAI[1]));
            else
                verlet.Update(NPC.Center, center.Center - new Vector2(center.localAI[0], center.localAI[1]));
            verlet.gravity = (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.25f;
            verlet.Draw(spriteBatch, "NPCs/Cecitior/CecitiorChain");
        }
        return false;
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC center = Main.npc[(int)NPC.ai[0]];
        if (NPC.IsABestiaryIconDummy || !center.active || center.type != NPCType<Cecitior>())
            return;
        Texture2D a = Helper.GetTexture("NPCs/Cecitior/CecitiorChain_base").Value;
        Texture2D glow = Helper.GetTexture("NPCs/Cecitior/CecitiorChain_base_Glow").Value;
        Texture2D b = TextureAssets.Npc[Type].Value;
        Texture2D glow2 = Assets.ExtraSprites.Cecitior.CecitiorEye_Glow.Value;
        if (verlet is null)
            verlet = new(NPC.Center, 7, 16, 1, true, true, 13);
        else
        {
            spriteBatch.Draw(a, verlet.firstP.position - new Vector2(0, 20).RotatedBy(Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f) - screenPos, null, drawColor, Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f, a.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(glow, verlet.firstP.position - new Vector2(0, 20).RotatedBy(Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f) - screenPos, null, drawColor, Helper.FromAToB(verlet.firstP.position, verlet.points[5].position, reverse: true).ToRotation() - 1.57f, a.Size() / 2, 1, SpriteEffects.None, 0);
        }
        spriteBatch.Draw(b, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
        spriteBatch.Draw(glow2, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
    }
    public override void FindFrame(int frameHeight)
    {
        if (NPC.IsABestiaryIconDummy)
        {

            ++NPC.frameCounter;
            if (NPC.frameCounter % 5 == 0 && NPC.frameCounter < 550)
            {
                if (NPC.frame.Y < frameHeight * 15)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
            if (NPC.frameCounter % 5 == 0 && NPC.frameCounter > 550)
            {
                if (NPC.frame.Y < frameHeight * 15)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = frameHeight * 13;
            }
            if (NPC.frameCounter > 650)
                NPC.frameCounter = 0;
        }
        if (animationOffsetTimer < 0)
        {
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (center.ai[3] == 1 || frantic && center.ai[0] != 0) //frantically looking
            {
                ++NPC.frameCounter;
                if (NPC.frameCounter % 5 == 0 && NPC.frameCounter < 550)
                {
                    if (NPC.frame.Y < frameHeight * 15)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
                if (NPC.frameCounter % 5 == 0 && NPC.frameCounter > 550)
                {
                    if (NPC.frame.Y < frameHeight * 15)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = frameHeight * 13;
                }
                if (NPC.frameCounter > 650)
                    NPC.frameCounter = 0;
            }
            else //stare at player
            {
                ++NPC.frameCounter;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < frameHeight * 18 && NPC.frame.Y >= frameHeight * 16)
                        NPC.frame.Y += frameHeight;
                    else if (NPC.frame.Y < frameHeight * 16 || NPC.frame.Y >= frameHeight * 18)
                        NPC.frame.Y = 16 * frameHeight;
                }
            }
        }
    }

    public float Index
    {
        get => NPC.ai[1];
        set => NPC.ai[1] = value;
    }
    public float AIState
    {
        get => NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    public float AITimer
    {
        get => NPC.ai[3];
        set => NPC.ai[3] = value;
    }
    bool frantic;
    Vector2 focalPoint;
    private static readonly int[] leftSiders = new int[] { 0, 5, 4 };
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
            if (verlet is not null)
            {
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Vector2.Zero, Find<ModGore>("EbonianMod/CecitiorChainGore").Type);
                }
            }
    }
    public override void PostAI()
    {
        NPC center = Main.npc[(int)NPC.ai[0]];
        if (!center.active || center.type != NPCType<Cecitior>() || center.ai[0] == -12124)
            return;
        NPC.netUpdate = center.netUpdate;
    }
    public override void AI()
    {

        animationOffsetTimer--;
        float angle = Helper.CircleDividedEqually(NPC.ai[1], 6) + MathHelper.ToRadians(15);
        bool leftie = leftSiders.Contains((int)NPC.ai[1]);
        NPC center = Main.npc[(int)NPC.ai[0]];
        Player player = Main.player[center.target];
        int eyeCount = 0;
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.active && npc.type == Type)
                eyeCount++;
        }
        bool halfEyesPhase2 = eyeCount <= 3;
        if (!center.active || center.type != NPCType<Cecitior>() || center.ai[0] == -12124)
        {
            NPC.life = 0;
            NPC.netUpdate = true;
            return;
        }
        if (center.ai[3] == 1 || frantic)
        {
            NPC.rotation = randRot;
        }
        else
        {
            NPC.rotation = Helper.FromAToB(NPC.Center, focalPoint).ToRotation() + MathHelper.Pi;
        }
        if (center.ai[0] != 0)
            NPC.dontTakeDamage = false;

        switch (center.ai[0])
        {
            case 0:
                if (center.ai[1] > 50 && center.ai[1] < 170)
                {
                    focalPoint = center.Center;
                    if (leftie)
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(160).RotatedBy(angle + MathF.Sin(center.ai[1])), false) / 10f;
                    else
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center - new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(160).RotatedBy(angle - MathF.Sin(center.ai[1])), false) / 10f;
                }
                else
                {
                    NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle + ToRadians(center.ai[2])), false) / 10f;
                    focalPoint = Vector2.Lerp(focalPoint, player.Center, 0.45f);
                }
                break;
            case 1:
                NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle + ToRadians(center.ai[2])), false) / 10f;
                focalPoint = player.Center;
                AITimer = 0;
                break;
            case 2:
                AITimer++;
                if (AITimer > 100 && AITimer < 200)
                {
                    if (leftie)
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(100).RotatedBy(angle), false) / 10f;
                    else
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center - new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(100).RotatedBy(angle), false) / 10f;
                    frantic = false;
                    focalPoint = NPC.Center + Helper.FromAToB(center.Center, NPC.Center) * 400;
                    animationOffsetTimer++;
                    if ((AITimer == 130 || (halfEyesPhase2 && AITimer == 115)) && leftie)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                        MPUtils.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(center.Center, NPC.Center), ProjectileType<EyeVFX>(), 0, 0);
                        MPUtils.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(center.Center, NPC.Center) * 3, ProjectileType<CecitiorEyeP>(), 30, 0);
                    }

                    if ((AITimer == 160 || (halfEyesPhase2 && AITimer == 145)) && !leftie)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                        MPUtils.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(center.Center, NPC.Center), ProjectileType<EyeVFX>(), 0, 0);
                        MPUtils.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(center.Center, NPC.Center) * 3, ProjectileType<CecitiorEyeP>(), 30, 0);
                    }

                }
                if (AITimer < 61 && AITimer > 1)
                {
                    frantic = false;
                    focalPoint = NPC.Center + Vector2.UnitY * 100;
                    NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2((NPC.ai[1] - 3) * 200, -100), false) / 10f;
                    if (AITimer % 30 == 0)
                    {
                        SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                        Projectile a = MPUtils.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * 5, ProjectileID.BloodArrow, 15, 0);
                        if (a is not null)
                        {
                            a.hostile = true;
                            a.friendly = false;
                            a.netUpdate = true;
                        }
                    }
                }
                if (AITimer > 200)
                    NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle), false) / 5f;
                break;
            case 3:
                focalPoint = player.Center;
                if (center.ai[1] > 115)
                    NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle), false) / 5f;
                else
                {
                    if (leftie)
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(100).RotatedBy(angle), false) / 10f;
                    else
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center - new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(100).RotatedBy(angle), false) / 10f;

                }

                break;
            case 4:
                if (center.ai[1] > 115)
                    NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle), false) / 5f;
                else
                {
                    if (leftie)
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(100).RotatedBy(angle), false) / 10f;
                    else
                        NPC.velocity = Helper.FromAToB(NPC.Center, center.Center - new Vector2(center.localAI[0], center.localAI[1]) + new Vector2(100).RotatedBy(angle), false) / 10f;

                }
                focalPoint = player.Center;
                break;
            case 5:
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center + new Vector2(200).RotatedBy(angle + MathHelper.ToRadians(center.ai[2] * (halfEyesPhase2 ? 3 : 1))), false) / 3;
                focalPoint = player.Center;
                if (center.ai[1] % (halfEyesPhase2 ? 20 : 50) == 0 && center.ai[1] > 1)
                {
                    SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                    MPUtils.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(NPC.Center, focalPoint) * 7f, ProjectileType<CecitiorTeeth>(), 30, 0);
                }
                break;
            case 6:
                NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle), false) / 10f;
                focalPoint = player.Center;
                break;
            case 7:
                NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle), false) / 10f;
                focalPoint = player.Center;
                break;
            case 8:
                NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(100).RotatedBy(angle), false) / 10f;
                focalPoint = player.Center;
                break;
        }
    }
}
