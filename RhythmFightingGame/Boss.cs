using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace RhythmFightingGame
{
    public class Boss : Sprite
    {
        GraphicsDeviceManager graphics;
        public List<Sprite> attacks;
        public bool killed;

        public Boss(GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime) : base(spriteSheetInfo, gameTime)
        {
            this.graphics = graphics;
            attacks = new List<Sprite>();
        }

        public void SetUpBoss()
        {
            visible = false;
            animations.currentAnimation = "hover";
            killed = false;
        }

        public void Spawn(Player player)
        {
            if (!visible && !killed)
            {
                visible = true;
                GenerateAttacks();
                pos.X = player.endDashPos.X + (500 * World.random.Next(-4, 5)) + tex.Width / 2 + 60;
                pos.Y = graphics.GraphicsDevice.Viewport.Height - tex.Height / 2;
            }
        }

        public void GenerateAttacks()
        {
            int attackNumber = World.random.Next(6, 11);
            for (int i = 0; i < attackNumber; i++)
            {
                int attackType = World.random.Next(1, 4);
                Sprite attack = new Sprite(graphics);
                attack.drawRect = new Rectangle(0, 0, 10, 10);
                if (attackType == 1)
                {
                    attack.color = Color.Blue;
                }
                else if (attackType == 2)
                {
                    attack.color = Color.Yellow;
                }
                else if (attackType == 3)
                {
                    attack.color = Color.Red;
                }
                attacks.Add(attack);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                base.Draw(spriteBatch);
                for (int i = 0; i < attacks.Count; i++)
                {
                    attacks[i].pos = new Vector2(pos.X + 5 + ((attacks[i].drawRect.Width * 2) * i) - tex.Width / 4, pos.Y - tex.Height / 4 + 20);
                    attacks[i].drawRect.X = (int)attacks[i].pos.X;
                    attacks[i].drawRect.Y = (int)attacks[i].pos.Y;
                    attacks[i].DrawRect(spriteBatch);
                }
            }
        }
    }
}
