using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GLX;

namespace RhythmFightingGame
{
    public class RhythmHandler
    {
        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;

        public TimeSpan timeSincePressed;
        bool waitingForPress;

        public TimeSpan rhythmTime;
        public TimeSpan offBeatTime;

        public RhythmHandler()
        {
            previousGamePadState = GamePad.GetState(PlayerIndex.One);
            previousKeyboardState = Keyboard.GetState();
            timeSincePressed = TimeSpan.Zero;
            rhythmTime = TimeSpan.Zero;
            offBeatTime = TimeSpan.Zero;
            waitingForPress = false;
        }

        public void Update(GameTimeWrapper gameTime, Player player, EnemyHandler enemyHandler)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            Enemy closestEnemy = null;
            float distanceFromEnemy = float.MaxValue;
            foreach (Enemy enemy in enemyHandler.enemies)
            {
                if (enemy.visible)
                {
                    if (Math.Abs(enemy.pos.X - player.pos.X) < distanceFromEnemy)
                    {
                        distanceFromEnemy = Math.Abs(enemy.pos.X - player.pos.X);
                        closestEnemy = enemy;
                    }
                }
            }

            if (gamePadState.Buttons.X == ButtonState.Pressed && previousGamePadState.Buttons.X == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D1) && previousKeyboardState.IsKeyUp(Keys.D1))
            {
                SetBeat();
                player.animations.currentAnimation = "attack";
                if (CanAttackEnemy(player, closestEnemy))
                {
                    if (closestEnemy.attacks[0].color == Color.Blue)
                    {
                        AttackedEnemy(closestEnemy, enemyHandler);
                    }
                }
            }
            else if (gamePadState.Buttons.Y == ButtonState.Pressed && previousGamePadState.Buttons.Y == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D2) && previousKeyboardState.IsKeyUp(Keys.D2))
            {
                SetBeat();
                player.animations.currentAnimation = "attack";
                if (CanAttackEnemy(player, closestEnemy))
                {
                    if (closestEnemy.attacks[0].color == Color.Yellow)
                    {
                        AttackedEnemy(closestEnemy, enemyHandler);
                    }
                }
            }
            else if (gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D3) && previousKeyboardState.IsKeyUp(Keys.D3))
            {
                SetBeat();
                player.animations.currentAnimation = "attack";
                if (CanAttackEnemy(player, closestEnemy))
                {
                    if (closestEnemy.attacks[0].color == Color.Red)
                    {
                        AttackedEnemy(closestEnemy, enemyHandler);
                    }
                }
            }
            else if (gamePadState.Triggers.Left > 0.5f && previousGamePadState.Triggers.Left < 0.5f ||
                keyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left))
            {
                SetBeat();
            }
            else if (gamePadState.Triggers.Right > 0.5f && previousGamePadState.Triggers.Right < 0.5f ||
                keyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right))
            {
                SetBeat();
            }

            if (Game1.colorSheetAlpha.Value > 1)
            {
                Game1.colorSheetAlpha.Value = 1;
            }

            if (waitingForPress)
            {
                timeSincePressed += gameTime.ElapsedGameTime;
                if (timeSincePressed > TimeSpan.FromSeconds(2))
                {
                    waitingForPress = false;
                    timeSincePressed = TimeSpan.Zero;
                    rhythmTime = TimeSpan.Zero;
                    offBeatTime = TimeSpan.Zero;
                    Game1.colorSheetAlpha.Value = 0;
                }
            }

            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        bool CanAttackEnemy(Player player, Enemy enemy)
        {
            if (player.facing == Player.Direction.Left)
            {
                return enemy.pos.X - player.pos.X == -250;
            }
            else if (player.facing == Player.Direction.Right)
            {
                return enemy.pos.X - player.pos.X == 250;
            }
            return false;
        }

        void AttackedEnemy(Enemy closestEnemy, EnemyHandler enemyHandler)
        {
            Game1.colorSheetAlpha.Value += 0.1f;
            closestEnemy.attacks.RemoveAt(0);
            if (closestEnemy.attacks.Count == 0)
            {
                closestEnemy.visible = false;
                enemyHandler.enemiesAlive--;
                enemyHandler.enemyPositions.Remove(closestEnemy.pos.X);
            }
        }

        void SetBeat()
        {
            if (waitingForPress)
            {
                if (rhythmTime != TimeSpan.Zero)
                {
                    offBeatTime = timeSincePressed - rhythmTime;
                }
                if (rhythmTime == TimeSpan.Zero)
                {
                    rhythmTime = timeSincePressed;
                }
                timeSincePressed = TimeSpan.Zero;
            }
            else
            {
                timeSincePressed = TimeSpan.Zero;
                waitingForPress = true;
            }
        }
    }
}
