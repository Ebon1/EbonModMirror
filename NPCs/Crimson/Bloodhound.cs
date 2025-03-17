using Terraria;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.Items.Weapons.Magic;
namespace EbonianMod.NPCs.Crimson
{
    public class Bloodhound : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.Bloodhound.Bestiary"),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, 2, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ItemType<CrimCannon>(), 60));
        }

        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 46;
            NPC.damage = 15;
            NPC.defense = 4;
            NPC.lifeMax = 85;
            AIType = 155;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 26;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.ai[3] == 0)
                for (int i = 0; i < Main.rand.Next(5); i++)
                    NPC.NewNPCDirect(NPC.InheritSource(NPC), NPC.Center, NPC.type, ai3: 1);
            off = Main.rand.NextFloat(0.3f);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight)
            {
                return .035f;
            }
            else
            {
                return 0;
            }
        }
        public int timer = 0;
        public float off = 0;
        public override void AI()
        {
            NPC.ai[0] = 2;
            if (Helper.TRay.CastLength(NPC.position + NPC.velocity * off, NPC.velocity, 60) > 50)
                NPC.position.X += NPC.velocity.X * off;
            if (++timer >= 35)
            {
                NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                timer = 0;
            }
        }
        public override bool CheckDead()
        {
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
            }
            return base.CheckDead();
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else if (NPC.frameCounter < 15)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = 3 * frameHeight;
            }
            else if (NPC.frameCounter < 25)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = 5 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }

            if (!NPC.velocity.Y.CloseTo(0, 0.3f))
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = 5 * frameHeight;
            }
        }
    }
}