﻿using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Terrortoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson.Crimera
{
    public class CrimeraHead : WormHead
    {
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Ichor, 2, 1, 4));
        }

        //public override bool HasCustomBodySegments => true;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if ((hit.Damage >= NPC.life && NPC.life <= 0))
            {
                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/Gnasher1").Type, NPC.scale);
                }
            }
        }
        public override bool byHeight => false;
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Crimson/Crimera/Crimera",
                Position = new Vector2(0, 40f),
                PortraitPositionXOverride = 0f,
                Rotation = PiOver2,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson && Main.hardMode)
            {
                return .1f;
            }
            else
            {
                return 0;
            }
        }
        public override bool useNormalMovement => !(NPC.ai[2] > 300 && NPC.ai[2] < 650);
        float offset;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(offset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            offset = reader.ReadSingle();
        }
        public override void ExtraAI()
        {
            Player player = Main.player[NPC.target];
            if (player.Distance(NPC.Center) > 1800) return;
            NPC.ai[2]++;
            if (NPC.ai[2] == 300)
                offset = Main.rand.NextFloat(1.5f, 3);
            if (NPC.ai[2] > 300 && NPC.ai[2] < 650)
            {
                NPC.damage = 0;
                Vector2 pos = player.Center + new Vector2(300, 0).RotatedBy(MathHelper.ToRadians(NPC.ai[2] * offset));
                NPC.velocity = NPC.Center.FromAToB(pos, false) * 0.025f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                if (NPC.ai[2] % 4 == 0 && NPC.ai[2] > 340)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RotatedByRandom(MathHelper.PiOver4 * 1.5f) * Main.rand.NextFloat(.5f, 2f), ProjectileID.BloodShot, 10, 0);
                //NPC.rotation = MathHelper.Lerp(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2, 0.3f);
                //NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.Clamp(Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.PiOver2, FollowerNPC.rotation - MathHelper.ToRadians(30), FollowerNPC.rotation + MathHelper.ToRadians(30)), 0.5f);
            }
            if (NPC.ai[2] > 800)
            {
                NPC.damage = 30;
                NPC.ai[2] = (int)(-200 * offset);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.CrimeraHead.Bestiary"),
            });
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(66, 68);
            NPC.value = Item.buyPrice(0, 0, 20);
            NPC.damage = 30;
            NPC.aiStyle = -1;

        }
        /*public override int SpawnBodySegments(int segmentCount)
        {
            var source = NPC.GetSource_FromThis();
            NPC.ai[3]++;
            int latestNPC = SpawnSegment(source, BodyType, NPC.whoAmI, NPC.ai[3] == 3 ? 1 : 0);
            latestNPC = SpawnSegment(source, BodyType, latestNPC, NPC.ai[3] == 3 ? 1 : 0);
            return latestNPC;
        }*/
        public override int BodyType => NPCType<CrimeraBody>();

        public override int TailType => NPCType<CrimeraTail>();

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/Crimera/CrimeraHead");
            spriteBatch.Draw(tex, NPC.Center - (Vector2.UnitY * tex.Height / 3).RotatedBy(NPC.rotation) - screenPos, null, drawColor, NPC.rotation, new Vector2(tex.Width / 2, tex.Height / 4), NPC.scale, SpriteEffects.None, 0);


            if (Main.LocalPlayer.HasBuff(BuffID.Hunter) && !NPC.IsABestiaryIconDummy)
                spriteBatch.Draw(tex, NPC.Center - (Vector2.UnitY * tex.Height / 3).RotatedBy(NPC.rotation) - screenPos, null, NPC.HunterPotionColor(), NPC.rotation, new Vector2(tex.Width / 2, tex.Height / 4), NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void Init()
        {

            MinSegmentLength = 3;
            MaxSegmentLength = 3;
            MoveSpeed = 7.5f;
            Acceleration = 0.035f;
            CanFly = true;
        }
    }
    public class CrimeraBody : WormBody
    {

        public override bool byHeight => false;
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/Crimera/CrimeraBody");
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            if (Main.LocalPlayer.HasBuff(BuffID.Hunter) && !NPC.IsABestiaryIconDummy)
                spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.HunterPotionColor(), NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 16);
            NPC.aiStyle = -1;

        }
        public override void Init()
        {
            MoveSpeed = 7.5f;
            Acceleration = 0.035f;

        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if ((hit.Damage >= NPC.life && NPC.life <= 0))
            {
                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/Gnasher1").Type, NPC.scale);
                }
            }
        }
    }
    public class CrimeraTail : WormTail
    {
        public override bool byHeight => false;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if ((hit.Damage >= NPC.life && NPC.life <= 0))
            {
                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/Gnasher1").Type, NPC.scale);
                }
            }
        }
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {

            NPC.buffImmune[BuffID.Ichor] = true;
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(66 / 1.5f, 68 / 1.5f);
            NPC.aiStyle = -1;
            NPC.damage = 0;

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/Crimera/CrimeraTail");
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, new Vector2(tex.Width / 2, tex.Height + 2), NPC.scale, SpriteEffects.None, 0);

            if (Main.LocalPlayer.HasBuff(BuffID.Hunter) && !NPC.IsABestiaryIconDummy)
                spriteBatch.Draw(tex, NPC.Center - screenPos, null, NPC.HunterPotionColor(), NPC.rotation, new Vector2(tex.Width / 2, tex.Height + 2), NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        /*public override void ExtraAI()
        {
            if (++NPC.ai[2] % 30 == 0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY.RotatedBy(NPC.rotation) * 4, ProjectileType<TFlameThrower>(), 10, 0);
            }
        }*/
        public override void Init()
        {
            MoveSpeed = 7.5f;
            Acceleration = 0.035f;

        }
    }
}
