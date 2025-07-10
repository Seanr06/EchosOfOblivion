using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EchosOfOblivion.NPCs.Bosses.Zotzkar
{
    public class Camazotz : ModNPC
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

        }
        public override void SetDefaults()
        {

        }

        public override void AI()
        {
            
        }

        public override void FindFrame(int frameHeight)
        {
            
        }

        public override bool CheckDead()
        {
            return true;
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
