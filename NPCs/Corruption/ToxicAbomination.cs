﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ObjectInteractions;
using EbonianMod.Items.Misc;

namespace EbonianMod.NPCs.Corruption
{
    public class ToxicAbomination : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(10, 0)
            });

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.ToxicAbomination.Bestiary"),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.CursedFlame, 2, 1, 4));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt && Main.hardMode)
            {
                return .08f;
            }
            else
            {
                return 0;
            }
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.width = 84;
            NPC.height = 82;
            NPC.npcSlots = 0.1f;
            NPC.lifeMax = 450;
            NPC.damage = 37;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.buffImmune[24] = true;
            NPC.knockBackResist = 0.2f;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 20);
            NPC.defense = 12;

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/ToxicAbomination_Glow");
            Texture2D tex2 = Helper.GetTexture("NPCs/Corruption/ToxicAbomination");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter % 5 == 0)
            {
                if (AITimer < 400 || AITimer > 455)
                {
                    if ((NPC.frame.Y += frameHeight) > 7 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    if (NPC.frameCounter % 10 == 0)
                        NPC.frame.Y = 8 * frameHeight;
                    else
                        NPC.frame.Y = 9 * frameHeight;
                }
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.ai[3] == 0)
            {
                NPC.scale = Main.rand.NextFloat(0.9f, 1.05f);
                NPC.velocity = Main.rand.NextVector2Unit();
            }
        }
        float AITimer;
        public override void PostAI()
        {
            Lighting.AddLight(NPC.Center, TorchID.Cursed);
            NPC.TargetClosest(false);
            AITimer++;
            AIType = (AITimer < 400 ? 205 : -1);
            NPC.aiStyle = (AITimer < 400 ? 5 : -1);
            if (AITimer < 300)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active && npc.whoAmI != NPC.whoAmI)
                    {
                        if (npc.Center.Distance(NPC.Center) < npc.width * npc.scale)
                        {
                            NPC.velocity += NPC.Center.FromAToB(npc.Center, true, true) * 0.5f;
                        }
                        if (npc.Center == NPC.Center)
                        {
                            NPC.velocity = Main.rand.NextVector2Unit() * 5;
                        }
                    }
                }
                NPC.position += NPC.velocity * 0.2f;
            }
            else
            {
                if (Main.player[NPC.target].Distance(NPC.Center) > 1800) return;
                if (AITimer == 305)
                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);

                if (AITimer < 317 && AITimer > 310)
                    NPC.velocity += Helper.FromAToB(NPC.Center, Main.player[NPC.target].Center) * 2.4f;

                if (AITimer > 355)
                    NPC.velocity *= 0.9f;

                NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0);

                if (AITimer > 375)
                {
                    AITimer = Main.rand.Next(-200, 100);
                }
            }
        }
        public override bool? CanFallThroughPlatforms() => true;
        public override bool CheckDead()
        {
            for (int i = 0; i < 8; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity, Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity, Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
                for (int j = 0; j < 3; j++)
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
            }
            return true;
        }
    }
}
