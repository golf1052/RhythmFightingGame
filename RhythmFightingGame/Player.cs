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
        public enum Direction
        {
            Left,
            Right,
            None
        }
        public Direction facing;
        public Direction dashingDirection;
        float dashDistance = 500;
        float dashSpeed = 50;

        bool dashing;
        public Vector2 endDashPos;

        public Player(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime) : base(spriteSheetInfo, gameTime)
        {
            facing = Direction.Right;
            dashing = false;
            endDashPos = Vector2.Zero;
        }

        public void Dash(Direction direction)
        {
            if (!dashing)
            {
                dashing = true;
                dashingDirection = direction;
                if (direction == Direction.Left)
                {
                    endDashPos = new Vector2(pos.X - dashDistance, pos.Y);
                    vel = new Vector2(-dashSpeed, 0);
                    if (facing == Direction.Left)
                    {
                        animations.currentAnimation = "moveforwards";
                    }
                    else if (facing == Direction.Right)
                    {
                        animations.currentAnimation = "movebackwards";
                    }
                }
                else if (direction == Direction.Right)
                {
                    endDashPos = new Vector2(pos.X + dashDistance, pos.Y);
                    vel = new Vector2(dashSpeed, 0);
                    if (facing == Direction.Right)
                    {
                        animations.currentAnimation = "moveforwards";
                    }
                    else if (facing == Direction.Left)
                    {
                        animations.currentAnimation = "movebackwards";
                    }
                }
            }
        }

        public override void Update(GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            if (dashing)
            {
                if (dashingDirection == Direction.Left)
                {
                    if (pos.X <= endDashPos.X)
                    {
                        vel = Vector2.Zero;
                        dashing = false;
                        dashingDirection = Direction.None;
                    }
                }
                else if (dashingDirection == Direction.Right)
                {
                    if (pos.X >= endDashPos.X)
                    {
                        vel = Vector2.Zero;
                        dashing = false;
                        dashingDirection = Direction.None;
                    }
                }
            }
            base.Update(gameTime, graphics);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (facing == Direction.Left)
            {
                spriteBatch.Draw(tex, pos, null, color, rotation, origin, scale, SpriteEffects.FlipHorizontally, 0);
            }
            else if (facing == Direction.Right)
            {
                spriteBatch.Draw(tex, pos, null, color, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }
    }
}
