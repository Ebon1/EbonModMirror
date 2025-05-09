﻿using EbonianMod.Items.Weapons.Magic;
using EbonianMod.NPCs.ArchmageX;
using EbonianMod.Tiles;
using Terraria.ModLoader.IO;


namespace EbonianMod;

public class EbonianSystem : ModSystem
{
    public static float savedMusicVol, setMusicBackTimer, setMusicBackTimerMax;
    public int timesDiedToXareus;

    public bool downedXareus = false, gotTheStaff = false, xareusFuckingDies = false;
    public int constantTimer;

    public static float conglomerateSkyFlash;
    public Color conglomerateSkyColorOverride;

    public static bool heardXareusIntroMonologue;
    public static int xareusFightCooldown;
    public static float deltaTime;
    public override void Load()
    {
        heardXareusIntroMonologue = false;
        xareusFightCooldown = 0;
    }
    public static float FlashAlpha, DarkAlpha;
    public Point staffTilePosition;
    public override void PostUpdateEverything()
    {
        if (FlashAlpha > 0)
            FlashAlpha -= 0.01f;

        if (!Main.gameInactive)
            DarkAlpha = Lerp(DarkAlpha, 0, 0.1f);
        if (DarkAlpha < .05f)
            DarkAlpha = 0;
        conglomerateSkyFlash = Lerp(conglomerateSkyFlash, 0, 0.07f);
        conglomerateSkyColorOverride = Color.Lerp(conglomerateSkyColorOverride, Color.White, 0.03f);
        if (conglomerateSkyFlash < 0.05f)
        {
            conglomerateSkyFlash = 0;
            conglomerateSkyColorOverride = Color.White;
        }

        xareusFightCooldown--;
        constantTimer++;

        if (!NPC.AnyNPCs(NPCType<ArchmageStaffNPC>()) && constantTimer % 600 == 0)
        {
            if (staffTilePosition == Point.Zero || Main.tile[staffTilePosition.X, staffTilePosition.Y].HasTile
                || Main.tile[staffTilePosition.X, staffTilePosition.Y].TileType != (ushort)TileType<ArchmageStaffTile>())
            {
                for (int i = Main.maxTilesX / 2 - 440; i < Main.maxTilesX / 2 + 440; i++)
                    for (int j = 135; j < Main.maxTilesY / 2; j++)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == (ushort)TileType<ArchmageStaffTile>())
                        {
                            staffTilePosition = new Point(i, j);
                            break;
                        }
                    }
            }
        }
        if (!NPC.AnyNPCs(NPCType<ArchmageStaffNPC>()) && constantTimer % 60 == 0)
        {
            if (staffTilePosition != Point.Zero && Main.tile[staffTilePosition.X, staffTilePosition.Y].HasTile && Main.tile[staffTilePosition.X, staffTilePosition.Y].TileType == (ushort)TileType<ArchmageStaffTile>())
            {
                MPUtils.NewNPC(new Vector2(staffTilePosition.X * 16 + 20, staffTilePosition.Y * 16 + 40), NPCType<ArchmageStaffNPC>(), ai3: 1);
            }
        }
    }
    public override void SaveWorldData(TagCompound tag)
    {
        tag.Set("XarusDown", downedXareus);
        tag.Set("XarusDownForReal", gotTheStaff);
        tag.Set("XarusDownForRealReal", xareusFuckingDies);
        tag.Set("XareusDieTimes", timesDiedToXareus);
    }
    public override void LoadWorldData(TagCompound tag)
    {
        downedXareus = tag.GetBool("XarusDown");
        gotTheStaff = tag.GetBool("XarusDownForReal");
        xareusFuckingDies = tag.GetBool("XarusDownForRealReal");
        timesDiedToXareus = tag.GetInt("XareusDieTimes");
    }
    public override void PostWorldGen()
    {
        downedXareus = false;
        gotTheStaff = false;
        xareusFuckingDies = false;
        timesDiedToXareus = 0;
    }
    public override void UpdateUI(GameTime gameTime)
    {
        if (--setMusicBackTimer < 0)
        {
            savedMusicVol = Main.musicVolume;
        }
        else
            Main.musicVolume = Lerp(savedMusicVol, 0, setMusicBackTimer / setMusicBackTimerMax);

        if (Main.WaveQuality == 0)
        {
            Main.NewText("Ebonian Mod doesn't currently work properly when the Wave Quality is set to Off.", Main.errorColor);
            Main.WaveQuality = 1;
        }

        if (Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy || Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro)
        {
            Main.NewText("Ebonian Mod doesn't currently work properly with Trippy or Retro lights.", Main.errorColor);
            Lighting.Mode = Terraria.Graphics.Light.LightMode.Color;
        }
    }
    public override void OnWorldLoad()
    {
        xareusFightCooldown = 0;

        if (gotTheStaff)
            ItemID.Sets.ShimmerTransformToItem[ItemID.AmethystStaff] = ItemType<StaffOfX>();
        else
            ItemID.Sets.ShimmerTransformToItem[ItemID.AmethystStaff] = 0;
    }
    public static void TemporarilySetMusicTo0(float time)
    {
        savedMusicVol = Main.musicVolume;
        setMusicBackTimer = time;
        setMusicBackTimerMax = time;
    }
}
