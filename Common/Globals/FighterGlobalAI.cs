using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace EbonianMod.Common.Globals
{
    // rest in peace Mythos of Moonlight but whoever wrote this is mentally ill, gotta rewrite at some point
    public class FighterGlobalAI : GlobalNPC
    {
        public bool GoldMarked = false;
        public override void PostAI(NPC npc)
        {
            if (GoldMarked)
            {
                int num = 244;
                if (npc.value >= 1000000)
                {
                    num = 247;
                }
                else if (npc.value >= 10000)
                {
                    num = 246;
                }
                else if (npc.value >= 100)
                {
                    num = 245;
                }
                if (Main.rand.NextBool())
                {
                    int num2 = Dust.NewDust(npc.position, npc.width, npc.height, num, 0f, 0f, 254, default, 0.25f);
                    Main.dust[num2].velocity *= 0.2f;
                }
            }
        }
        public override bool InstancePerEntity => true;
        const float StrideLimit = 70.78f;
        public bool Jump = false;
        public void FighterAI(NPC NPC, float jumpHeight, float strideSpeed, bool canJump, int jumpFrame = 1, int jumpOffset = 4, float turningVel = .06f)
        {
            NPC.TargetClosest(false);
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
            var player = Main.player[NPC.target];
            if (NPC.collideX && NPC.frameCounter > 2 && NPC.ai[3] <= 0)
            {
                NPC.velocity.Y = -jumpHeight;
                NPC.ai[3] = 1;
            }
            if (Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].HasTile)
            {
                if (Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].LeftSlope || Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].BottomSlope || Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].RightSlope)
                {
                    NPC.velocity.Y = -jumpHeight;
                    NPC.ai[3] = 1;
                }
                else NPC.ai[3] = 0;
            }

            FitVelocityXToTarget(NPC, strideSpeed * NPC.direction);
            var horizontalDistance = Math.Abs(NPC.Center.X - player.Center.X);
            if (horizontalDistance >= 35.78f)
            {
                NPC.FaceTarget();
            }
            NPC.spriteDirection = NPC.direction;
        }
        public void AimlessWander(NPC NPC, float jumpHeight, float strideSpeed)
        {
            NPC.TargetClosest(false);
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
            if (NPC.collideX && NPC.frameCounter > 2 && NPC.ai[3] <= 0)
            {
                NPC.velocity.Y = -jumpHeight;
                NPC.ai[3] = 1;
            }
            if (Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].HasTile)
            {
                if (Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].LeftSlope || Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].BottomSlope || Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16].RightSlope)
                {
                    NPC.velocity.Y = -jumpHeight;
                    NPC.ai[3] = 1;
                }
                else NPC.ai[3] = 0;
            }

            FitVelocityXToTarget(NPC, strideSpeed * NPC.direction);
            var horizontalDistance = Math.Abs(NPC.Center.X - (NPC.Center.X + NPC.direction * 100));
            if (horizontalDistance >= 35.78f)
            {
                NPC.FaceTarget();
            }
            NPC.spriteDirection = NPC.direction;
        }
        void FitVelocityXToTarget(NPC NPC, float newX) => NPC.velocity.X = Lerp(NPC.velocity.X, newX, 0.1f);
        public void FighterAIOLD(NPC NPC, float jumpHeight, float strideSpeed, bool canJump, int jumpFrame = 1, int jumpOffset = 4, float turningVel = .06f)
        {
            NPC.TargetClosest(false);

            if (strideSpeed * NPC.direction >= 0)
            {
                if (NPC.velocity.X < strideSpeed * NPC.direction) NPC.velocity.X += turningVel;
            }
            else
            {
                if (NPC.velocity.X > strideSpeed * NPC.direction) NPC.velocity.X -= turningVel;
            }
            var player = Main.player[NPC.target];
            if (NPC.collideX && canJump && !Jump)
            {
                NPC.velocity.Y = -jumpHeight;
                Jump = true;
            }
            Tile bot = Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16];
            Tile exBot = Main.tile[NPC.Hitbox.Center.X / 16, NPC.Hitbox.Bottom / 16 + 1];
            if (bot.HasTile)
            {
                if (bot.LeftSlope || bot.BottomSlope || bot.RightSlope)
                {
                    NPC.velocity.Y = -jumpHeight * 0.035f;
                    Jump = true;
                }
                else Jump = false;
            }
            else
            {
                if (exBot.HasTile)
                {
                    if (exBot.LeftSlope || exBot.BottomSlope || exBot.RightSlope)
                    {
                        NPC.velocity.Y = -jumpHeight;
                        Jump = true;
                    }
                    else Jump = false;
                }
            }
            if (Jump)
            {
                if (jumpFrame >= 0) NPC.frame = new Rectangle(0, NPC.height * jumpFrame + jumpOffset, NPC.width, NPC.height + jumpOffset);
            }
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);

            var horizontalDistance = Math.Abs(NPC.Center.X - player.Center.X);
            if (horizontalDistance >= StrideLimit)
            {
                NPC.FaceTarget();
            }
            NPC.spriteDirection = NPC.direction;
        }
    }
}