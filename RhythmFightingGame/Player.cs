using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace RhythmFightingGame
{
    public class Player : Sprite
    {
        public enum Facing
        {
            Left,
            Right
        }
        public Facing facing;

        bool dashing;
        Vector2 endDashPos;

        public Player (Texture2D loadedTex) : base(loadedTex)
        {
            facing = Facing.Right;
            dashing = false;
        }

        public void Dash(Facing direction)
        {
            if (!dashing)
            {
                dashing = true;
                if (direction == Facing.Left)
                {
                    endDashPos = new Vector2(pos.X - 300, pos.Y);
                    vel = new Vector2(-100, 0);
                }
                else if (direction == Facing.Right)
                {
                    endDashPos = new Vector2(pos.X + 300, pos.Y);
                    vel = new Vector2(100, 0);
                }
            }
        }

        public override void Update(GameTimeWrapper gameTime, GraphicsDevice graphicsDevice)
        {
            if (dashing)
            {
                if (facing == Facing.Left)
                {
                    if (pos.X <= endDashPos.X)
                    {
                        vel = Vector2.Zero;
                        dashing = false;
                    }
                }
                else if (facing == Facing.Right)
                {
                    if (pos.X >= endDashPos.X)
                    {
                        vel = Vector2.Zero;
                        dashing = false;
                    }
                }
            }
            base.Update(gameTime, graphicsDevice);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (facing == Facing.Left)
            {
                spriteBatch.Draw(tex, pos, null, color, rotation, origin, scale, SpriteEffects.FlipHorizontally, 0);
            }
            else if (facing == Facing.Right)
            {
                spriteBatch.Draw(tex, pos, null, color, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }
    }
}
