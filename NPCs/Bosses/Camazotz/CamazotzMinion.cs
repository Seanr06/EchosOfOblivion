using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EchosOfOblivion.NPCs.Bosses.Camazotz
{
    public class CamazotzMinion : ModNPC
    {

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 100;
            NPC.damage = 50;
            NPC.defense = 10;
            NPC.width = 14;
            NPC.height = 12;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.scale = 1.25f;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead || (!player.ZoneRockLayerHeight && !player.ZoneDirtLayerHeight))
            {
                // Despawn
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (player.dead || (!player.ZoneRockLayerHeight && !player.ZoneDirtLayerHeight))
                {
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    if (player.ZoneUnderworldHeight)
                    {
                        NPC.velocity.Y -= 0.1f;
                    }
                    else
                    {
                        NPC.velocity.Y += 0.1f;
                    }
                    return;
                }
            }
            moveToPlayer(player, 5f, .25f);

        }

        private void moveToPlayer(Player player, float moveSpeed, float accelerationRate)
        {
            // Set Distance to Player
            NPC.rotation = 0f;
            NPC.spriteDirection = (NPC.velocity.X < 0) ? -1 : 1;

            float distanceToPlayer = Vector2.Distance(NPC.Center, player.Center);

            // Set Move Speeds
            float movementSpeed = moveSpeed / distanceToPlayer;

            float targetVelocityX = (player.Center.X - NPC.Center.X) * movementSpeed;
            float targetVelocityY = (player.Center.Y - NPC.Center.Y) * movementSpeed;

            // Apply Acceleration
            if (NPC.velocity.X < targetVelocityX)
            {
                // Increase Velocity by Acceleration
                NPC.velocity.X += accelerationRate;

                // Further increase velocity
                if (NPC.velocity.X < 0f && targetVelocityX > 0f)
                {
                    NPC.velocity.X += accelerationRate;
                }
            }

            if (NPC.velocity.X > targetVelocityX)
            {
                // Increase Velocity by Acceleration
                NPC.velocity.X -= accelerationRate;

                // Further increase velocity
                if (NPC.velocity.X > 0f && targetVelocityX < 0f)
                {
                    NPC.velocity.X -= accelerationRate;
                }
            }

            if (NPC.velocity.Y < targetVelocityY)
            {
                // Increase Velocity by Acceleration
                NPC.velocity.Y += accelerationRate;

                // Further increase velocity
                if (NPC.velocity.Y < 0f && targetVelocityY > 0f)
                {
                    NPC.velocity.Y += accelerationRate;
                }
            }

            if (NPC.velocity.Y > targetVelocityY)
            {
                // Increase Velocity by Acceleration
                NPC.velocity.Y -= accelerationRate;

                // Further increase velocity
                if (NPC.velocity.Y > 0f && targetVelocityY < 0f)
                {
                    NPC.velocity.Y -= accelerationRate;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter == 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = (NPC.frame.Y + frameHeight) % (4 * frameHeight);
            }
        }
    }
}
