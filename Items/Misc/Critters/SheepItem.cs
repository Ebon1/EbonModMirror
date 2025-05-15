﻿using EbonianMod.NPCs.Overworld.Critters;

namespace EbonianMod.Items.Misc.Critters;

public class SheepItem : ModItem
{
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Bunny);
        Item.makeNPC = NPCType<Sheep>();
        Item.value = Item.buyPrice(0, 0, 10, 0);
    }
}
public class SheepItemNaked : ModItem
{
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Bunny);
        Item.makeNPC = 0;
        Item.value = Item.buyPrice(0, 0, 10, 0);
    }
    public override bool? UseItem(Player player)
    {
        NPC.NewNPCDirect(null, Main.MouseWorld, NPCType<Sheep>(), 0, ai2: 1);
        return true;
    }
}
