using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GLX;

namespace RhythmFightingGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        World world;
        GameTimeWrapper mainGameTime;

        KeyboardState previousKeyboardState;
        GamePadState previousGamePadState;

        RhythmHandler rhythmHandler;

        Player player;
        BackgroundColor colorSheet;
        public static FloatTweener colorSheetAlpha;
        Background background;
        Background foreground;

        TextItem timeSinceLastButtonText;
        TextItem rhythmTimeText;
        TextItem offBeatText;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[0];
            Window.IsBorderless = true;
            Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
            graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            graphics.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            world = new World(graphics);
            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1.0m);
            world.AddTime(mainGameTime);
            world.camera1.focus = Camera.Focus.Center;
            DebugText.Initialize(Vector2.Zero, DebugText.Corner.TopLeft, 0);

            rhythmHandler = new RhythmHandler();

            colorSheetAlpha = new FloatTweener();
            colorSheetAlpha.Value = 0;
            colorSheetAlpha.smoothingActive = true;
            colorSheetAlpha.smoothingType = TweenerBase.SmoothingType.RecursiveLinear;
            colorSheetAlpha.smoothingRate = 0.01f;

            previousKeyboardState = Keyboard.GetState();
            previousGamePadState = GamePad.GetState(PlayerIndex.One);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            world.LoadSpriteBatch();
            player = new Player(new SpriteSheetInfo(288, 288), mainGameTime);
            player.animations["moveforwards"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("Rob-animation-forwards"), 15, 6, 3, SpriteSheet.Direction.LeftToRight, 25, false);
            player.animations["movebackwards"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("Rob-animation-backwards"), 15, 6, 3, SpriteSheet.Direction.LeftToRight, 25, false);
            player.Ready(graphics);
            player.animations.active = false;
            player.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - (player.tex.Height / 2 - 27));

            background = new Background(Content.Load<Texture2D>("Background"));
            foreground = new Background(Content.Load<Texture2D>("Foreground"));
            colorSheet = new BackgroundColor(graphics);
            colorSheet.drawRect = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            colorSheet.AddColor("forward", new Color(204, 150, 41));
            colorSheet.AddColor("backward", new Color(204, 110, 41));

            DebugText.spriteFont = Content.Load<SpriteFont>("DebugFont");
            timeSinceLastButtonText = new TextItem(DebugText.spriteFont);
            rhythmTimeText = new TextItem(DebugText.spriteFont);
            offBeatText = new TextItem(DebugText.spriteFont);
            DebugText.debugTexts.Add(timeSinceLastButtonText);
            DebugText.debugTexts.Add(rhythmTimeText);
            DebugText.debugTexts.Add(offBeatText);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            world.Update(gameTime);
        }

        public void MainUpdate(GameTimeWrapper gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                if (colorSheet.alpha > 0)
                {
                    colorSheet.alpha -= 0.01f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                if (colorSheet.alpha < 1)
                {
                    colorSheet.alpha += 0.01f;
                }
            }
            colorSheet.alpha = MathHelper.Clamp(colorSheet.alpha, 0, 1);

            if (gamePadState.Triggers.Left > 0.5f && previousGamePadState.Triggers.Left < 0.5f ||
                keyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left))
            {
                player.Dash(Player.Direction.Left);
            }
            else if (gamePadState.Triggers.Right > 0.5f && previousGamePadState.Triggers.Right < 0.5f ||
                keyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right))
            {
                player.Dash(Player.Direction.Right);
            }

            if (gamePadState.ThumbSticks.Left.X < -0.5f && previousGamePadState.ThumbSticks.Left.X > -0.5f ||
                keyboardState.IsKeyDown(Keys.Q) && previousKeyboardState.IsKeyUp(Keys.Q))
            {
                player.facing = Player.Direction.Left;
            }
            if (gamePadState.ThumbSticks.Left.X > 0.5f && previousGamePadState.ThumbSticks.Left.X < 0.5f ||
                keyboardState.IsKeyDown(Keys.E) && previousKeyboardState.IsKeyUp(Keys.E))
            {
                player.facing = Player.Direction.Right;
            }
            
            rhythmHandler.Update(gameTime);
            timeSinceLastButtonText.text = "Measure Time: " + rhythmHandler.timeSincePressed.TotalMilliseconds.ToString();
            rhythmTimeText.text = "Current Beat: " + rhythmHandler.rhythmTime.TotalMilliseconds.ToString();
            offBeatText.text = "Offby: " + rhythmHandler.offBeatTime.TotalMilliseconds.ToString();
            player.Update(gameTime, graphics);
            colorSheet.pos = new Vector2(player.pos.X - graphics.GraphicsDevice.Viewport.Width / 2, 0);
            colorSheetAlpha.Update(gameTime);
            colorSheet.alpha = colorSheetAlpha.Value;
            colorSheet.Update(gameTime, graphics);
            world.camera1.pan.Value = new Vector2(player.pos.X, graphics.GraphicsDevice.Viewport.Height / 2);
            DebugText.pos = new Vector2(world.camera1.pan.Value.X - graphics.GraphicsDevice.Viewport.Width / 2,
                world.camera1.pan.Value.Y - graphics.GraphicsDevice.Viewport.Height / 2);
            previousGamePadState = gamePadState;
            previousKeyboardState = keyboardState;
            world.UpdateCurrentCamera(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            world.BeginDraw();
            world.Draw(background.Draw);
            world.Draw(colorSheet.DrawRect);
            world.Draw(foreground.Draw);
            world.Draw(player.Draw);
            world.Draw(DebugText.Draw);
            world.EndDraw();
            base.Draw(gameTime);
        }
    }
}
