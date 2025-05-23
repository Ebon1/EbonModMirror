﻿using EbonianMod.Tiles;
using Terraria.GameContent.Creative;

namespace EbonianMod.Items.Tiles;

internal class HotMusic : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Garbage"), ItemType<HotMusic>(), TileType<ThisShitSomeHotGarbage>());
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.createTile = TileType<ThisShitSomeHotGarbage>();
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(0, 10, 0, 0);
        Item.accessory = true;
    }
    public override bool? PrefixChance(int pre, UnifiedRandom rand)
    {
        return false;
    }
}
