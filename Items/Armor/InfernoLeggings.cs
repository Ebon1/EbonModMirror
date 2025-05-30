namespace EbonianMod.Items.Armor;

[AutoloadEquip(EquipType.Legs)]
public class InfernoLeggings : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.rare = ItemRarityID.Yellow;
        Item.defense = 12;
    }
    public override bool IsLoadingEnabled(Mod mod) => false;
}