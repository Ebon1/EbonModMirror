﻿using EbonianMod.Items.Weapons.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ObjectData;

namespace EbonianMod.Tiles.Ambient;
public class RustyAxeT : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.Origin = Point16.Zero;
        TileObjectData.addTile(Type);
        Main.tileMerge[TileID.Sand][Type] = true;

        DustType = DustID.Bone;

        RegisterItemDrop(ItemType<RustyWaraxe>());

        AddMapEntry(Color.Cyan);
    }
    public override void RandomUpdate(int i, int j)
    {
        Main.NewText("I exist");
        Main.NewText(i + " " + j);
        Main.NewText(TileObjectData.GetTileData(Main.tile[i, j]).Origin);
        Main.NewText(Main.LocalPlayer.Center.ToTileCoordinates().X + " " + Main.LocalPlayer.Center.ToTileCoordinates().Y);
        if (Main.mouseRight)
            Main.LocalPlayer.Center = new Vector2(i, j) * 16;
    }
    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        yield return new Item(ItemType<RustyWaraxe>());
    }
}
