﻿using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using System.IO;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using static System.Formats.Asn1.AsnWriter;
//
using EbonianMod.Items.Weapons.Melee;
//using EbonianMod.Worldgen.Subworlds;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.Friendly.Crimson;
using System.Reflection.Metadata;
using EbonianMod.Projectiles;
using ReLogic.Utilities;
using Terraria.UI;
using EbonianMod.Common.Achievements;
using EbonianMod.Common.Systems;
using static System.Net.Mime.MediaTypeNames;
using Terraria.GameContent.Golf;
using System.Linq;
using Steamworks;
using Terraria.GameContent.UI.Elements;
using EbonianMod.NPCs.Corruption;
using static EbonianMod.Helper;
using EbonianMod.Common.Systems.Misc;
using EbonianMod.Items.BossTreasure;
using EbonianMod.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using EbonianMod.Items.Pets.Hightoma;
using EbonianMod.Items.Pets;
using EbonianMod.Items.Tiles.Trophies;
using EbonianMod.Items.Misc;
using EbonianMod.Bossbars;
using EbonianMod.Items.Tiles;
using EbonianMod.Items.Armor.Vanity;

namespace EbonianMod.NPCs.Cecitior
{
    [AutoloadBossHead]
    public class Cecitior : ModNPC
    {
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Cecitoma"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.Cecitior.Bestiary"),
            });
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            Main.npcFrameCount[NPC.type] = 7;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(new CommonDrop(ItemType<CecitiorMaterial>(), 1, 40, 60));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<SelfStab>(), 1));
            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.Common(ItemType<CeciMask>(), 4));
            npcLoot.Add(ItemDropRule.Common(ItemType<CecitiorTrophy>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<CecitiorPet>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<CecitiorRelic>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<DapperCecitiorI>(), 5));

            npcLoot.Add(ItemDropRule.BossBag(ItemType<CecitiorBag>()));
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 8000;
            NPC.damage = 40;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.defense = 27;
            NPC.knockBackResist = 0;
            NPC.width = 118;
            NPC.height = 100;
            NPC.rarity = 999;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = true;
            SoundStyle death = EbonianSounds.cecitiorDie;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = death;
            NPC.buffImmune[24] = true;
            NPC.buffImmune[BuffID.Ichor] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.netAlways = true;
            NPC.hide = true;
            NPC.value = Item.buyPrice(0, 10);
            NPC.BossBar = GetInstance<CecitiorBar>();
            //NPC.alpha = 255;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }
        Verlet[] verlet = new Verlet[10];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < attackNum; i++)
            {
                pattern[i] = Main.rand.Next(1, attackNum + 1);
            }
            for (int i = 0; i < 10; i++)
            {
                float scale = 2;
                switch (i)
                {
                    case 1:
                    case 5:
                        scale = 4;
                        break;
                    case 2:
                    case 6:
                        scale = 2;
                        break;
                    case 3:
                    case 7:
                        scale = 3;
                        break;
                    case 4:
                    case 8:
                        scale = 1;
                        break;
                }
                verlet[i] = new(NPC.Center, 2, 15, 1 * scale, true, true, (int)(5f * scale));
            }
        }
        float shakeVal;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = Helper.GetTexture("NPCs/Cecitior/Cecitior_Glow");
            Texture2D tex = TextureAssets.Npc[Type].Value;
            Vector2 shakeOffset = Main.rand.NextVector2Circular(shakeVal, shakeVal);
            if (verlet[0] != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = Vector2.Zero;
                    Vector2 offset2 = Vector2.Zero;
                    float scale = 2;
                    switch (i)
                    {
                        case 1:
                            scale = 4;
                            offset = new Vector2(4, 28);
                            offset2 = new Vector2(-7, -10);
                            break;
                        case 2:
                            scale = 2;
                            offset = new Vector2(10, -20);
                            offset2 = new Vector2(1, 20);
                            break;
                        case 3:
                            scale = 3;
                            offset = new Vector2(-15, 10);
                            offset2 = new Vector2(10, -20);
                            break;
                        case 4:
                            scale = 1;
                            offset = new Vector2(1, 20);
                            offset2 = new Vector2(4, -32);
                            break;
                        case 5:
                            scale = 3;
                            offset = new Vector2(-20, -20);
                            offset2 = new Vector2(-6, 32);
                            break;
                        case 6:
                            scale = 2;
                            offset = new Vector2(10, -10);
                            offset2 = new Vector2(4, 12);
                            break;
                        case 7:
                            scale = 4;
                            offset = new Vector2(10, -20);
                            offset2 = new Vector2(-4, 32);
                            break;
                        case 8:
                            scale = 2;
                            offset = new Vector2(10, -25);
                            offset2 = new Vector2(-14, 32);
                            break;
                        case 9:
                            scale = 1;
                            offset = new Vector2(1, -20);
                            offset2 = new Vector2(4, 22);
                            break;
                    }
                    verlet[i].Update(NPC.Center + offset2 - openOffset, NPC.Center + openOffset + new Vector2(30, 4) + offset);
                    if (verlet[i].segments[10].cut)
                        verlet[i].Draw(spriteBatch, "Extras/maroon", scale: scale);
                }
            }
            if (claw[0].verlet != null && phase2)
            {
                Texture2D trail = Helper.GetTexture("NPCs/Cecitior/Hook/CecitiorHook_1");
                for (int i = 0; i < claw.Length; i++)
                {
                    for (int num16 = claw[i].oldPosition.Length - 1; num16 > 0; num16--)
                    {
                        claw[i].oldPosition[num16] = claw[i].oldPosition[num16 - 1];
                    }
                    claw[i].oldPosition[0] = claw[i].position;

                    for (int num16 = claw[i].oldRotation.Length - 1; num16 > 0; num16--)
                    {
                        claw[i].oldRotation[num16] = claw[i].oldRotation[num16 - 1];
                    }
                    claw[i].oldRotation[0] = claw[i].verlet.endRot;

                    var fadeMult = 1f / claw[i].oldPosition.Length;
                    for (int j = 0; j < claw[i].oldPosition.Length; j++)
                    {
                        float mult = (1f - fadeMult * j);
                        float mult2 = (1f - fadeMult * (j));
                        if (j > 0)
                        {
                            mult2 = (1f - fadeMult * (j - 1));
                            for (float k = 0; k < 5; k++)
                            {
                                Vector2 pos = Vector2.Lerp(claw[i].oldPosition[j], claw[i].oldPosition[j - 1], (float)(k / 5));
                                Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, Color.Maroon * 0.05f * MathHelper.Lerp(mult2, mult, (float)(k / 5)), claw[i].oldRotation[j], trail.Size() / 2, MathHelper.Lerp(mult2, mult, (float)(k / 5)), SpriteEffects.None, 0);
                            }
                        }
                    }

                    claw[i].verlet.Update(NPC.Center + (new Vector2(20 + i * 6f, (i - 1) * 10).RotatedBy(openRotation) + openOffset) * (i == 2 ? -1 : 1), claw[i].position);
                    if (i == (int)AITimer3)
                    {
                        if (AIState == Phase2ClawGrab && AITimer2 == 1)
                        {
                            claw[i].verlet.Draw(spriteBatch, "NPCs/Cecitior/Hook/CecitiorHook_0", endTex: "NPCs/Cecitior/Hook/CecitiorHook_8");
                            claw[i].verlet.Draw(spriteBatch, "Extras/Empty", endTex: "NPCs/Cecitior/Hook/CecitiorHook_8_Glow", useColor: true, color: Color.White);
                        }
                        else
                        {
                            claw[i].verlet.Draw(spriteBatch, "NPCs/Cecitior/Hook/CecitiorHook_0", endTex: "NPCs/Cecitior/Hook/CecitiorHook_" + hookFrame);
                            claw[i].verlet.Draw(spriteBatch, "Extras/Empty", endTex: "NPCs/Cecitior/Hook/CecitiorHook_" + hookFrame + "_Glow", useColor: true, color: Color.White);
                        }
                    }
                    else
                    {
                        claw[i].verlet.Draw(spriteBatch, "NPCs/Cecitior/Hook/CecitiorHook_0", endTex: "NPCs/Cecitior/Hook/CecitiorHook_" + hookFrame);
                        claw[i].verlet.Draw(spriteBatch, "Extras/Empty", endTex: "NPCs/Cecitior/Hook/CecitiorHook_" + hookFrame + "_Glow", useColor: true, color: Color.White);
                    }

                    /*Texture2D tex = Helper.GetTexture("Projectiles/Cecitior/CecitiorBombThing_Extra");
                    //Main.EntitySpriteDraw(tex, claw[i].position - Main.screenPosition, null, Color.Black * 0.1f * claw[i].orbAlpha, Main.GameUpdateCount * 0.03f, tex.Size() / 2, 0.7f, SpriteEffects.None);
                    Main.spriteBatch.Reload(BlendState.Additive);
                    //Main.EntitySpriteDraw(tex, claw[i].position - Main.screenPosition, null, Color.Gold * 0.7f * claw[i].orbAlpha, Main.GameUpdateCount * 0.03f, tex.Size() / 2, 0.7f, SpriteEffects.None);
                    //Main.EntitySpriteDraw(tex, claw[i].position - Main.screenPosition, null, Color.White * 0.3f * claw[i].orbAlpha, Main.GameUpdateCount * 0.03f, tex.Size() / 2, 0.7f, SpriteEffects.None);
                    Main.spriteBatch.Reload(BlendState.AlphaBlend);*/

                }

            }

            Texture2D teeth = Helper.GetTexture("NPCs/Cecitior/CecitiorTeeth");
            Texture2D partTeeth = Helper.GetTexture("NPCs/Cecitior/CecitiorTeeth2");
            Texture2D part = Helper.GetTexture("NPCs/Cecitior/Cecitior_Part");
            Texture2D partGlow = Helper.GetTexture("NPCs/Cecitior/Cecitior_Part_Glow");
            if (NPC.frame.Y == 6 * 102)//(openOffset.Length() > 0.25f || openOffset.Length() < -0.25f || openRotation != 0)
            {
                spriteBatch.Draw(teeth, NPC.Center + shakeOffset - openOffset - new Vector2(0, -2) - screenPos, null, new Color(Lighting.GetSubLight(NPC.Center - openOffset)), NPC.rotation, teeth.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(partTeeth, NPC.Center - shakeOffset + new Vector2(30, 4) + openOffset - screenPos, null, new Color(Lighting.GetSubLight(NPC.Center + openOffset)), openRotation, partTeeth.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                if (verlet[0] != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 offset = Vector2.Zero;
                        Vector2 offset2 = Vector2.Zero;
                        float scale = 2;
                        switch (i)
                        {
                            case 1:
                                scale = 4;
                                offset = new Vector2(4, 28);
                                offset2 = new Vector2(-7, -10);
                                break;
                            case 2:
                                scale = 2;
                                offset = new Vector2(10, -20);
                                offset2 = new Vector2(1, 20);
                                break;
                            case 3:
                                scale = 3;
                                offset = new Vector2(-15, -10);
                                offset2 = new Vector2(10, -20);
                                break;
                            case 4:
                                scale = 1;
                                offset = new Vector2(1, 20);
                                offset2 = new Vector2(4, 32);
                                break;
                        }
                        //verlet[i].Update(NPC.Center + offset2, NPC.Center + openOffset + new Vector2(30, 4) + offset);
                        if (!verlet[i].segments[10].cut)
                            verlet[i].Draw(spriteBatch, "Extras/maroon", scale: scale);
                    }
                }
                spriteBatch.Draw(part, NPC.Center - shakeOffset + new Vector2(30, 4) + openOffset - screenPos, null, new Color(Lighting.GetSubLight(NPC.Center + new Vector2(30, 4) + openOffset) * 1.25f), openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(partGlow, NPC.Center - shakeOffset + new Vector2(30, 4) + openOffset - screenPos, null, Color.White, openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);

                spriteBatch.Draw(tex, NPC.Center + shakeOffset - openOffset - screenPos, NPC.frame, new Color(Lighting.GetSubLight(NPC.Center - openOffset) * 1.25f), NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(glow, NPC.Center + shakeOffset - openOffset - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                return false;
            }
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        int hookFrame = 1;
        public override void FindFrame(int frameHeight)
        {
            if (openOffset.Length() > 1 || openOffset.Length() < -1 || openRotation != 0)
                NPC.frame.Y = frameHeight * 6;
            else
            if (++NPC.frameCounter % 5 == 0)
            {
                hookFrame++;
                if (hookFrame > 7 || hookFrame < 1)
                    hookFrame = 1;
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        public override bool CheckDead()
        {
            Player player = Main.player[NPC.target];
            if (!deathAnim)
            {
                for (int i = 0; i < claw.Length; i++)
                {
                    //claw[i].orbAlpha = 0f;
                }
                open = false;
                deathAnim = true;
                NPC.life = 1;
                savedPos = NPC.Center;
                NPC.velocity = Vector2.Zero;
                if (tongue != null)
                    tongue.Kill();
                AIState = PrePreDeath;
                AITimer = 0;
                openRotation = 0f;
                rotation = 0f;
                openOffset = Vector2.Zero;
                AITimer2 = 0;
                EbonianSystem.ChangeCameraPos(NPC.Center, 180, new ZoomInfo(2, 1.1f, InOutElastic, InOutCirc), 1.5f, InOutQuart);
                NPC.dontTakeDamage = true;
            }
            return AIState == Death;
        }
        float rotation, openRotation;
        bool open;
        int eyeType = NPCType<CecitiorEye>();
        Vector2 openOffset;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(openOffset);
            writer.Write(open);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(openRotation);
            writer.Write(rotation);
            writer.Write(lastAi);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            openOffset = reader.ReadVector2();
            open = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            openRotation = reader.ReadSingle();
            rotation = reader.ReadSingle();
            lastAi = reader.Read();
        }
        public float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        float VerletAlpha = 1;
        private float AITimer2 = 0;
        private float AITimer3 = 0;
        int lastAi;
        Projectile tongue = null;
        //attacks (for ctrl f)
        const int PhaseTransition = -4, PrePreDeath = -3, Death = -2, PreDeath = -1, Intro = 0, Idle = 1, EyeBehaviour = 2, Chomp = 3, Teeth = 4, EyeBehaviour2 = 5, LaserRain = 6, ThrowUpBlood = 7, Tongue = 8,
            Phase2ThrowUpEyes = 9, Phase2Claw = 10, Phase2ClawGrab = 11, Phase2ClawMultiple = 12, Phase2GrabBomb = 13, Phase2ClawBodySlam = 14;

        public static SlotId cachedSound;
        public SlotId openSound;
        Vector2 savedPos, savedClawPos;
        const int attackNum = 14;
        public float[] pattern = new float[attackNum];
        public float[] oldPattern = new float[attackNum];
        float Next = 2;
        float OldState;
        public void GenerateNewPattern()
        {
            for (int i = 0; i < attackNum - 1; i++)
            {
                pattern[i] = Main.rand.Next(2, attackNum + 1);
            }
            for (int i = 0; i < attackNum - 1; i++)
            {
                int attempts = 0;
                while (++attempts < 35 && (pattern.Count(p => p == pattern[i]) != 1 || pattern[i] == 0 || oldPattern.Last() == pattern.First()))
                {
                    for (int j = 0; j < attackNum - 1; j++)
                    {
                        if (pattern[j] == pattern[i] && i != j)
                            pattern[i] = Main.rand.Next(2, attackNum + 1);
                    }
                    //pattern[i] = Main.rand.Next(2, attackNum + 1);
                }
            }
            Next = pattern.First();
        }
        public void SwitchToRandom()
        {
            if (pattern.Any())
            {
                if (AIState == pattern[attackNum - 2])
                {
                    GenerateNewPattern();
                    Next = pattern.First();
                }
                else if (AIState == Intro)
                {
                    GenerateNewPattern();
                    Next = pattern.First();
                }
                else
                {
                    oldPattern = pattern;
                    Next = pattern[pattern.ToList().IndexOf((int)OldState) + 1];
                }
            }
        }
        void ResetState()
        {
            AITimer = 0;
            AITimer2 = 0;
            AITimer3 = 0;
            SwitchToRandom();
            shakeVal = 0;
            AIState = Idle;
            NPC.damage = 0;
            rotation = 0;
            openRotation = 0;
            open = false;
            openOffset = Vector2.Zero;
            NPC.velocity = Vector2.Zero;
        }
        bool phase2;
        public struct CecitiorClaw
        {
            public Vector2 position;
            public Vector2[] oldPosition = new Vector2[25];
            public float[] oldRotation = new float[25];
            public Verlet verlet;
            public CecitiorClaw(Vector2 _position, Verlet _verlet)
            {
                position = _position;
                verlet = _verlet;
            }
        }
        public CecitiorClaw[] claw = new CecitiorClaw[3];
        bool deathAnim;
        bool halfEyesPhase2;
        int oldHP;
        public override void AI()
        {
            if (!deathAnim && NPC.life <= 1)
            {
                for (int i = 0; i < claw.Length; i++)
                {
                    //claw[i].orbAlpha = 0f;
                }
                open = false;
                deathAnim = true;
                NPC.life = 1;
                savedPos = NPC.Center;
                NPC.velocity = Vector2.Zero;
                if (tongue != null)
                    tongue.Kill();
                AIState = PrePreDeath;
                AITimer = 0;
                openRotation = 0f;
                rotation = 0f;
                openOffset = Vector2.Zero;
                AITimer2 = 0;
                EbonianSystem.ChangeCameraPos(NPC.Center, 180, new ZoomInfo(2, 1.1f, InOutElastic, InOutCirc), 1.5f, InOutQuart);
                NPC.dontTakeDamage = true;
            }
            if (oldHP == 0)
                oldHP = NPC.life;
            //if (AIState != Intro && AIState != PhaseTransition && AIState != Idle && AIState != PrePreDeath && AIState != PreDeath && AIState != Death)
            //  AIState = Teeth; //DEBUG
            if ((pattern.Contains(Intro) || pattern.Contains(Idle)) && AIState == Idle)
                GenerateNewPattern();
            if (AIState != Idle && AIState != Intro && AIState != PrePreDeath && AIState != PreDeath && AIState != Death && AIState != PhaseTransition)
                OldState = AIState;
            Player player = Main.player[NPC.target];
            if (!cachedSound.IsValid || !SoundEngine.TryGetActiveSound(cachedSound, out var activeSound) || !activeSound.IsPlaying)
            {
                cachedSound = SoundEngine.PlaySound(EbonianSounds.cecitiorIdle.WithVolumeScale(0.35f), NPC.Center);
            }
            if (SoundEngine.TryGetActiveSound(cachedSound, out var __activeSound))
            {
                __activeSound.Pitch = Lerp(-1, 1, NPC.velocity.Length() / 13);
            }
            int eyeCount = 0;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.active && npc.type == eyeType)
                    eyeCount++;
            }

            if (eyeCount == 0 && !phase2 && AIState != Intro)
            {
                AITimer = 0;
                AITimer2 = 0;
                open = false;
                openRotation = 0;
                rotation = 0;
                openOffset = Vector2.Zero;
                AIState = PhaseTransition;
                NPC.velocity = Vector2.Zero;
                phase2 = true;
            }
            if (!deathAnim)
                if (!player.active || player.dead)// || !player.ZoneCorrupt)
                {
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];
                    if (NPC.HasValidTarget)
                    {
                        if (AIState != Intro)
                            AIState = Idle;
                        AITimer = 0;
                    }
                    if (!player.active || player.dead)// || !player.ZoneCrimson)
                    {
                        AIState = -12124;
                        shakeVal = 0;
                        open = false;
                        NPC.velocity = new Vector2(0, 10f);

                        if (claw[0].verlet != null && phase2)
                        {
                            claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                            claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                            claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        }

                        if (NPC.timeLeft > 60)
                        {
                            NPC.timeLeft = 60;
                        }
                        return;
                    }
                }
            NPC.localAI[0] = openOffset.X;
            NPC.localAI[1] = openOffset.Y;
            if (open)
            {
                NPC.ai[3] = 1; //letting the eyes know its open.
                NPC.damage = 0;
            }
            else
            {
                NPC.ai[3] = 0;
                if (!(AIState == Chomp && AITimer2 % 2 != 0) && !(AIState == PreDeath && (AITimer2 == 1 || AITimer2 == 3)))
                    openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.5f);
                //NPC.damage = 40;
                if ((openOffset.Length() < 2.5f && openOffset.Length() > 1f) || (openOffset.Length() > -2.5f && openOffset.Length() < -1f))
                {
                    if (SoundEngine.TryGetActiveSound(openSound, out var sound) && AITimer > 60)
                    {
                        sound.Stop();
                    }
                    SoundEngine.PlaySound(EbonianSounds.cecitiorClose, NPC.Center);
                    EbonianSystem.ScreenShakeAmount = 5;
                }
                if (openOffset != Vector2.Zero && AIState != ThrowUpBlood && AIState != LaserRain && NPC.frame.Y == 6 * 102)
                    if (player.Center.Distance(NPC.Center - openOffset) < 75 || player.Center.Distance(NPC.Center + openOffset) < 75)
                        player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 150, 0);
            }
            //open = Main.mouseRight;
            //openRotation = NPC.rotation;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, rotation, 0.35f);
            if (AIState == Death)
            {
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                for (int i = 0; i < 7; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2Circular(20, 20), ProjectileType<CecitiorBombThing>(), 30, 0);
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(30, 75), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), new Vector2(Main.rand.NextFloat(-1, 1), -1) * Main.rand.NextFloat(1, 15), 814, 10, 0);
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(30, 75), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), new Vector2(Main.rand.NextFloat(-1, 1), -1) * Main.rand.NextFloat(1, 15), ProjectileType<Gibs>(), 10, 0);
                    Projectile.NewProjectile(null, NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(20, 45), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), Vector2.Zero, ProjectileType<BloodExplosionWSprite>(), 0, 0);
                    Projectile.NewProjectile(null, NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(20, 45), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                }
                EbonianSystem.ScreenShakeAmount = 10f;
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + openOffset, Vector2.UnitX * 5, Find<ModGore>("EbonianMod/Cecitior1").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -Vector2.UnitX * 5, Find<ModGore>("EbonianMod/Cecitior2").Type, NPC.scale);
                // if (!EbonianAchievementSystem.acquiredAchievement[2])
                //   InGameNotificationsTracker.AddNotification(new EbonianAchievementNotification(2));
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_Death(), claw[i].position, Main.rand.NextVector2CircularEdge(15, 15) * Main.rand.NextFloat(0.75f, 1f), ProjectileType<ClawGore>(), 0, 0, -1, claw[i].verlet.firstP.position.X, claw[i].verlet.firstP.position.Y);
                }
                /*for (int j = 0; j < claw.Length; j++)
                    for (int i = 0; i < claw[j].verlet.points.Count; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), claw[j].verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), claw[j].verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimorrhageChain").Type, NPC.scale);
                        if (i == 0)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), claw[j].verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/Crimorrhage2").Type, NPC.scale);
                            if (Main.rand.NextBool())
                                Gore.NewGore(NPC.GetSource_Death(), claw[j].verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/Crimorrhage1").Type, NPC.scale);
                            if (Main.rand.NextBool())
                                Gore.NewGore(NPC.GetSource_Death(), claw[j].verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/Bone2").Type, NPC.scale);
                            if (Main.rand.NextBool())
                                Gore.NewGore(NPC.GetSource_Death(), claw[j].verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk5").Type, NPC.scale);
                            if (Main.rand.NextBool())
                                Gore.NewGore(NPC.GetSource_Death(), claw[j].verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk4").Type, NPC.scale);
                        }
                    }*/
                NPC.dontTakeDamage = false;
                NPC.life = 0;
                GetInstance<DownedBossSystem>().downedCecitior = true;
                SoundEngine.PlaySound(EbonianSounds.evilOutro);
                if (SoundEngine.TryGetActiveSound(cachedSound, out var _activeSound))
                    _activeSound.Stop();
                NPC.checkDead();
            }
            else if (AIState == PrePreDeath)
            {
                if (claw[0].verlet != null && phase2)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(250, -265).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(0.1f, 0.5f)) * 0.2f + Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(0.015f)), 0.025f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(265, 245).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(0.1f, 0.5f)) * 0.2f + Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(.015f)), 0.025f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-210, 255).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(0.1f, 0.5f)) * 0.2f + Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(.015f)), 0.025f);
                }
                if (rotation == 0 || AITimer > 0)
                    AITimer++;
                if (AITimer == 1)
                    open = false;
                if (AITimer == 0 || AITimer > 150)
                {
                    rotation = MathHelper.Lerp(rotation, 0, 0.3f);
                    openRotation = MathHelper.Lerp(openRotation, 0, 0.3f);
                    openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.3f);
                }
                if (AITimer >= 100 && AITimer < 115)
                {
                    openOffset.X++;
                    openRotation += MathHelper.ToRadians(2);
                    rotation -= MathHelper.ToRadians(2);
                }
                if (AITimer == 1)
                {
                    SoundEngine.PlaySound(EbonianSounds.fleshHit, NPC.Center);
                    SoundEngine.PlaySound(EbonianSounds.terrortomaFlesh, NPC.Center);
                }
                if (AITimer > 115 && AITimer < 160)
                {
                    if (AITimer % 5 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-1, 1), -1) * Main.rand.NextFloat(1, 15), 814, 10, 0);
                    }
                    if (AITimer % (AITimer < 130 ? 2 : 0) == 0)
                        for (int i = 0; i < 3; i++)
                        {
                            Dust.NewDustPerfect(NPC.Center - openOffset, DustID.Blood, new Vector2(Main.rand.NextFloat(0, 1), -1) * Main.rand.NextFloat(1, 15), Scale: Main.rand.NextFloat(1, 2)).noGravity = false;
                            Dust.NewDustPerfect(NPC.Center + openOffset, DustID.Blood, new Vector2(Main.rand.NextFloat(-1, 0), -1) * Main.rand.NextFloat(1, 15), Scale: Main.rand.NextFloat(1, 2)).noGravity = false;
                        }
                }
                NPC.Center = savedPos;
                if (AITimer < 100 && AITimer > 0)
                    NPC.Center += Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, (20 - AITimer * 0.2f).Safe());
                if (AITimer > 170)
                {
                    AIState = PreDeath;
                    AITimer = 0;
                    savedPos = NPC.Center;
                }
            }
            else if (AIState == PreDeath)
            {
                AITimer++;
                if (AITimer % 16 - (AITimer3 * 2) == 0 && AITimer3 > 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2Circular(20, 20), ProjectileType<CecitiorBombThing>(), 30, 0);
                        if (Main.rand.NextBool())
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(30, 75), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), new Vector2(Main.rand.NextFloat(-1, 1), -1) * Main.rand.NextFloat(1, 10), ProjectileType<Gibs>(), 0, 0);
                        else
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(30, 75), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), new Vector2(Main.rand.NextFloat(-1, 1), -1) * Main.rand.NextFloat(1, 10), 814, 10, 0);
                        Projectile.NewProjectile(null, NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(30, 75), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), Vector2.Zero, ProjectileType<BloodExplosionWSprite>(), 0, 0);
                    }
                    Projectile.NewProjectile(null, NPC.Center + (openOffset + new Vector2(Main.rand.NextFloat(30, 75), Main.rand.NextFloat(-45, 45))) * Main.rand.NextFloatDirection(), Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                }
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambience");
                open = true;
                openOffset = Vector2.Lerp(openOffset, new Vector2(100 + MathF.Sin(Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(2, 9)) * Main.rand.NextFloat(50, 80), MathF.Sin(Main.GlobalTimeWrappedHourly * Main.rand.NextFloat(2, 5)) * Main.rand.NextFloat(20, 40)), 0.15f);
                if (claw[0].verlet != null)
                {
                    if (AITimer == 1)
                        AITimer2 = Main.rand.NextFloat(MathHelper.Pi * 2);
                    if (AITimer < 20)
                    {
                        NPC.velocity *= 0.9f;
                        claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + new Vector2(45, -185).RotatedBy(AITimer2), 0.3f);
                        claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + new Vector2(0, -200).RotatedBy(AITimer2), 0.3f);
                        claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center + new Vector2(-45, -185).RotatedBy(AITimer2), 0.3f);
                    }
                    if (AITimer == 20)
                    {
                        for (int i = 0; i < claw.Length; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), claw[i].position + Vector2.UnitY.RotatedBy(AITimer2) * 110, -Vector2.UnitY.RotatedBy(AITimer2), ProjectileType<CecitiorClawSlash>(), 30, 0, ai2: 1);
                        }
                    }
                    if (AITimer >= 20)
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 10, 0.05f);
                        for (int i = 0; i < claw.Length; i++)
                        {
                            claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center - new Vector2(45, -170).RotatedBy(AITimer2), 0.05f);
                            claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center - new Vector2(0, -200).RotatedBy(AITimer2), 0.05f);
                            claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - new Vector2(-45, -170).RotatedBy(AITimer2), 0.05f);
                        }
                    }
                    if (AITimer >= 26)
                    {
                        if (AITimer3 < 6)
                        {
                            if (verlet[0] != null && AITimer3 < 4)
                            {
                                verlet[(int)AITimer3].segments[10].cut = true;
                                verlet[(int)AITimer3].stiffness = 1;

                                verlet[(int)AITimer3 + 5].segments[10].cut = true;
                                verlet[(int)AITimer3 + 5].stiffness = 1;
                            }
                            AITimer = 0;
                            AITimer3++;
                            savedPos = NPC.Center;
                        }
                        else
                        {
                            verlet[9].segments[10].cut = true;
                            verlet[9].stiffness = 1;
                            AITimer3 = 0;
                            AITimer2 = 0;
                            AIState = Death;
                            AITimer = 0;
                        }
                    }
                }
            }
            else if (AIState == PhaseTransition)
            {
                AITimer++;
                if (AITimer == 1)
                {
                    savedPos = NPC.Center;
                    NPC.velocity = Vector2.Zero;
                    //EbonianSystem.ChangeCameraPos(NPC.Center, 80, 1.2f);
                    EbonianSystem.ChangeCameraPos(NPC.Center, 110, null, 1.5f, InOutSine);
                }
                if (AITimer == 63)
                    EbonianSystem.ChangeZoom(50, new ZoomInfo(2, 1f, InOutSine, InOutSine, true));
                if (AITimer < 60)
                {
                    NPC.dontTakeDamage = true;
                    NPC.Center = savedPos + Main.rand.NextVector2Circular(AITimer * 0.25f, AITimer * 0.25f);
                }
                if (AITimer < 53 && AITimer > 40)
                {
                    EbonianSystem.ScreenShakeAmount = (AITimer - 40) * 0.1f;
                }
                if (AITimer == 63)
                {
                    if (NPC.life > 1)
                        NPC.dontTakeDamage = false;
                    for (int i = 0; i < 3; i++)
                    {
                        claw[i] = new CecitiorClaw(NPC.Center, new Verlet(NPC.Center, 12, 22, 0.15f, stiffness: 50));
                    }
                    SoundEngine.PlaySound(EbonianSounds.fleshHit with { Pitch = -0.3f, PitchVariance = 0.2f }, player.Center);

                    for (int k = 0; k < 25; k++)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.Blood, Vector2.UnitX.RotatedByRandom(1) * Main.rand.NextFloat(7, 15), 0, default, Main.rand.NextFloat(1, 2));
                    }
                    for (int k = 0; k < 25; k++)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.Blood, -Vector2.UnitX.RotatedBy(0.25f).RotatedByRandom(0.5f) * Main.rand.NextFloat(7, 15), 0, default, Main.rand.NextFloat(1, 2));
                    }

                }
                if (AITimer > 63)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                }
                if (AITimer >= 95)
                {
                    GenerateNewPattern();
                    ResetState();
                    if (NPC.life <= 1)
                    {
                        AIState = PrePreDeath;
                    }
                    else
                        AIState = Phase2ClawGrab;
                }
            }
            else if (AIState == Intro)
            {
                AITimer++;
                if (AITimer == 1)
                {
                    GenerateNewPattern();
                    NPC.boss = true;
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMiniboss");
                    EbonianSystem.ChangeCameraPos(NPC.Center, 260, new ZoomInfo(2, 1.1f, InOutElastic, InOutCirc), 1.5f, InOutQuart);
                    for (int i = 0; i < 200; i++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6), Scale: 3);
                    }
                }
                if (AITimer == 55)
                {
                    openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                    for (int i = 0; i < 6; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 6) + MathHelper.ToRadians(15);
                        NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(1).RotatedBy(angle), NPCType<CecitiorEye>(), 0, NPC.whoAmI, i);
                    }
                }
                if (AITimer == 80)
                {
                    EbonianSystem.ScreenShakeAmount = 10f;
                    SoundEngine.PlaySound(EbonianSounds.terrortomaFlesh, NPC.Center);
                }
                if (AITimer >= 60 && AITimer <= 160)
                {
                    open = true;
                    if (AITimer >= 80 && AITimer % 10 == 0)
                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                    if (openOffset.X < 30)
                    {
                        openOffset.X += 2.5f;
                    }
                    if (AITimer > 80)
                    {
                        openRotation = MathHelper.ToRadians(MathF.Sin(AITimer * 1.5f) * 15);
                        NPC.rotation = MathHelper.ToRadians(MathF.Sin(-AITimer * 1.5f) * 15);
                        rotation = MathHelper.ToRadians(MathF.Sin(-AITimer * 1.5f) * 15);
                    }
                }
                if (AITimer >= 160)
                {
                    open = false;
                    openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.2f);
                    rotation = MathHelper.Lerp(rotation, 0, 0.2f);
                    openRotation = MathHelper.Lerp(openRotation, 0, 0.2f);
                }
                if (AITimer >= 170)
                {
                    open = false;
                    openOffset = Vector2.Zero;
                    rotation = 0;
                    openRotation = 0;
                }
                if (AITimer >= 200)
                {
                    AITimer = 0;
                    AIState = Idle;
                }
            }
            else if (AIState == Idle)
            {
                AITimer++;
                halfEyesPhase2 = eyeCount <= 3;
                oldHP = NPC.life;
                if (claw[0].verlet != null && phase2)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                }
                AITimer2 = 1;

                if (AITimer < 70 && NPC.Distance(player.Center) > 200)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 100) + Helper.FromAToB(player.Center, NPC.Center) * 100) * 15, 0.15f);
                else NPC.velocity *= 0.97f;

                if (AITimer >= NPC.life / 260 + 10)
                {
                    AIState = Next;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == EyeBehaviour)
            {
                lastAi = (int)AIState;
                AITimer++;
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 350), false) / 20f;
                if (AITimer >= 220 - (halfEyesPhase2 ? 121 : 0) || phase2)
                {
                    ResetState();
                }
            }
            else if (AIState == Chomp)
            {
                NPC.dontTakeDamage = true;
                lastAi = (int)AIState;
                AITimer++;
                if (claw[0].verlet != null && phase2)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                }
                if (AITimer == 1) openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                if (open)
                {
                    if (AITimer2 % 2 != (phase2 ? 1 : 0))
                        openOffset += Vector2.UnitY * 5;
                    else
                        openOffset += Vector2.UnitX * 6;
                }
                if (AITimer < 25)
                {
                    open = true;
                    if (AITimer2 % 2 != (phase2 ? 1 : 0))
                    {
                        openRotation = MathHelper.Lerp(openRotation, MathHelper.ToRadians(90), 0.5f);
                        rotation = MathHelper.Lerp(rotation, MathHelper.ToRadians(90), 0.5f);
                    }
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 10f;
                }
                if (AITimer >= 25 && AITimer < (50 + (phase2 ? 7 : 0)))
                {
                    shakeVal = Lerp(shakeVal, (phase2 ? 30 : 15), 0.1f);
                    if (AITimer < 53)
                        savedPos = player.Center + (phase2 ? player.velocity * 5 : Vector2.Zero);
                    if (AITimer2 % 2 != (phase2 ? 1 : 0))
                        NPC.velocity = Helper.FromAToB(NPC.Center, savedPos, false) / 5f;
                    else
                        NPC.velocity = Helper.FromAToB(NPC.Center, savedPos, false) / 5f;
                }
                if (AITimer == 50 + (phase2 ? 7 : 0))
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - openOffset, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + openOffset, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                    //SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center + openOffset);
                    NPC.velocity = Vector2.Zero;
                }
                if (AITimer == 65)
                {
                    shakeVal = 0;
                    open = false;
                }
                if (AITimer > 65)
                {
                    openOffset.Y = MathHelper.Lerp(openOffset.Y, 0, 0.3f);
                    //NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + openOffset, 0.6f);
                }
                if (AITimer2 % 2 != (phase2 ? 1 : 0))
                {
                    if (openOffset.Y < 50 && AITimer > 25)
                    {
                        if (openOffset != Vector2.Zero)
                        {
                            SoundEngine.PlaySound(EbonianSounds.cecitiorClose, NPC.Center);
                            EbonianSystem.ScreenShakeAmount = 5;
                        }
                        openRotation = 0;
                        //rotation = 0;
                        open = false;
                        NPC.frame.Y = 0;
                        openOffset = Vector2.Zero;
                    }
                }
                if (AITimer >= 75)
                {
                    openRotation = 0;
                    rotation = 0;
                    if (phase2)
                        NPC.dontTakeDamage = false;
                    int num = 1;
                    if (halfEyesPhase2)
                        num = 2;
                    if (phase2)
                        num = 0;
                    if (AITimer2 < num)
                    {
                        AITimer2++;
                        AITimer = 0;
                        NPC.velocity = Vector2.Zero;
                    }
                    else
                    {
                        ResetState();
                    }
                }
            }
            else if (AIState == Teeth)
            {
                NPC.dontTakeDamage = true;
                lastAi = (int)AIState;
                if (claw[0].verlet != null && phase2)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                }
                AITimer++;
                if (AITimer == 1)
                    openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                if (AITimer < 20)
                {
                    open = true;
                    shakeVal = Lerp(shakeVal, 10, 0.1f);
                    openOffset += Vector2.UnitX * 13;
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 8f;
                }
                if (AITimer == 40)
                {
                    NPC.velocity = Vector2.Zero;

                    SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                    SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                    for (int i = 0; i < 6; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 12);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + openOffset, new Vector2(5 + (i * 0.1f)).RotatedBy(angle), ProjectileType<CecitiorTeeth>(), 30, 0);
                    }
                    for (int i = 8; i < 12; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 12);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - openOffset, new Vector2(5 + (i * 0.1f)).RotatedBy(angle), ProjectileType<CecitiorTeeth>(), 30, 0);
                    }
                }
                if (AITimer == 50 && halfEyesPhase2)
                {
                    NPC.velocity = Vector2.Zero;
                    SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                    for (int i = 0; i < 6; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 12);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + openOffset, new Vector2(5 + (i * 0.1f)).RotatedBy(angle - MathHelper.PiOver4 / 2), ProjectileType<CecitiorTeeth>(), 30, 0);
                    }
                    for (int i = 8; i < 12; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 12);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - openOffset, new Vector2(5 + (i * 0.1f)).RotatedBy(angle + MathHelper.PiOver4 / 2), ProjectileType<CecitiorTeeth>(), 30, 0);
                    }
                }
                if (AITimer == 60 && phase2)
                {
                    SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                    for (int i = 0; i < 6; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 12);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + openOffset, new Vector2(5 + (i * 0.1f)).RotatedBy(angle - MathHelper.PiOver4), ProjectileType<CecitiorTeeth>(), 30, 0);
                    }
                    for (int i = 8; i < 12; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 12);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - openOffset, new Vector2(5 + (i * 0.1f)).RotatedBy(angle + MathHelper.PiOver4), ProjectileType<CecitiorTeeth>(), 30, 0);
                    }
                }
                if (AITimer == 60)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - openOffset, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + openOffset, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                    //SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center + openOffset);
                    NPC.velocity = Vector2.Zero;
                }
                if (AITimer < 75 && AITimer > 40)
                    shakeVal = Lerp(shakeVal, (phase2 ? 30 : 15), 0.0f);
                if (AITimer == 75)
                {
                    open = false;
                }
                if (AITimer >= 85)
                {
                    if (phase2)
                        NPC.dontTakeDamage = false;
                    ResetState();
                }
            }
            else if (AIState == Tongue)
            {
                lastAi = (int)AIState;
                if (claw[0].verlet != null && phase2)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                }
                AITimer++;
                if (phase2)
                    AITimer++;
                open = true;
                if (AITimer < 15)
                {
                    AITimer2 = 10;
                    openOffset.X++;
                    openRotation -= MathHelper.ToRadians(2);
                    rotation += MathHelper.ToRadians(2);
                }
                if (AITimer == 40)
                {
                    SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                    tongue = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Clamp(Helper.FromAToB(NPC.Center, player.Center), new Vector2(-0.35f, 1), new Vector2(0.35f, 1)) * 1.5f, ProjectileType<LatcherPCecitior>(), 15, 0, -1, NPC.whoAmI);
                }
                if (AITimer < 20)
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 10f;
                if (AITimer == 20)
                {
                    NPC.velocity = Vector2.Zero;
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, 0), ProjectileType<BloodShockwave2>(), 0, 0, -1, NPC.whoAmI);
                }
                if (tongue != null)
                {
                    if (tongue.ai[1] == 1 && tongue.active)
                    {
                        NPC.damage = 15;
                        AITimer -= 0.5f;
                    }
                    else if (tongue.ai[1] == 0)
                        AITimer++;
                    else if (!tongue.active)
                    {
                        NPC.velocity = NPC.velocity.RotatedBy(ToRadians(3 * (NPC.velocity.X > 0 ? -1 : 1))) * 0.97f;
                        rotation = 0;
                        openRotation = 0;

                        NPC.damage = 0;
                        openOffset = Vector2.Zero;
                        open = false;
                        NPC.velocity *= 0.985f;
                        AITimer += 2;
                    }
                }
                if (AITimer >= 240)
                {
                    openOffset = Vector2.Zero;
                    open = false;
                }
                if (AITimer >= 260)
                {
                    tongue = null;
                    ResetState();
                }
            }
            else if (AIState == EyeBehaviour2)
            {
                lastAi = (int)AIState;
                AITimer++;
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 10f;
                if (AITimer >= 200 - (halfEyesPhase2 ? 130 : 0) || phase2)
                {
                    ResetState();
                }
            }
            else if (AIState == LaserRain)
            {
                lastAi = (int)AIState;
                AITimer++;
                open = true;
                if (AITimer < 15)
                {
                    if (halfEyesPhase2)
                        AITimer2 = 5;
                    else
                        AITimer2 = 10;
                    openOffset.X++;
                    openRotation -= MathHelper.ToRadians(2);
                    rotation += MathHelper.ToRadians(2);
                }
                if (AITimer < 35)
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 2;
                if (AITimer > 35)
                    NPC.velocity = Vector2.Zero;
                if (AITimer > 50 && AITimer < 65)
                {
                    AITimer2 -= 0.55f;
                    NPC.velocity = Vector2.Zero;
                    if (AITimer % 3 == 0)
                        SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                    for (int i = -1; i < 2; i++)
                    {
                        if (i == 0)
                            continue;
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * AITimer2, 5), ProjectileType<CecitiorTeeth>(), 30, 0);
                    }
                }
                if (AITimer >= 65)
                {
                    openOffset = Vector2.Zero;
                    open = false;
                }

                if (AITimer >= 70 || phase2)
                {
                    ResetState();
                }
            }
            else if (AIState == ThrowUpBlood)
            {
                if (lastAi == AIState)
                    ResetState();
                lastAi = (int)AIState;
                if (claw[0].verlet != null && phase2)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                }
                AITimer++;
                open = true;
                if (AITimer < 15)
                {
                    if (halfEyesPhase2)
                        AITimer2 = 20;
                    else
                        AITimer2 = 10;
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 20f;
                    openOffset.X++;
                    openRotation += MathHelper.ToRadians(2);
                    rotation -= MathHelper.ToRadians(2);
                }
                if (AITimer >= 30 && AITimer <= 60 && AITimer % (phase2 ? 3 : halfEyesPhase2 ? 5 : 10) == 0)
                {
                    AITimer2 -= 4;
                    NPC.velocity = Vector2.Zero;
                    if (halfEyesPhase2)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(AITimer2 * 0.5f, -6), ProjectileType<CIchor>(), 30, 0);
                    Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(AITimer2 * 5, -5), ProjectileType<CHeart>(), 30, 0);
                    a.friendly = false;
                    a.hostile = true;

                }
                if (AITimer >= 60)
                {
                    openOffset.X--;
                    openRotation -= MathHelper.ToRadians(2);
                    rotation += MathHelper.ToRadians(2);
                }
                if (AITimer >= 70)
                {
                    openOffset = Vector2.Zero;
                    open = false;
                }

                if (AITimer >= 75)
                {
                    ResetState();
                }
            }
            else if (AIState == Phase2ThrowUpEyes)
            {
                AITimer++;
                if (claw[0].verlet != null && phase2)
                {
                    claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                }
                NPC.velocity *= 0.9f;
                open = true;
                if (AITimer < 15)
                {
                    AITimer2 = 10;
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 20f;
                    openOffset.X++;
                    openRotation += MathHelper.ToRadians(2);
                    rotation -= MathHelper.ToRadians(2);
                }
                if (AITimer == 20)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2CircularEdge(30, 30), Main.rand.NextVector2Unit(), ProjectileType<EyeVFX>(), 0, 0);
                if (AITimer >= 30 && AITimer <= 80 && AITimer % 5 == 0)
                {
                    if (AITimer % 15 == 0)
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2CircularEdge(30, 30), Main.rand.NextVector2Unit() * 2, ProjectileType<CecitiorEyeP>(), 30, 0);
                }
                if (AITimer >= 80)
                {
                    openOffset.X--;
                    openRotation -= MathHelper.ToRadians(2);
                    rotation += MathHelper.ToRadians(2);
                }
                if (AITimer >= 90)
                {
                    openOffset = Vector2.Zero;
                    open = false;
                }
                if (AITimer >= 95 || !phase2)
                {
                    ResetState();
                }
            }
            else if (AIState == Phase2Claw)
            {
                AITimer++;
                if (claw[0].verlet != null)
                {
                    if (AITimer == 1)
                        AITimer2 = Main.rand.NextFloat(MathHelper.Pi * 2);
                    if (AITimer < 35)
                    {
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 2;
                        savedPos = player.Center;
                        claw[0].position = Vector2.Lerp(claw[0].position, savedPos + new Vector2(45, -170).RotatedBy(AITimer2), 0.3f);
                        claw[1].position = Vector2.Lerp(claw[1].position, savedPos + new Vector2(0, -200).RotatedBy(AITimer2), 0.3f);
                        claw[2].position = Vector2.Lerp(claw[2].position, savedPos + new Vector2(-45, -170).RotatedBy(AITimer2), 0.3f);
                    }
                    if (AITimer == 45)
                    {
                        for (int i = 0; i < claw.Length; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), claw[i].position + Vector2.UnitY.RotatedBy(AITimer2) * 110, -Vector2.UnitY.RotatedBy(AITimer2), ProjectileType<CecitiorClawSlash>(), 30, 0, ai2: 1);
                        }
                    }
                    if (AITimer >= 45)
                    {
                        NPC.velocity *= 0.9f;
                        for (int i = 0; i < claw.Length; i++)
                        {
                            claw[0].position = Vector2.Lerp(claw[0].position, savedPos - new Vector2(45, -170).RotatedBy(AITimer2), 0.04f);
                            claw[1].position = Vector2.Lerp(claw[1].position, savedPos - new Vector2(0, -200).RotatedBy(AITimer2), 0.04f);
                            claw[2].position = Vector2.Lerp(claw[2].position, savedPos - new Vector2(-45, -170).RotatedBy(AITimer2), 0.04f);
                        }
                    }
                }

                if (AITimer >= 55 || !phase2)
                {
                    ResetState();
                }
            }
            else if (AIState == Phase2ClawGrab)
            {
                AITimer++;
                if (AITimer < 40)
                {
                    savedPos = claw[(int)AITimer3].position + Vector2.Clamp(Helper.FromAToB(claw[(int)AITimer3].position, player.Center, false), -Vector2.One * 320, Vector2.One * 320);
                    if (AITimer3 == 0)
                    {
                        claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 20f;
                    }
                }
                if (AITimer == 40)
                {
                    if (AITimer3 == 0)
                    {
                        savedClawPos = claw[(int)AITimer3].position;
                        SoundEngine.PlaySound(EbonianSounds.cecitiorSlice, NPC.Center);
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), claw[(int)AITimer3].position, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                }
                if (AITimer > 40)
                {
                    NPC.velocity *= 0.9f;
                    if (AITimer < 80)
                    {
                        claw[(int)AITimer3].position = Vector2.Lerp(claw[(int)AITimer3].position, savedPos + Helper.FromAToB(savedClawPos, savedPos) * (100 + AITimer3 * 40), 0.15f + (AITimer3 * 0.025f));
                        if (AITimer3 == 1)
                        {
                            claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.015f);
                        }
                        if (AITimer3 == 2)
                        {
                            claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.015f);
                            claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.015f);
                        }
                    }
                    if (AITimer > 80 && AITimer2 == 0)
                    {
                        if (oldHP < NPC.lifeMax / 2)
                        {
                            if (AITimer3 < 2)
                            {
                                AITimer3++;
                                AITimer2 = 0;
                                AITimer = 60;
                                savedClawPos = claw[(int)AITimer3].position;
                                SoundEngine.PlaySound(EbonianSounds.cecitiorSlice, NPC.Center);
                                savedPos = claw[(int)AITimer3].position + Vector2.Clamp(Helper.FromAToB(claw[(int)AITimer3].position, player.Center, false), -Vector2.One * 320, Vector2.One * 320);
                            }
                            else
                                ResetState();
                        }
                        else if (oldHP < NPC.lifeMax - NPC.lifeMax / 4)
                        {
                            if (AITimer3 == 0)
                            {
                                AITimer3++;
                                AITimer2 = 0;
                                AITimer = 50;
                                savedClawPos = claw[(int)AITimer3].position;
                                SoundEngine.PlaySound(EbonianSounds.cecitiorSlice, NPC.Center);
                                savedPos = claw[(int)AITimer3].position + Vector2.Clamp(Helper.FromAToB(claw[(int)AITimer3].position, player.Center, false), -Vector2.One * 320, Vector2.One * 320);
                            }
                            else
                                ResetState();
                        }
                        else
                        {
                            ResetState();
                        }
                    }
                    if (claw[(int)AITimer3].position.Distance(player.Center) < 27 && AITimer2 == 0)
                    {
                        AITimer2 = 1;
                        AITimer = 70;
                    }
                }
                if (AITimer2 == 1)
                {
                    if (AITimer < 159)
                    {
                        player.Center = claw[(int)AITimer3].position;
                        claw[(int)AITimer3].position = Vector2.Lerp(claw[(int)AITimer3].position, NPC.Center + new Vector2(30, 100 - AITimer * 0.6f), 0.1f);
                    }
                    else
                    {
                        AITimer += 2;
                        claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    }
                    if (open)
                    {
                        openOffset += Vector2.UnitX * 6;
                    }
                    if (AITimer >= 110 && AITimer < 155)
                    {
                        open = true;
                    }
                    if (AITimer >= 155)
                    {
                        open = false;
                    }
                    if (AITimer >= 190)
                    {
                        ResetState();
                    }
                }
                if (!phase2)
                {
                    ResetState();
                }
            }
            else if (AIState == Phase2ClawMultiple)
            {
                AITimer++;
                if (AITimer % 50 == 1)
                    AITimer3 = Main.rand.Next(3);

                if (AITimer % 50 == 2)
                    for (int i = 0; i < 3; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), claw[(int)AITimer3].position, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                if (claw[0].verlet != null && AITimer < 201 - (oldHP < NPC.lifeMax - NPC.lifeMax / 4 ? (oldHP < NPC.lifeMax / 2 ? 0 : 50) : 100))
                {
                    if (AITimer3 != 0)
                        claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.1f);
                    if (AITimer3 != 1)
                        claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.1f);
                    if (AITimer3 != 2)
                        claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.1f);
                    if (AITimer % 50 == 1)
                    {
                        AITimer2 = Main.rand.NextFloat(MathHelper.Pi * 2);
                    }
                    if (AITimer % 50 < 40)
                    {
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 2;
                        savedPos = player.Center;
                        if (AITimer3 == 0)
                            claw[0].position = Vector2.Lerp(claw[0].position, savedPos + new Vector2(0, -200).RotatedBy(AITimer2), 0.3f);
                        if (AITimer3 == 1)
                            claw[1].position = Vector2.Lerp(claw[1].position, savedPos + new Vector2(0, -200).RotatedBy(AITimer2), 0.3f);
                        if (AITimer3 == 2)
                            claw[2].position = Vector2.Lerp(claw[2].position, savedPos + new Vector2(0, -200).RotatedBy(AITimer2), 0.3f);
                    }
                    if (AITimer % 50 == 40)
                    {
                        for (int i = 0; i < claw.Length; i++)
                        {
                            if (i == AITimer3)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), claw[i].position + Vector2.UnitY.RotatedBy(AITimer2) * 110, -Vector2.UnitY.RotatedBy(AITimer2), ProjectileType<CecitiorClawSlash>(), 30, 0, ai2: 1);
                        }
                    }
                    if (AITimer % 50 >= 40)
                    {
                        NPC.velocity *= 0.9f;
                        for (int i = 0; i < claw.Length; i++)
                        {
                            if (AITimer3 == 0)
                                claw[0].position = Vector2.Lerp(claw[0].position, savedPos - new Vector2(0, -200).RotatedBy(AITimer2), 0.04f);
                            if (AITimer3 == 1)
                                claw[1].position = Vector2.Lerp(claw[1].position, savedPos - new Vector2(0, -200).RotatedBy(AITimer2), 0.04f);
                            if (AITimer3 == 2)
                                claw[2].position = Vector2.Lerp(claw[2].position, savedPos - new Vector2(0, -200).RotatedBy(AITimer2), 0.04f);
                        }
                    }
                }

                if (AITimer >= 201 - (oldHP < NPC.lifeMax - NPC.lifeMax / 4 ? (oldHP < NPC.lifeMax / 2 ? 0 : 50) : 100) || !phase2)
                {
                    ResetState();
                }
            }
            else if (AIState == Phase2GrabBomb)
            {
                if (!phase2)
                    ResetState();
                else
                {
                    AITimer++;
                    if (AITimer <= 165 - (oldHP < NPC.lifeMax - NPC.lifeMax / 4 ? (oldHP < NPC.lifeMax / 2 ? 0 : 50) : 100))
                        open = true;

                    if (AITimer < 15)
                    {
                        AITimer2 = 35;
                        openOffset.X++;
                        openRotation += MathHelper.ToRadians(2);
                        rotation -= MathHelper.ToRadians(2);
                    }
                    if (AITimer < 65)
                    {
                        if (AITimer < 20)
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 250), false) / 20f;
                        else
                            NPC.velocity *= 0.9f;
                        //claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                    }
                    if (AITimer >= 15 && AITimer < 25)
                        claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + new Vector2(0, 10), 0.2f);
                    if (AITimer >= 20 && AITimer <= 30)
                    {
                        //claw[0].orbAlpha += .1f;
                        savedPos = player.Center;
                    }
                    if (AITimer >= 30 && AITimer <= 65)
                    {
                        AITimer2--;
                        float swingProgress = Ease(Utils.GetLerpValue(0f, 35, AITimer2));
                        float defRot = Helper.FromAToB(NPC.Center, savedPos).ToRotation();
                        float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                        float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                        float rotation = start + MathHelper.Pi * 3 / 2 * swingProgress;
                        Vector2 position = NPC.Center +
                            rotation.ToRotationVector2() * 210 * ScaleFunction(swingProgress);
                        claw[0].position = Vector2.Lerp(claw[0].position, position, 0.2f);
                        if (AITimer % 6 == 1)
                        {
                            SoundEngine.PlaySound(SoundID.Item1, claw[0].position);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), claw[0].position, rotation.ToRotationVector2() * 6, ProjectileType<CecitiorBombThing>(), 30, 0);
                        }
                        //if (AITimer == 64)
                        //claw[0].orbAlpha = 0f;
                    }

                    if (oldHP < NPC.lifeMax - NPC.lifeMax / 4)
                    {

                        if (AITimer >= 65 && AITimer < 115)
                        {
                            if (AITimer < 75)
                                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 250), false) / 20f;
                            else
                                NPC.velocity *= 0.9f;
                            claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                            //claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                            claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                        }
                        if (AITimer >= 50 && AITimer < 70)
                            claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + new Vector2(0, 10), 0.2f);
                        if (AITimer >= 60 && AITimer <= 70)
                        {
                            //claw[1].orbAlpha += .1f;
                        }
                        if (AITimer == 66)
                        {
                            AITimer2 = 35;
                            savedPos = player.Center;
                        }

                        if (AITimer >= 80 && AITimer <= 115)
                        {
                            AITimer2--;
                            float swingProgress = Ease(Utils.GetLerpValue(0f, 35, AITimer2));
                            float defRot = Helper.FromAToB(NPC.Center, savedPos, reverse: true).ToRotation();
                            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                            float rotation = start + MathHelper.Pi * 3 / 2 * swingProgress;
                            Vector2 position = NPC.Center +
                                rotation.ToRotationVector2() * 160 * ScaleFunction(swingProgress);
                            claw[1].position = Vector2.Lerp(claw[1].position, position, 0.2f);
                            if (AITimer % 6 == 1)
                            {
                                SoundEngine.PlaySound(SoundID.Item1, claw[1].position);
                                //claw[1].orbAlpha = MathHelper.Lerp(claw[1].orbAlpha, 0, 0.24f);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), claw[1].position, rotation.ToRotationVector2() * 6, ProjectileType<CecitiorBombThing>(), 30, 0);
                            }
                            //if (AITimer == 114)
                            //claw[1].orbAlpha = 0f;
                        }

                        if (oldHP < NPC.lifeMax / 2)
                        {
                            if (AITimer >= 115 && AITimer < 165)
                            {
                                if (AITimer < 125)
                                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 250), false) / 20f;
                                else
                                    NPC.velocity *= 0.9f;
                                claw[0].position = Vector2.Lerp(claw[0].position, NPC.Center + openOffset + new Vector2(150, -65).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                                claw[1].position = Vector2.Lerp(claw[1].position, NPC.Center + openOffset + new Vector2(165, 45).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                                //claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center - openOffset + new Vector2(-110, 55).RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f), 0.2f);
                            }
                            if (AITimer >= 105 && AITimer < 125)
                                claw[2].position = Vector2.Lerp(claw[2].position, NPC.Center + new Vector2(0, 10), 0.2f);
                            if (AITimer >= 115 && AITimer <= 125)
                            {
                                //claw[2].orbAlpha += .1f;
                            }
                            if (AITimer == 120)
                            {
                                savedPos = player.Center;
                                AITimer2 = 35;
                            }

                            if (AITimer >= 125 && AITimer <= 165)
                            {
                                AITimer2--;
                                float swingProgress = Ease(Utils.GetLerpValue(0f, 35, AITimer2));
                                float defRot = Helper.FromAToB(NPC.Center, savedPos).ToRotation();
                                float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                                float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                                float rotation = start + MathHelper.Pi * 3 / 2 * swingProgress;
                                Vector2 position = NPC.Center +
                                    rotation.ToRotationVector2() * 170 * ScaleFunction(swingProgress);
                                claw[2].position = Vector2.Lerp(claw[2].position, position, 0.2f);
                                if (AITimer % 6 == 1)
                                {
                                    SoundEngine.PlaySound(SoundID.Item1, claw[2].position);
                                    //claw[2].orbAlpha = MathHelper.Lerp(claw[2].orbAlpha, 0, 0.24f);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), claw[2].position, rotation.ToRotationVector2().RotatedByRandom(1) * 6, ProjectileType<CecitiorBombThing>(), 30, 0);
                                }
                                //if (AITimer == 164)
                                //claw[2].orbAlpha = 0f;
                            }
                        }
                    }

                    if (AITimer >= 165 - (oldHP < NPC.lifeMax - NPC.lifeMax / 4 ? (oldHP < NPC.lifeMax / 2 ? 0 : 50) : 100))
                    {
                        openOffset = Vector2.Zero;
                        openRotation = MathHelper.Lerp(openRotation, 0f, 0.1f);
                        rotation = MathHelper.Lerp(rotation, 0f, 0.1f);
                        open = false;
                    }
                    if (AITimer >= 180 - (oldHP < NPC.lifeMax - NPC.lifeMax / 4 ? (oldHP < NPC.lifeMax / 2 ? 0 : 50) : 100) || !phase2)
                    {
                        ResetState();
                    }
                }
            }
            else if (AIState == Phase2ClawBodySlam)
            {
                if (!phase2)
                    ResetState();
                else
                {
                    AITimer++;
                    if (AITimer < 20)
                        NPC.velocity = Helper.FromAToB(NPC.Center, Helper.TRay.Cast(player.Center, Vector2.UnitY, 1200, true) - new Vector2(0, 200), false) / 10f;
                    if (AITimer > 20 && AITimer < 45)
                    {
                        NPC.velocity *= 0.8f;
                        claw[0].position = Vector2.Lerp(claw[0].position, Helper.TRay.Cast(NPC.Center + new Vector2(70, 0), new Vector2(0.2f, 1), 400, true) + new Vector2(0, 25), 0.1f);
                        claw[1].position = Vector2.Lerp(claw[1].position, Helper.TRay.Cast(NPC.Center, new Vector2(-0.05f, 1), 400, true) + new Vector2(0, 25), 0.1f);
                        claw[2].position = Vector2.Lerp(claw[2].position, Helper.TRay.Cast(NPC.Center + new Vector2(-75, 0), new Vector2(-0.25f, 1), 400, true) + new Vector2(0, 25), 0.1f);
                    }
                    if (AITimer <= 60 && AITimer >= 50)
                    {
                        NPC.velocity.Y += 3;
                        NPC.damage = 100;
                    }
                    if (Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, NPC.height * 2, true) < NPC.height && AITimer2 == 0 && AITimer > 45)
                    {
                        if (AITimer < 60)
                            AITimer = 61;
                        AITimer2 = 1;
                        EbonianSystem.ScreenShakeAmount = 5;
                        NPC.velocity = Vector2.UnitY * -17.5f;

                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                        for (int i = 0; i < 6; i++)
                        {
                            Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 100), Main.rand.NextVector2Circular(7, 7), ProjectileType<Gibs>(), 40, 0, 0, 0);
                            p.friendly = false;
                            p.hostile = true;
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 100), Main.rand.NextVector2Circular(4, 4), ProjectileType<CecitiorEyeP>(), 40, 0, 0, 0);
                        }


                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<FatSmash>(), 0, 0, 0, 0);
                        SoundEngine.PlaySound(EbonianSounds.cecitiorSlam, NPC.Center);
                    }
                    if (AITimer > 65 && AITimer2 == 1)
                    {
                        NPC.velocity *= 0.9f;
                        NPC.damage = 0;
                    }
                    if (AITimer >= 100 && oldHP > NPC.lifeMax - NPC.lifeMax / 4)
                    {
                        NPC.damage = 0;
                        ResetState();
                    }

                    if (AITimer >= 120 && oldHP <= NPC.lifeMax - NPC.lifeMax / 4)
                    {
                        AITimer = 70;
                        AITimer2 = 2;
                        NPC.damage = 0;
                        AITimer3++;
                        if (AITimer3 > (oldHP < NPC.lifeMax / 2 ? 1 : 0))
                        {
                            ResetState();
                        }
                    }
                    if (oldHP <= NPC.lifeMax - NPC.lifeMax / 4)
                    {
                        if (AITimer <= 80 && AITimer >= 71)
                        {
                            if (AITimer == 71)
                                AITimer2 = 2;
                            NPC.damage = 100;
                            NPC.velocity.Y += 3;
                        }
                        if (Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, NPC.height * 2, true) < NPC.height && AITimer2 == 2)
                        {
                            if (AITimer < 80)
                                AITimer = 81;
                            AITimer2 = 3;
                            EbonianSystem.ScreenShakeAmount = 5;
                            NPC.velocity = Vector2.UnitY * Main.rand.NextFloat(-17.5f, -10);

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<FatSmash>(), 0, 0, 0, 0);
                            if (AITimer3 == 0)
                            {
                                for (int i = 0; i < 2; i++)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                                for (int i = 0; i < 10; i++)
                                {
                                    Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 100), Main.rand.NextVector2Circular(14, 14), ProjectileType<Gibs>(), 40, 0, 0, 0);
                                    p.friendly = false;
                                    p.hostile = true;
                                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 100), new Vector2(Main.rand.NextFloat(-4f, 4), Main.rand.NextFloat(-7, -3)), ProjectileType<CIchor>(), 40, 0, 0, 0);
                                }
                            }

                            if (AITimer3 == 1)
                            {
                                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                                for (int i = 0; i < 2; i++)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                                for (int i = 0; i < 16; i++)
                                {
                                    Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 100), Main.rand.NextVector2Circular(14, 14), ProjectileType<Gibs>(), 40, 0, 0, 0);
                                    p.friendly = false;
                                    p.hostile = true;
                                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 100), Main.rand.NextVector2Circular(4, 4), ProjectileType<CecitiorEyeP>(), 40, 0, 0, 0);
                                }
                            }
                            SoundEngine.PlaySound(EbonianSounds.cecitiorSlam, NPC.Center);
                        }
                        if (AITimer > 85 && AITimer2 == 3)
                        {
                            NPC.damage = 0;
                            NPC.velocity *= 0.8f;
                            NPC.velocity.Y--;
                        }
                    }
                }
            }
        }
        public float Ease(float x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2;
        }
        public float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
    }
}
