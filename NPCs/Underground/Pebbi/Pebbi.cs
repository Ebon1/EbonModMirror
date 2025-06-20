using System;
using EbonianMod.Common.Globals;
using EbonianMod.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Underground.Pebbi
{
    public class Pebbi1 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1 };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 20;
            NPC.damage = 12;
            NPC.lifeMax = 30;
            NPC.defense = 2;
            NPC.knockBackResist = 1f;
            NPC.HitSound = SoundID.NPCHit7;
            SoundStyle sound = SoundID.NPCDeath43;
            sound.PitchRange = new(1.5f, 3f);
            NPC.DeathSound = sound;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }

        const float JUMP_HEIGHT = 6, STRIDE_SPEED = 2f;
        public override void AI()
        {
            NPC.GetGlobalNPC<FighterGlobalAI>().FighterAI(NPC, JUMP_HEIGHT, STRIDE_SPEED, true, 2, 0);
            if (NPC.collideX && NPC.collideY)
                NPC.velocity.Y -= 5f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.life = 30;
            NPC.lifeMax = 30;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.ai[0]++;
            if (!NPC.GetGlobalNPC<FighterGlobalAI>().Jump)
            {
                NPC.frame.Y = (int)(NPC.ai[0] / 6) % 4 * frameHeight;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2 * hit.HitDirection, -1.5f);
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2 * hit.HitDirection, -1.5f);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.StoneBlock, 1, 2, 4));
        }
    }
    public class Pebbi2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chunk Pebbi");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1 };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 26;
            NPC.damage = 12;
            NPC.lifeMax = 30;
            NPC.defense = 6;
            NPC.HitSound = SoundID.NPCHit7;
            SoundStyle sound = SoundID.NPCDeath43;
            sound.PitchRange = new(1.5f, 3f);
            NPC.DeathSound = sound;
            NPC.knockBackResist = .6f;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.life = 30;
            NPC.lifeMax = 30;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.ai[0]++;
            if (!NPC.GetGlobalNPC<FighterGlobalAI>().Jump)
            {
                NPC.frame.Y = (int)(NPC.ai[0] / 6) % 4 * frameHeight;
            }
        }
        public override void AI()
        {
            NPC.GetGlobalNPC<FighterGlobalAI>().FighterAI(NPC, 4, 1f, true, 1, 0);
            if (NPC.collideX && NPC.collideY)
                NPC.velocity.Y -= 5f;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2 * hit.HitDirection, -1.5f);
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2 * hit.HitDirection, -1.5f);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.StoneBlock, 1, 2, 4));
        }
    }
    public class Pebbi3 : ModNPC
    {
        public bool Jump = false;
        public float strideSpeed = 4f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Geode Pebbi");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1 };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 12;
            NPC.lifeMax = 30;
            NPC.defense = 2;
            NPC.knockBackResist = 2f;
            NPC.HitSound = SoundID.NPCHit7;
            SoundStyle sound = SoundID.NPCDeath43;
            sound.PitchRange = new(1.5f, 3f);
            NPC.DeathSound = sound;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.life = 30;
            NPC.lifeMax = 30;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.ai[0]++;
            NPC.frame.Y = (int)(NPC.ai[0] / 6) % 5 * frameHeight;
        }
        public override void AI()
        {
            NPC.TargetClosest(false);
            if (strideSpeed * NPC.direction >= 0)
            {
                if (NPC.velocity.X < strideSpeed * NPC.direction) NPC.velocity.X += .025f;
            }
            else
            {
                if (NPC.velocity.X > strideSpeed * NPC.direction) NPC.velocity.X -= .025f;
            }
            var player = Main.player[NPC.target];
            if (NPC.collideX && !Jump)
            {
                NPC.velocity.Y = -5f;
                Jump = true;
            }
            if (NPC.collideY && NPC.velocity.Y >= 0) Jump = false;
            var horizontalDistance = Math.Abs(NPC.Center.X - player.Center.X);
            if (horizontalDistance >= 66f)
            {
                NPC.FaceTarget();
            }
            NPC.spriteDirection = -NPC.direction;
            if (NPC.collideX && NPC.collideY)
                NPC.velocity.Y -= 4f;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2 * hit.HitDirection, -1.5f);
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2 * hit.HitDirection, -1.5f);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.StoneBlock, 1, 2, 4));
        }
    }
    public class PebbiSwarm : ModNPC
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true, });
            // DisplayName.SetDefault("PebbiSwarm");
        }
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 0;
            NPC.lifeMax = 1;
            NPC.defense = 0;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float chance = !(spawnInfo.Player.ZoneSnow || spawnInfo.Player.ZoneUndergroundDesert || spawnInfo.Player.ZoneDungeon || spawnInfo.Player.ZoneJungle || spawnInfo.PlayerSafe || spawnInfo.Player.ZoneUnderworldHeight) && (spawnInfo.Player.ZoneNormalCaverns) ? 0.07f : 0f;
            return chance;
        }
        public override void OnSpawn(IEntitySource source)
        {
            int num = Main.rand.Next(5, 8);
            for (int i = 1; i <= num; i++)
            {
                int type = Main.rand.Next(new int[3] { ModContent.NPCType<Pebbi1>(), ModContent.NPCType<Pebbi2>(), ModContent.NPCType<Pebbi3>() });
                NPC.NewNPC(source, (int)NPC.position.X + Main.rand.Next(NPC.width), (int)NPC.position.Y + Main.rand.Next(NPC.height), type);
            }
            NPC.life = 0;
            NPC.checkDead();
        }
    }
}