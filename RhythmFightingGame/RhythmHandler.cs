using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GLX;
using Microsoft.Xna.Framework.Graphics;

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
        public TimeSpan? beatTime;
        int beatCount = 0;

        public Sound beatStartSound;
        public Sound beatSound;

        Sprite[] collsionSprites;
        Sprite currentFlash;

        public Sprite arrow;

        bool flashing;
        FloatTweener flashingAlpha;

        public RhythmHandler()
        {
            previousGamePadState = GamePad.GetState(PlayerIndex.One);
            previousKeyboardState = Keyboard.GetState();
            collsionSprites = new Sprite[3];
            flashingAlpha = new FloatTweener();
            SetUpRhythmHandler();
        }

        public void SetUpRhythmHandler()
        {
            timeSincePressed = TimeSpan.Zero;
            rhythmTime = TimeSpan.Zero;
            offBeatTime = TimeSpan.Zero;
            waitingForPress = false;
            flashing = false;
        }

        public void LoadCollisionSprites(List<Sprite> collisions)
        {
            collsionSprites[0] = collisions[0];
            collsionSprites[1] = collisions[1];
            collsionSprites[2] = collisions[2];
        }

        public void Update(GameTimeWrapper gameTime, Player player, EnemyHandler enemyHandler, Boss boss)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (boss.visible)
            {
                
            }

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

            if (closestEnemy == null)
            {
                arrow.visible = false;
            }
            else
            {
                if (closestEnemy.pos.X - player.pos.X > 750)
                {
                    arrow.visible = true;
                    arrow.rotation = 0;
                }
                else if (closestEnemy.pos.X - player.pos.X < -750)
                {
                    arrow.visible = true;
                    arrow.rotation = 180;
                }

                if (Vector2.Distance(closestEnemy.pos, player.pos) == 250)
                {
                    arrow.visible = false;
                }
            }

            if (boss.visible)
            {
                float distanceFromBoss = Math.Abs(boss.pos.X - player.pos.X);
                if (boss.pos.X - player.pos.X > 750)
                {
                    arrow.visible = true;
                    arrow.rotation = 0;
                }
                else if (boss.pos.X - player.pos.X < -750)
                {
                    arrow.visible = true;
                    arrow.rotation = 180;
                }
                if (distanceFromBoss == 250)
                {
                    arrow.visible = false;
                }
            }

            if (gamePadState.Buttons.X == ButtonState.Pressed && previousGamePadState.Buttons.X == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D1) && previousKeyboardState.IsKeyUp(Keys.D1))
            {
                SetBeat();
                player.animations.currentAnimation = "punch";
                if (CanAttackEnemy(player, closestEnemy))
                {
                    if (closestEnemy.attacks[0].color == Color.Blue)
                    {
                        AttackedEnemy(closestEnemy, enemyHandler);
                    }
                }

                if (boss.visible)
                {
                    if (CanAttackBoss(player, boss))
                    {
                        if (boss.attacks[0].color == Color.Blue)
                        {
                            AttackedBoss(boss);
                        }
                    }
                }
            }
            else if (gamePadState.Buttons.Y == ButtonState.Pressed && previousGamePadState.Buttons.Y == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D2) && previousKeyboardState.IsKeyUp(Keys.D2))
            {
                SetBeat();
                player.animations.currentAnimation = "kick";
                if (CanAttackEnemy(player, closestEnemy))
                {
                    if (closestEnemy.attacks[0].color == Color.Yellow)
                    {
                        AttackedEnemy(closestEnemy, enemyHandler);
                    }
                }

                if (boss.visible)
                {
                    if (CanAttackBoss(player, boss))
                    {
                        if (boss.attacks[0].color == Color.Yellow)
                        {
                            AttackedBoss(boss);
                        }
                    }
                }
            }
            else if (gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D3) && previousKeyboardState.IsKeyUp(Keys.D3))
            {
                SetBeat();
                player.animations.currentAnimation = "punch";
                if (CanAttackEnemy(player, closestEnemy))
                {
                    if (closestEnemy.attacks[0].color == Color.Red)
                    {
                        AttackedEnemy(closestEnemy, enemyHandler);
                    }
                }

                if (boss.visible)
                {
                    if (CanAttackBoss(player, boss))
                    {
                        if (boss.attacks[0].color == Color.Red)
                        {
                            AttackedBoss(boss);
                        }
                    }
                }
            }
            else if (gamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
            {
                SetBeat();
                player.animations.currentAnimation = "jump";
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
                beatTime -= gameTime.ElapsedGameTime;
                if (beatTime != null)
                {
                    if (beatTime.Value <= TimeSpan.Zero)
                    {
                        beatCount++;
                        if (beatCount <= 3)
                        {
                            beatSound.Play();
                        }
                        else if (beatCount == 4)
                        {
                            beatStartSound.Play();
                            beatCount = 0;
                        }
                        beatTime = rhythmTime;
                    }
                }
                if (timeSincePressed > TimeSpan.FromSeconds(2))
                {
                    waitingForPress = false;
                    timeSincePressed = TimeSpan.Zero;
                    rhythmTime = TimeSpan.Zero;
                    offBeatTime = TimeSpan.Zero;
                    beatTime = null;
                    beatCount = 0;
                    Game1.colorSheetAlpha.Value = 0;
                }
            }

            flashingAlpha.Update(gameTime);
            if (flashing)
            {
                currentFlash.alpha = flashingAlpha.Value;
                if (flashingAlpha.Value == 1)
                {
                    flashingAlpha.Value = 0;
                }
                else if (flashingAlpha.Value == 0)
                {
                    flashing = false;
                }
            }

            arrow.pos = new Vector2(player.pos.X, 100);
            previousKeyboardState = keyboardState;
            previousGamePadState = gamePadState;
        }

        bool CanAttackEnemy(Player player, Enemy enemy)
        {
            if (enemy != null)
            {
                if (player.facing == Player.Direction.Left)
                {
                    return enemy.pos.X - player.pos.X == -250;
                }
                else if (player.facing == Player.Direction.Right)
                {
                    return enemy.pos.X - player.pos.X == 250;
                }
            }
            return false;
        }

        bool CanAttackBoss(Player player, Boss boss)
        {
            if (boss != null)
            {
                if (player.facing == Player.Direction.Left)
                {
                    return boss.pos.X - player.pos.X == -250;
                }
                else if (player.facing == Player.Direction.Right)
                {
                    return boss.pos.X - player.pos.X == 250;
                }
            }
            return false;
        }

        void AttackedEnemy(Enemy closestEnemy, EnemyHandler enemyHandler)
        {
            currentFlash = collsionSprites[World.random.Next(0, 3)];
            currentFlash.alpha = 0;
            currentFlash.pos = closestEnemy.pos;
            Flash();
            Game1.colorSheetAlpha.Value += 0.1f;
            closestEnemy.attacks.RemoveAt(0);
            if (closestEnemy.attacks.Count == 0)
            {
                closestEnemy.visible = false;
                enemyHandler.enemiesAlive--;
                enemyHandler.enemyPositions.Remove(closestEnemy.pos.X);
                enemyHandler.enemiesKilled++;
            }
        }

        void AttackedBoss(Boss boss)
        {
            currentFlash = collsionSprites[World.random.Next(0, 3)];
            currentFlash.alpha = 0;
            currentFlash.pos = boss.pos;
            Flash();
            Game1.colorSheetAlpha.Value += 0.1f;
            boss.attacks.RemoveAt(0);
            if (boss.attacks.Count == 0)
            {
                if (boss.animations.currentAnimation == "hover")
                {
                    boss.animations.currentAnimation = "damage";
                }
                else if (boss.animations.currentAnimation == "final")
                {
                    boss.visible = false;
                    boss.killed = true;
                    DebugText.debugTexts.Add(Game1.winText);
                }
            }
        }

        void Flash()
        {
            flashing = true;
            flashingAlpha.smoothingActive = true;
            flashingAlpha.smoothingRate = 0.1f;
            flashingAlpha.smoothingType = TweenerBase.SmoothingType.Linear;
            flashingAlpha.Value = 1;
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
                    beatStartSound.Play();
                    rhythmTime = timeSincePressed;
                    beatTime = rhythmTime;
                }
                timeSincePressed = TimeSpan.Zero;
            }
            else
            {
                beatStartSound.Play();
                timeSincePressed = TimeSpan.Zero;
                waitingForPress = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (flashing)
            {
                currentFlash.Draw(spriteBatch);
            }
            if (arrow.visible)
            {
                arrow.Draw(spriteBatch);
            }
        }
    }
}
