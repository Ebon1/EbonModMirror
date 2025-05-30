﻿namespace EbonianMod.Projectiles.VFXProjectiles;

public class BloodShockwave : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.explosion.Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Maroon * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.05f;
        if (Projectile.ai[0] > 1)
            Projectile.Kill();
    }
}
public class BloodShockwave2 : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.Extras2.circle_02.Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Maroon * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.05f;
        if (Projectile.ai[0] > 1)
            Projectile.Kill();
    }
}

public class InferosShockwave : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.Extras2.circle_02.Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0] / 2);
        Color color = Color.Lerp(Color.OrangeRed, Color.Yellow, alpha);
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.075f;
        if (Projectile.ai[0] > 1.5f)
            Projectile.Kill();
    }
}
public class InferosShockwave2 : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.Extras2.circle_02.Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0] * 2);
        Color color = Color.Lerp(Color.OrangeRed, Color.Yellow, alpha);
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.025f;
        if (Projectile.ai[0] > 0.5f)
            Projectile.Kill();
    }
}
