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

        public void Update(GameTimeWrapper gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.X == ButtonState.Pressed && previousGamePadState.Buttons.X == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D1) && previousKeyboardState.IsKeyUp(Keys.D1))
            {
                SetBeat();
                Game1.colorSheetAlpha.Value += 0.1f;
            }
            else if (gamePadState.Buttons.Y == ButtonState.Pressed && previousGamePadState.Buttons.Y == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D2) && previousKeyboardState.IsKeyUp(Keys.D2))
            {
                SetBeat();
                Game1.colorSheetAlpha.Value += 0.1f;
            }
            else if (gamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released ||
                keyboardState.IsKeyDown(Keys.D3) && previousKeyboardState.IsKeyUp(Keys.D3))
            {
                SetBeat();
                Game1.colorSheetAlpha.Value += 0.1f;
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
