﻿using EbonianMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Tiles
{
    public class CrimsonBrainBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileMerge[TileType<CrimsonBrainBlockSpecial>()][Type] = true;
            DustType = DustID.Blood;
            //ItemDrop = ItemType<Items.Tiles.CrimsonBrainI>();

            AddMapEntry(Color.Maroon);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
    public class CrimsonBrainBlockSpecial : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileType<CrimsonBrainBlock>()][Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            DustType = DustID.Blood;
            RegisterItemDrop(ItemType<Items.Accessories.BrainAcc>());

            AddMapEntry(Color.Maroon);
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ItemType<BrainAcc>());
        }
        public Rectangle frame;
        public int frameCounter;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            for (int k = -1; k < 2; k++)
            {
                Vector2 neckOrigin = new Vector2(i * 16, j * 16) + new Vector2(8, 8) - Main.screenPosition;
                Vector2 center = neckOrigin + new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly) * 40 * k, -100);
                Vector2 finalCenter = neckOrigin + new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly) * 40 * k, -100);
                Vector2 distToProj = neckOrigin - center;
                float projRotation = distToProj.ToRotation() - 1.57f;
                float distance = distToProj.Length();
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                while (distance > 4 && !float.IsNaN(distance))
                {
                    distToProj.Normalize();
                    distToProj *= 4;
                    center += distToProj;
                    distToProj = neckOrigin - center;
                    distance = distToProj.Length();

                    //Draw chain
                    spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessories/BrainAcc_Chain").Value, center + zero,
                        null, Lighting.GetColor(new(i, j)), projRotation,
                        new Vector2(8 * 0.5f, 4 * 0.5f), 1f, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(Request<Texture2D>("EbonianMod/Items/Accessories/TinyBrain").Value, finalCenter + zero,
                            frame, Lighting.GetColor(new(i, j)), 0,
                            new Vector2(34 * 0.5f, 34 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            frameCounter++;
            if (frameCounter < 2)
            {
                frame.Y = 0 * 40;
            }
            else if (frameCounter < 4)
            {
                frame.Y = 1 * 40;
            }
            else if (frameCounter < 6)
            {
                frame.Y = 2 * 40;
            }
            else if (frameCounter < 8)
            {
                frame.Y = 3 * 40;
            }
            else
            {
                frameCounter = 0;
            }
            frame = new Rectangle(0, frame.Y, Request<Texture2D>("EbonianMod/Items/Accessories/TinyBrain").Value.Width, Request<Texture2D>("EbonianMod/Items/Accessories/TinyBrain").Value.Height / 4);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
