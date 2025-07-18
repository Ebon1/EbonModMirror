﻿using System;
using System.IO;

namespace EbonianMod.Projectiles.Bases;

public abstract class HeldSword : ModProjectile
{
    public int swingTime = 20;
    public int minSwingTime = 5;
    public float holdOffset = 50f;
    public float baseHoldOffset = 50f;
    public bool modifyCooldown;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //ADD TRUE MELEE ON HIT LATER.
        OnHit(target, hit, damageDone);
    }
    public virtual void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        SetExtraDefaults();

        swingTime = (int)Clamp(swingTime / (1 + (Main.LocalPlayer.GetAttackSpeed(DamageClass.Melee) - 1) / 3), minSwingTime, int.MaxValue);
        if (!modifyCooldown)
            Projectile.localNPCHitCooldown = swingTime;
        Projectile.timeLeft = swingTime;
        baseHoldOffset = holdOffset;
    }
    public virtual float Ease(float f)
    {
        return 1 - (float)Math.Pow(2, 10 * f - 10);
    }
    public virtual float ScaleFunction(float progress)
    {
        return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
    }
    public virtual void SetExtraDefaults()
    {

    }
    /// <summary>
    /// AI0 is unused, AI1 is the direction of the sword, if AI2 is anything but 0, the default AI() and Kill() methods won't run.
    /// </summary>
    public virtual void ExtraAI()
    {

    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Projectile.localAI[0]);
        writer.Write(Projectile.localAI[1]);
        writer.Write(Projectile.localAI[2]);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Projectile.localAI[0] = reader.ReadSingle();
        Projectile.localAI[1] = reader.ReadSingle();
        Projectile.localAI[2] = reader.ReadSingle();
    }
    public Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;
    public bool useHeld = true;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.active || player.dead || player.CCed || player.noItems)
        {
            return;
        }

        player.itemTime = 2;
        player.itemAnimation = 2;

        if (Projectile.ai[2] != 0)
        {
            ExtraAI();
            return;
        }
        if (useHeld)
            player.heldProj = Projectile.whoAmI;
        if (Projectile.ai[1] != 0)
        {
            int direction = (int)Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (PiOver2 + PiOver4);
            float end = defRot + (PiOver2 + PiOver4);
            float rotation = direction == 1 ? start + Pi * 3 / 2 * swingProgress : end - Pi * 3 / 2 * swingProgress;
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - PiOver2) +
                rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + PiOver4;
            if (player.direction != (Projectile.velocity.X < 0 ? -1 : 1))
            {
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
            }
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, stretch, rotation - PiOver2);
        }
        else
        {
            float progress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            holdOffset = baseHoldOffset * (progress + 0.25f);
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = Projectile.velocity.ToRotation() * player.direction;
            pos += Projectile.velocity.ToRotation().ToRotationVector2() * holdOffset;
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - PiOver2);
            Projectile.rotation = (pos - player.Center).ToRotation() + PiOver2 - PiOver4 * Projectile.spriteDirection;
            Projectile.Center = pos;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }
        ExtraAI();
    }
    public override void OnKill(int timeLeft)
    {
        Player player = Main.player[Projectile.owner];
        if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems && Projectile.ai[2] == 0)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, -Projectile.ai[1]);
                proj.rotation = Projectile.rotation;
                proj.Center = Projectile.Center;
                proj.SyncProjectile();
            }
        }
    }
    public override bool ShouldUpdatePosition() => false;
    public float glowAlpha;
    public BlendState glowBlend;
    public Vector2 visualOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 orig = texture.Size() / 2;
        Vector2 off = new Vector2(0, Main.player[Projectile.owner].gfxOffY);
        Main.EntitySpriteDraw(texture, Projectile.Center + visualOffset + off - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
        if (glowAlpha > 0 && glowBlend is not null)
        {
            Texture2D glow = Helper.GetTexture(GlowTexture).Value;
            Main.spriteBatch.Reload(glowBlend);
            Main.EntitySpriteDraw(glow, Projectile.Center + visualOffset + off - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White * glowAlpha, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Player player = Main.player[Projectile.owner];
        float rot = Projectile.rotation - PiOver4;
        Vector2 start = player.Center;
        Vector2 end = player.Center + rot.ToRotationVector2() * (Projectile.height + holdOffset * 0.8f);
        float a = 0;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, projHitbox.Width, ref a) && Collision.CanHitLine(Projectile.Center + Projectile.Center.FromAToB(end) * 20, 5, 5, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height);
    }
}
