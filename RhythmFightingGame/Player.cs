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

        public enum State
        {
            Ground,
            Air
        }
        public State state;

        float dashDistance = 500;
        float dashSpeed = 50;

        float jumpHeight = 200;
        float jumpDistance = 0;
        bool hitJumpPeak = false;

        bool dashing;
        public Vector2 endDashPos;

        public Player(SpriteSheetInfo spriteSheetInfo, GameTimeWrapper gameTime) : base(spriteSheetInfo, gameTime)
        {
            SetUpPlayer();
        }

        public void SetUpPlayer()
        {
            facing = Direction.Right;
            dashing = false;
            endDashPos = Vector2.Zero;
            state = State.Ground;
        }

        public void Dash(Direction direction)
        {
            if (state == State.Air)
            {
                return;
            }
            if (animations.currentAnimation != "moveforwards" &&
                animations.currentAnimation != "movebackwards")
            {
                return;
            }
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
                        Game1.colorSheet.currentColorName = "forward";
                    }
                    else if (facing == Direction.Right)
                    {
                        animations.currentAnimation = "movebackwards";
                        Game1.colorSheet.currentColorName = "backward";
                    }
                }
                else if (direction == Direction.Right)
                {
                    endDashPos = new Vector2(pos.X + dashDistance, pos.Y);
                    vel = new Vector2(dashSpeed, 0);
                    if (facing == Direction.Right)
                    {
                        animations.currentAnimation = "moveforwards";
                        Game1.colorSheet.currentColorName = "forward";
                    }
                    else if (facing == Direction.Left)
                    {
                        animations.currentAnimation = "movebackwards";
                        Game1.colorSheet.currentColorName = "backward";
                    }
                }
            }
        }

        public void StartJump()
        {
            state = State.Air;
        }

        public void EndJump()
        {
            state = State.Ground;
        }

        public override void Update(GameTimeWrapper gameTime, GraphicsDeviceManager graphics)
        {
            if (state == State.Air)
            {
                if (!hitJumpPeak)
                {
                    pos.Y -= 20;
                    jumpDistance += 20;
                }
                else
                {
                    pos.Y += 20;
                    jumpDistance -= 20;
                }

                if (jumpDistance == jumpHeight)
                {
                    hitJumpPeak = true;
                }
                if (jumpDistance == 0)
                {
                    state = State.Ground;
                    hitJumpPeak = false;
                }
            }
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
