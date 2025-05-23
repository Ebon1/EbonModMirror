﻿using EbonianMod.Items.Accessories;
using EbonianMod.Projectiles;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Projectiles.Friendly.Generic;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace EbonianMod.Common.Players;
public class AccessoryPlayer : ModPlayer
{
    public bool brainAcc, heartAcc, hotShield, rei, reiV, xTent, starBit, goldenTip;
    public int reiBoostCool, reiBoostT, xTentCool, goldenTipI;
    public override void ResetEffects()
    {
        reiBoostCool--;
        xTentCool--;
        if (reiBoostCool > 0)
            reiBoostT--;
        if (!goldenTip)
            goldenTipI = 0;
        rei = false;
        reiV = false;
        hotShield = false;
        brainAcc = false;
        xTent = false;
        heartAcc = false;
        starBit = false;
        goldenTip = false;
    }
    public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (item.DamageType == DamageClass.Magic && xTent && xTentCool <= 0 && Main.myPlayer == Player.whoAmI)
        {
            Projectile p = Projectile.NewProjectileDirect(source, position, Helper.FromAToB(position, Main.MouseWorld) * 8, ProjectileType<XAmethyst>(), 50, 0);
            p.DamageType = DamageClass.Magic;
            p.friendly = true;
            p.hostile = false;
            xTentCool = 60;
            p.SyncProjectile();
        }
        return base.Shoot(item, source, position, velocity, type, damage, knockback);
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (starBit && !target.friendly && hit.Crit && target.lifeMax > 10 && target.type != NPCID.TargetDummy)
        {
            // TO DO : Projectile spawn from sky
            Vector2 pos = Vector2.Zero;
            int random = Main.rand.Next(1, 3);
            Vector2 spawnpos = new Vector2(0, Player.position.Y + 900);
            if (random == 1)
                pos = new Vector2(Player.position.X + 1000, spawnpos.Y - Main.rand.Next(1, 1800));
            if (random == 2)
                pos = new Vector2(Player.position.X - 1000, spawnpos.Y - Main.rand.Next(1, 1800));
            Vector2 direction = Helper.FromAToB(pos, target.Center + target.velocity);

            Projectile.NewProjectile(Player.GetSource_FromThis(), pos, direction * 25, ModContent.ProjectileType<StarBitBlue>(), Player.HeldItem.damage * 2, 4f, Main.myPlayer);
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (starBit && !target.friendly && hit.Crit && target.lifeMax > 10 && target.type != NPCID.TargetDummy)
        {
            int random = Main.rand.Next(1, 3);
            Vector2 spawnpos = new Vector2(0, Player.position.Y + 900);
            Vector2 pos = Vector2.Zero;
            if (random == 1)
                pos = new Vector2(Player.position.X + 1000, spawnpos.Y - Main.rand.Next(1, 1800));
            if (random == 2)
                pos = new Vector2(Player.position.X - 1000, spawnpos.Y - Main.rand.Next(1, 1800));
            Vector2 direction = Helper.FromAToB(pos, target.Center + target.velocity);

            Projectile.NewProjectile(Player.GetSource_FromThis(), pos, direction * 25, ModContent.ProjectileType<StarBitBlue>(), Player.HeldItem.damage * 2, 4f, Main.myPlayer);
        }
    }
    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (reiBoostCool > 20)
        {
            modifiers.Cancel();
            Player.AddImmuneTime(ImmunityCooldownID.General, 40);
            Player.AddImmuneTime(ImmunityCooldownID.Bosses, 40);
            Player.AddImmuneTime(ImmunityCooldownID.DD2OgreKnockback, 40);
        }
        if (NPC.AnyNPCs(NPCType<TinyBrain>()))
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.active && npc.type == NPCType<TinyBrain>())
                {
                    npc.life = 0;
                    npc.checkDead();
                    break;
                }
            }
        }
    }
    public override void PostUpdateRunSpeeds()
    {
        if (rei)
        {
            Player.maxRunSpeed += .5f;
            Player.accRunSpeed += .5f;
        }
        if (hotShield)
            Player.CancelAllBootRunVisualEffects();
    }
    public override void PostUpdateMiscEffects()
    {
        if (hotShield)
        {
            Player.CancelAllBootRunVisualEffects();
            Player.accRunSpeed *= 0.7f;
            Player.moveSpeed *= 0.7f;
        }
    }
    public override void UpdateLifeRegen()
    {
        if (brainAcc)
            Player.lifeRegen += 5;
    }
    public override void PostUpdate()
    {
        if (hotShield)
        {
            Player.CancelAllBootRunVisualEffects();
        }
    }
}
