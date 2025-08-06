using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using System.Threading;
using Steamworks;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace EchosOfOblivion.NPCs.Bosses.Camazotz
{
    [AutoloadBossHead]
    public class Camazotz : ModNPC
    {

        private int state // BossPhase
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private int subState // AttackState
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private float stateTimer // AttackStateTimer
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        private float stateTimer2 // SecondAttackStateTimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        private Boolean isDashing = false;
        private Boolean isSlashing = false;

        private int horizontalDirection = 1; // 1 for right, -1 for left
        private float targetXDistance = 0;
        private Boolean foundTarget = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 54;
            NPC.height = 54;

            NPC.defense = 10;
            NPC.damage = Main.masterMode ? 50 : Main.expertMode ? 40 : 30;
            NPC.lifeMax = Main.masterMode ? 2000 : Main.expertMode ? 1500 : 3000;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(10);
            NPC.npcSlots = 30f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.SpawnWithHigherTime(30);
            Music = MusicID.Boss3;
        }


        public override void AI()
        {
            // Phase
            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            if (lifeRatio < 0.25f && state == 2f)
            {
                state = 3; // Transition to final phase when health is below 25%
            }
            else if (lifeRatio < 0.75f && state == 1f)
            {
                state = 2; // Transition to second phase when health is below 50%
            }
            else
            {
                state = 1; // Default to first phase
            }

            // Targetting
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

            stateTimer++;
            switch ((int)(state))
            {
                case 1: // Phase One
                    if (subState == 0) // AttackCooldown
                    {
                        if (stateTimer <= 300 ) 
                        {
                            HoverAbovePlayer(player);
                        }

                        if (stateTimer > 300)
                        {
                            Random rng = new Random();
                            subState = rng.Next(4, 5);
                            stateTimer = 0f;
                        }
                    }

                    if (subState == 1) // Dash at player
                    {
                        if (stateTimer <= 10)
                        {
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0f;
                            NPC.spriteDirection = (NPC.velocity.X < 0) ? 1 : -1;
                        }
                        if (stateTimer < 40 && stateTimer > 10)
                        {
                            moveToPlayer(player, 5f, 0.25f);    
                        }
                      
                        if (stateTimer == 40)
                        {
                            NPC.velocity = NPC.DirectionTo(player.Center) * 15f;
                            NPC.spriteDirection = (NPC.velocity.X < 0) ? 1 : -1;
                            NPC.rotation = NPC.velocity.ToRotation() + (float)((NPC.velocity.X < 0) ? Math.PI : 0);
                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                        }
                        else if (stateTimer > 40 && stateTimer < 100)
                        {
                            isDashing = true;
                            NPC.velocity *= 0.99f;
                        }

                        else if (stateTimer >= 100)
                        {
                            stateTimer = 0f;
                            stateTimer2++;
                            isDashing = false;

                            if (stateTimer2 >= 4)
                            {
                                subState = 0;
                                stateTimer2 = 0f;
                            }
                        } 
                    }
                    if (subState == 2) //echo attack
                    {
                        HoverAbovePlayer(player);
                        if (stateTimer >= 90)
                        {
                            Player target = Main.player[NPC.target];
                            Vector2 direction = Vector2.Normalize(target.Center - NPC.Center);
                            float speed = 8f;
                            Vector2 velocity = direction * speed;

                            int type = ModContent.ProjectileType<ScreechProjectile>();
                            int damage = 20;
                            float knockBack = 2f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, knockBack, Main.myPlayer);
                            stateTimer2++;
                            stateTimer = 0f;
                        }
                        if (stateTimer2 >= 5)
                        {
                            subState = 0;
                            stateTimer2 = 0f;
                        }
                    }
                    if (subState == 3) // Bat Swarm
                    {
                        if (stateTimer < 120)
                        {
                            NPC.velocity = Vector2.Zero;
                        }
                        if (stateTimer % 20 == 0)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CamazotzMinion>());
                        }
                        if (stateTimer >= 120)
                        {
                            stateTimer = 0f;
                            subState = 0;
                        }
                    }
                    if (subState == 4) // Slash attack
                    {
                        float slashDistance = 25f;
                        if (!isSlashing && stateTimer < 360)
                        {
                            moveToPlayer(player, 6f, 0.25f);
                        }
                        if (Vector2.Distance(NPC.Center, player.Center) < slashDistance) {
                            isDashing = true;
                            stateTimer = 260f;
                        }
                        if (isDashing && stateTimer <= 360)
                        {
                            NPC.velocity = Vector2.Zero;
                            if (stateTimer == 260)
                            {
                                Vector2 center = NPC.Center;
                                float horizontalOffset = 35f;
                                Vector2 leftPos = center + new Vector2(-horizontalOffset, 0f);
                                Vector2 rightPos = center + new Vector2(horizontalOffset, 0f);
                                int damage = 40;
                                float knockBack = 2f;
                                int type = ModContent.ProjectileType<SlashAttack>();

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), leftPos, Vector2.Zero, type, damage, knockBack, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), rightPos, Vector2.Zero, type, damage, knockBack, Main.myPlayer);

                            }
                        }
                        if (stateTimer >= 360)
                        {
                            isSlashing = false;
                            stateTimer = 0f;
                            subState = 0;
                        }
                    }


                    break;
                case 2: // Phase Two
                    
                    break;
                case 3: // Phase Three
                    
                    break;
                default:
                    NPC.active = false;
                    break;
            }
        }

        private void HoverAbovePlayer(Player player)
        {
            NPC.rotation = 0f;
            NPC.spriteDirection = (player.Center.X < NPC.Center.X) ? 1 : -1;

            float baseHeight = 300f;
            float hoverAmplitude = 50f;
            float hoverSpeed = 0.08f;
            float moveSpeed = 4f;
            float minAcceleration = 0.1f;
            float maxAcceleration = 0.25f;
            float overshootDistance = 120f;

            // Oscillating vertical offset
            float verticalOscillation = (float)Math.Sin(Main.GameUpdateCount * hoverSpeed) * hoverAmplitude;
            Vector2 hoverOffset = new Vector2(0f, -baseHeight + verticalOscillation);

            // === Initialize targetX on first run or after direction change ===
            if (!foundTarget)
            {
                targetXDistance = player.Center.X + horizontalDirection * overshootDistance;
                foundTarget = true;
            }

            // === Horizontal movement toward targetX ===
            float horizontalDistance = Math.Abs(targetXDistance - NPC.Center.X);
            float normalizedDistance = MathHelper.Clamp(horizontalDistance / 600f, 0f, 1f);
            float acceleration = MathHelper.Lerp(minAcceleration, maxAcceleration, normalizedDistance);

            float desiredVelocityX = (targetXDistance - NPC.Center.X > 0 ? 1f : -1f) * moveSpeed;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, desiredVelocityX, acceleration);

            // === Check if boss has reached or passed targetX ===
            if ((horizontalDirection == 1 && NPC.Center.X >= targetXDistance) ||
                (horizontalDirection == -1 && NPC.Center.X <= targetXDistance))
            {
                horizontalDirection *= -1; // Flip direction
                foundTarget = false; // Recalculate targetX next frame
            }

            // === Vertical float behavior ===
            Vector2 targetPosition = player.Center + hoverOffset;
            float desiredY = targetPosition.Y;
            float directionY = desiredY - NPC.Center.Y;
            float desiredVelocityY = directionY * 0.08f;
            NPC.velocity.Y = MathHelper.Clamp(desiredVelocityY, -4f, 4f);
        }

        private void moveToPlayer(Player player, float moveSpeed, float accelerationRate)
        {
            // Set Distance to Player
            NPC.rotation = 0f;
            NPC.spriteDirection = (NPC.velocity.X < 0) ? 1 : -1;

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

            if (isDashing)
            {
                NPC.frame.Y = frameHeight * 1;
                return;
            }

            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    NPC.frame.Y = 0;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcloot)
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

        }
    }
}
