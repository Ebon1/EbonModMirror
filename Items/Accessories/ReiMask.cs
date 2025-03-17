﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using EbonianMod.Dusts;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using EbonianMod.Common;
using Terraria.Localization;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class ReiMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Face.Sets.DrawInFaceHeadLayer[Item.faceSlot] = true;
            ArmorIDs.Face.Sets.PreventHairDraw[Item.faceSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(0, 20, 0, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.NightVisionHelmet).AddIngredient(ItemID.SoulofFright, 15).AddTile(TileID.MythrilAnvil).Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Text.Contains("{ReiM_Keybind}"));
            if (line != null)
            {
                line.Text = line.Text.Replace("{ReiM_Keybind}", "[" + EbonianKeybinds.ReiDash.GetAssignedKeys().FirstOrDefault("<unbound>") + "]");
            }
        }
        public override void UpdateVanity(Player player)
        {
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.reiV = true;
            if (player.ownedProjectileCounts[ProjectileType<ReiCapeP>()] < 1 && !modPlayer.sheep)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<ReiCapeP>(), 0, 0, player.whoAmI);
            }
            if (player.ownedProjectileCounts[ProjectileType<ReiCapeTrail>()] < 2 && !modPlayer.sheep)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (i == 0) continue;
                    Projectile.NewProjectileDirect(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<ReiCapeTrail>(), 0, 0, player.whoAmI, i).ai[0] = i;
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.rei = true;
            player.nightVision = true;
            if (player.ownedProjectileCounts[ProjectileType<ReiCapeP>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<ReiCapeP>(), 0, 0, player.whoAmI);
            }
            if (player.ownedProjectileCounts[ProjectileType<ReiCapeTrail>()] < 2)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (i == 0) continue;
                    Projectile.NewProjectileDirect(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<ReiCapeTrail>(), 0, 0, player.whoAmI, i).ai[0] = i;
                }
            }
            if (EbonianKeybinds.ReiDash.JustReleased && modPlayer.reiBoostCool <= 0)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active && !npc.friendly)
                    {
                        if (npc.Center.Distance(Main.MouseWorld) < npc.width + 50)
                        {
                            Vector2 pos = player.Center;
                            for (int i = 0; i < 50; i++)
                            {
                                for (int j = 0; j < 2; j++)
                                    Dust.NewDustPerfect(pos, DustID.Electric, Main.rand.NextVector2Circular(5, 5)).noGravity = true;
                                pos += Helper.FromAToB(pos, Main.MouseWorld, false) * 0.05f;
                            }
                            Helper.TPNoDust(Main.MouseWorld - new Vector2(0, 40), player);
                            Projectile.NewProjectile(null, Main.MouseWorld, Vector2.Zero, ProjectileType<ReiExplosion>(), 50, 0, player.whoAmI);

                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(npc.Center, Main.rand.NextVector2Unit(), 5, 6, 30, 1000));

                            SoundEngine.PlaySound(EbonianSounds.reiTP, Main.MouseWorld);
                            modPlayer.reiBoostCool = 60;
                            player.AddImmuneTime(ImmunityCooldownID.General, 30);
                            player.AddImmuneTime(ImmunityCooldownID.Bosses, 30);
                            break;
                        }
                    }
                }
                /*if (modPlayer.reiBoostCool <= 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustPerfect(player.Center, DustID.Electric, Main.rand.NextVector2Circular(5, 5));
                    }
                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, player.Center);
                    //SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/reiFail2"), player.Center);
                }*/
            }
            /*if (EbonianKeybinds.ReiDash.JustPressed && modPlayer.reiBoostCool > 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, player.Center);
                //SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/reiFail2"), player.Center);
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDustPerfect(player.Center, DustID.Electric, Main.rand.NextVector2Circular(5, 5));
                }
            }*/
        }
    }
}
