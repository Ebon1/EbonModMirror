﻿using EbonianMod.NPCs.ArchmageX;
using Terraria.Graphics.Effects;

namespace EbonianMod.Common.Detours;

public class MiscDetours : ModSystem
{
    public override void Load()
    {
        On_NPC.SetEventFlagCleared += EventClear;
        On_Main.Update += UpdateDeltaTime;
        On_FilterManager.CanCapture += (On_FilterManager.orig_CanCapture orig, FilterManager self) => true;
    }
    void UpdateDeltaTime(On_Main.orig_Update orig, Main self, GameTime gameTime)
    {
        float oldFrameRate = Main.frameRate;
        orig(self, gameTime);
        if (!Main.dedServ)
        {
            if (Main.FrameSkipMode == Terraria.Enums.FrameSkipMode.On) EbonianSystem.deltaTime = 1;
            else
            {
                float averageFrameRate = (Main.frameRate + oldFrameRate) / 2f;
                EbonianSystem.deltaTime = Clamp((float)(gameTime.TotalGameTime.TotalSeconds - gameTime.ElapsedGameTime.TotalSeconds) / (averageFrameRate), 0.2f, 1.1f);
            }
        }
    }
    void EventClear(On_NPC.orig_SetEventFlagCleared orig, ref bool eventFlag, int gameEventId)
    {
        if (gameEventId == 3 && !GetInstance<EbonianSystem>().xareusFuckingDies && GetInstance<EbonianSystem>().downedXareus)
        {
            MPUtils.NewNPC(Main.player[0].Center, NPCType<ArchmageCutsceneMartian>(), true);
            //NPC.NewNPCDirect(null, Main.player[0].Center, NPCType<ArchmageCutsceneMartian>(), 0, -1);
            GetInstance<EbonianSystem>().xareusFuckingDies = true;
        }
        orig(ref eventFlag, gameEventId);
    }
}
