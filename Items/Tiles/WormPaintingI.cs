﻿using EbonianMod.Tiles.Paintings;

namespace EbonianMod.Items.Tiles;

public class WormPaintingI : ModItem
{
    public override void SetStaticDefaults()
    {
    }
    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = 0;
        Item.useTurn = true;
        Item.rare = 0;
        Item.autoReuse = true;
        Item.useAnimation = 15;
        Item.rare = ItemRarityID.Green;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.value = Item.buyPrice(0, 1, 0, 0);
        Item.createTile = TileType<WormPainting>();
    }
}
