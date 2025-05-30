﻿namespace EbonianMod.Dusts;

public class ReiSmoke : ModDust
{
    //OBSOLETE: ReiCape.cs uses its own particle system now.
    public override string Texture => "EbonianMod/Extras/Empty";
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        //if (dust.scale <= 1f && dust.scale >= 0.8f)
        //  dust.scale = 0.25f;
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale -= 0.0005f;
        dust.velocity *= 0.95f;
        if (dust.scale < 0.005f)
            dust.velocity *= 0.85f;
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }
    public static void DrawAll(SpriteBatch sb)
    {
        foreach (Dust d in Main.dust)
        {
            if (d.type == DustType<ReiSmoke>() && d.active)
            {
                Texture2D tex = Assets.Extras.fireball.Value;
                sb.Draw(tex, d.position - Main.screenPosition, null, Color.White * d.scale * 10, 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
            }
        }
    }
}
