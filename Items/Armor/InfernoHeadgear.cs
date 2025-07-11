namespace EbonianMod.Items.Armor;

[AutoloadEquip(EquipType.Head)]
public class InfernoHeadgear : ModItem
{

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.defense = 16;
        Item.rare = ItemRarityID.Yellow;
    }
    public override bool IsLoadingEnabled(Mod mod) => false;

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemType<InfernoBreastplate>() && legs.type == ItemType<InfernoLeggings>();
    }
}
