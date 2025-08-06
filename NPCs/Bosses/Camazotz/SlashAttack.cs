using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EchosOfOblivion.NPCs.Bosses.Camazotz
{
    public class SlashAttack : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 20)
            {
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }

            Projectile.velocity = new Vector2(12f, 0f); // Fast horizontal motion
            Projectile.rotation = Projectile.velocity.ToRotation(); // Face direction of motion

            // Optional: spawn dust for visual flair
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 1.5f);
            }
        }
    }
}
