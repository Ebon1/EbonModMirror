using EbonianMod.Common.Graphics;
using EbonianMod.Common.Graphics.Skies;
using EbonianMod.Common.Misc;
using EbonianMod.NPCs.Aureus;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent.Skies;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace EbonianMod;

public class EbonianMod : Mod
{
    public static EbonianMod Instance => GetInstance<EbonianMod>();
    public static List<int> projectileFinalDrawList = new List<int>();
    public RenderTarget2D blurrender;
    public EbonianMod() => MusicSkipsVolumeRemap = true;
    public override void HandlePacket(BinaryReader reader, int whoAmI) => EbonianNetCode.HandlePackets(reader);
    public override void Load()
    {
        LoadEffects();
        LoadFilters();
        LoadDrawCache();
        Main.OnResolutionChanged += (Vector2 obj) => CreateRender();
        CreateRender();
    }
    public static List<Action> invisibleMaskCache = [], affectedByInvisibleMaskCache = [],
        blurDrawCache = [], pixelationDrawCache = [], finalDrawCache = [], garbageFlameCache = [],
        xareusGoopCache = [];
    public void LoadDrawCache()
    {
        invisibleMaskCache ??= [];
        affectedByInvisibleMaskCache ??= [];
        blurDrawCache ??= [];
        pixelationDrawCache ??= [];
        finalDrawCache ??= [];
        garbageFlameCache ??= [];
        xareusGoopCache ??= [];
    }


    public static Asset<Effect> bloom, colorQuant, softBloom, Tentacle, TentacleBlack, TentacleRT, ScreenDistort,
        SpriteRotation, TextGradient, TextGradient2, TextGradientY, BeamShader, Lens, Test1,
        Test2, LavaRT, Galaxy, CrystalShine, HorizBlur, TrailShader, RTAlpha, Crack, Blur,
        RTOutline, metaballGradient, metaballGradientNoiseTex, invisibleMask, PullingForce,
        displacementMap, waterEffect, spherize, flame;
    public static void LoadEffects()
    {
        static Asset<Effect> LoadEffect(string path) => Request<Effect>("EbonianMod/Effects/" + path); // line of code STOLEN *DIRECTLY* from ONE OF ZEN'S MODS. THANKS, IDIOT!
        bloom = LoadEffect("bloom");
        colorQuant = LoadEffect("colorQuant");
        Test1 = LoadEffect("Test1");
        HorizBlur = LoadEffect("horizBlur");
        Blur = LoadEffect("Blur");
        Crack = LoadEffect("crackTest");
        RTAlpha = LoadEffect("RTAlpha");
        RTOutline = LoadEffect("RTOutline");
        CrystalShine = LoadEffect("CrystalShine");
        TextGradient = LoadEffect("TextGradient");
        TextGradient2 = LoadEffect("TextGradient2");
        TextGradientY = LoadEffect("TextGradientY");
        Test2 = LoadEffect("Test2");
        Galaxy = LoadEffect("Galaxy");
        LavaRT = LoadEffect("LavaRT");
        SpriteRotation = LoadEffect("spriteRotation");
        BeamShader = LoadEffect("Beam");
        Lens = LoadEffect("Lens");
        Tentacle = LoadEffect("Tentacle");
        TentacleRT = LoadEffect("TentacleRT");
        ScreenDistort = LoadEffect("DistortMove");
        TentacleBlack = LoadEffect("TentacleBlack");
        TrailShader = LoadEffect("TrailShader");
        metaballGradient = LoadEffect("metaballGradient");
        metaballGradientNoiseTex = LoadEffect("metaballGradientNoiseTex");
        invisibleMask = LoadEffect("invisibleMask");
        PullingForce = LoadEffect("PullingForce");
        displacementMap = LoadEffect("displacementMap");
        waterEffect = LoadEffect("waterEffect");
        spherize = LoadEffect("spherize");
        flame = LoadEffect("flameEffect");
    }
    public void LoadFilters()
    {
        Filters.Scene["EbonianMod:CorruptTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.68f, .56f, .73f).UseOpacity(0.35f), EffectPriority.Medium);
        SkyManager.Instance["EbonianMod:CorruptTint"] = new BasicTint();

        Filters.Scene["EbonianMod:XMartian"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0, 0, 0).UseOpacity(0), EffectPriority.High);
        SkyManager.Instance["EbonianMod:XMartian"] = new MartianSky();

        Filters.Scene["EbonianMod:CrimsonTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.75f, 0f, 0f).UseOpacity(0.35f), EffectPriority.Medium);
        SkyManager.Instance["EbonianMod:CrimsonTint"] = new BasicTint();

        Filters.Scene["EbonianMod:Conglomerate"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0, 0, 0f).UseOpacity(0), EffectPriority.Medium);
        SkyManager.Instance["EbonianMod:Conglomerate"] = new ConglomerateSky();
        Filters.Scene["EbonianMod:Aureus"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.1f, .1f, .5f).UseOpacity(0.45f), EffectPriority.Medium);
        SkyManager.Instance["EbonianMod:Aureus"] = new AureusSky();

        Filters.Scene["EbonianMod:HellTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(2.55f, .97f, .31f).UseOpacity(0.1f), EffectPriority.Medium);
        SkyManager.Instance["EbonianMod:HellTint"] = new BasicTint();
        Filters.Scene["EbonianMod:HellTint2"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0.03f, 0f, .18f).UseOpacity(0.425f), EffectPriority.Medium);
        SkyManager.Instance["EbonianMod:HellTint2"] = new BasicTint();
        Filters.Scene["EbonianMod:ScreenFlash"] = new Filter(new ScreenShaderData(Request<Effect>("EbonianMod/Effects/ScreenFlash"), "Flash"), EffectPriority.VeryHigh);
    }
    public void CreateRender()
    {
        if (Main.netMode != NetmodeID.Server)
            Main.QueueMainThreadAction(() =>
            {
                if (Instance.blurrender != null)
                    if (!Instance.blurrender.IsDisposed)
                        Instance.blurrender.Dispose();
                Instance.blurrender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
    }
}
