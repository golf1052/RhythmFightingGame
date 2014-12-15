using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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
        public static BackgroundColor colorSheet;
        public static FloatTweener colorSheetAlpha;
        Background background;
        Background foreground;

        EnemyHandler enemyHandler;

        TextItem timeSinceLastButtonText;
        TextItem rhythmTimeText;
        TextItem offBeatText;
        public static TextItem winText;

        Boss boss;

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
            world = new World(graphics);
            mainGameTime = new GameTimeWrapper(MainUpdate, this, 1.0m);
            SetUpStates();
            DebugText.Initialize(Vector2.Zero, DebugText.Corner.TopLeft, 0);
            world.LoadSpriteBatch();
            player = new Player(new SpriteSheetInfo(288, 288), mainGameTime);
            player.animations["moveforwards"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("Rob-animation-forwards"), 15, 6, 3, SpriteSheet.Direction.LeftToRight, 25, false);
            player.animations.currentAnimation = "moveforwards";
            player.animations.RunOneFrame();
            player.animations["movebackwards"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("Rob-animation-backwards"), 15, 6, 3, SpriteSheet.Direction.LeftToRight, 25, false);
            player.animations["punch"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("punch"), new SpriteSheetInfo(288, 288), 11, 11, 1, SpriteSheet.Direction.LeftToRight, 40, false);
            player.animations.SetFrameAction("punch", 10, () =>
                {
                    player.animations.currentAnimation = "moveforwards";
                    player.animations.RunOneFrame();
                });
            player.animations["kick"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("kick"), new SpriteSheetInfo(288, 288), 13, 13, 1, SpriteSheet.Direction.LeftToRight, 40, false);
            player.animations.SetFrameAction("kick", 12, () =>
                {
                    player.animations.currentAnimation = "moveforwards";
                    player.animations.RunOneFrame();
                });
            player.animations["jump"] = player.animations.AddSpriteSheet(Content.Load<Texture2D>("jump"), new SpriteSheetInfo(288, 288), 17, 9, 2, SpriteSheet.Direction.LeftToRight, 50, false);
            player.animations.SetFrameAction("jump", 16, () =>
                {
                    player.animations.currentAnimation = "moveforwards";
                    player.animations.RunOneFrame();
                });
            player.animations.SetFrameAction("jump", 8, player.StartJump);
            player.Ready(graphics);

            background = new Background(new Sprite(Content.Load<Texture2D>("background-tile-left")), new Sprite(Content.Load<Texture2D>("background-tile-right")));
            foreground = new Background(new Sprite(Content.Load<Texture2D>("foreground-tile-left")), new Sprite(Content.Load<Texture2D>("foreground-tile-right")));
            colorSheet = new BackgroundColor(graphics);
            colorSheet.AddColor("forward", new Color(204, 150, 41));
            colorSheet.AddColor("backward", new Color(204, 110, 41));

            Texture2D enemySpriteSheet = Content.Load<Texture2D>("Enemy-A-hover");
            enemyHandler = new EnemyHandler(graphics, new SpriteSheetInfo(225, 225), new List<Texture2D>(new Texture2D[]{enemySpriteSheet}), mainGameTime);

            rhythmHandler.beatStartSound = new Sound(Content.Load<SoundEffect>("startsound"));
            rhythmHandler.beatSound = new Sound(Content.Load<SoundEffect>("beatsound"));
            List<Sprite> collisionSprites = new List<Sprite>();
            collisionSprites.Add(new Sprite(Content.Load<Texture2D>("collision-1")));
            collisionSprites.Add(new Sprite(Content.Load<Texture2D>("collision-2")));
            collisionSprites.Add(new Sprite(Content.Load<Texture2D>("collision-3")));
            rhythmHandler.LoadCollisionSprites(collisionSprites);
            rhythmHandler.arrow = new Sprite(Content.Load<Texture2D>("arrow"));

            boss = new Boss(graphics, new SpriteSheetInfo(380, 430), mainGameTime);
            boss.animations["hover"] = new SpriteSheet(Content.Load<Texture2D>("boss-attk"), new SpriteSheetInfo(380, 430), 6, 6, 1, SpriteSheet.Direction.LeftToRight, 50, true);
            boss.animations["damage"] = new SpriteSheet(Content.Load<Texture2D>("boss-damaged"), new SpriteSheetInfo(380, 430), 5, 5, 1, SpriteSheet.Direction.LeftToRight, 100, false);
            boss.animations["final"] = new SpriteSheet(Content.Load<Texture2D>("boss-3"), new SpriteSheetInfo(380, 430), 5, 5, 1, SpriteSheet.Direction.LeftToRight, 100, true);
            boss.animations.SetFrameAction("damage", 4, () =>
                {
                    boss.animations.currentAnimation = "final";
                    boss.GenerateAttacks();
                });
            boss.Ready(graphics);

            SetUpGame();

            DebugText.spriteFont = Content.Load<SpriteFont>("DebugFont");
            timeSinceLastButtonText = new TextItem(DebugText.spriteFont);
            rhythmTimeText = new TextItem(DebugText.spriteFont);
            offBeatText = new TextItem(DebugText.spriteFont);
            winText = new TextItem(DebugText.spriteFont);
            winText.text = "You Win!";
            DebugText.debugTexts.Add(timeSinceLastButtonText);
            DebugText.debugTexts.Add(rhythmTimeText);
            DebugText.debugTexts.Add(offBeatText);
        }

        void SetUpGame()
        {
            player.SetUpPlayer();
            player.animations.active = false;
            player.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - (player.tex.Height / 2 - 27));
            player.endDashPos = player.pos;
            colorSheet.drawRect = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            enemyHandler.enemiesAlive = 0;
            enemyHandler.enemyPositions.Clear();
            enemyHandler.SetUpEnemies();
            rhythmHandler.SetUpRhythmHandler();
            rhythmHandler.arrow.visible = false;
            boss.SetUpBoss();
        }

        void SetUpStates()
        {
            world.AddGameState("game1", graphics);
            world.AddMenuState("mainMenu", graphics, this);
            world.menuStates["mainMenu"].menuFont = Content.Load<SpriteFont>("DebugFont");
            world.menuStates["mainMenu"].menuDirection = MenuState.Direction.TopToBottom;
            world.menuStates["mainMenu"].initialPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                graphics.GraphicsDevice.Viewport.Height - graphics.GraphicsDevice.Viewport.Height / 3);
            world.menuStates["mainMenu"].spacing = 10;
            world.AddMenuState("helpMenu", graphics, this);
            world.menuStates["helpMenu"].menuFont = Content.Load<SpriteFont>("DebugFont");
            world.menuStates["helpMenu"].menuDirection = MenuState.Direction.TopToBottom;
            world.menuStates["helpMenu"].initialPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                graphics.GraphicsDevice.Viewport.Height - graphics.GraphicsDevice.Viewport.Height / 4);
            world.AddMenuState("pauseMenu", graphics, this);
            world.menuStates["pauseMenu"].menuFont = Content.Load<SpriteFont>("DebugFont");
            world.menuStates["pauseMenu"].menuDirection = MenuState.Direction.TopToBottom;
            world.menuStates["pauseMenu"].initialPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                graphics.GraphicsDevice.Viewport.Height / 2);
            world.menuStates["pauseMenu"].spacing = 10;
            world.ActivateMenuState("mainMenu");

            world.menuStates["mainMenu"].AddMenuItem("Play");
            world.menuStates["mainMenu"].SetMenuAction("Play", () =>
            {
                world.ClearStates();
                SetUpGame();
                world.ActivateGameState("game1");
            });
            world.menuStates["mainMenu"].AddMenuItem("Help");
            world.menuStates["mainMenu"].SetMenuAction("Help", () =>
            {
                world.ClearStates();
                world.ActivateMenuState("helpMenu");
            });
            world.menuStates["mainMenu"].AddMenuItem("Exit");
            world.menuStates["mainMenu"].SetMenuAction("Exit", () =>
            {
                this.Exit();
            });

            world.menuStates["helpMenu"].AddMenuItem("Back");
            world.menuStates["helpMenu"].SetMenuAction("Back", () =>
                {
                    world.ClearStates();
                    world.ActivateMenuState("mainMenu");
                });

            world.menuStates["pauseMenu"].AddMenuItem("Resume");
            world.menuStates["pauseMenu"].SetMenuAction("Resume", () =>
                {
                    world.ClearStates();
                    world.ActivateGameState("game1");
                });
            world.menuStates["pauseMenu"].AddMenuItem("Back To Main Menu");
            world.menuStates["pauseMenu"].SetMenuAction("Back To Main Menu", () =>
            {
                world.ClearStates();
                world.ActivateMenuState("mainMenu");
            });
            world.menuStates["pauseMenu"].AddMenuItem("Exit");
            world.menuStates["pauseMenu"].SetMenuAction("Exit", () =>
                {
                    this.Exit();
                });

            world.gameStates["game1"].AddTime(mainGameTime);
            world.gameStates["game1"].AddDraw(MainDraw);
            world.gameStates["game1"].camera1.focus = Camera.Focus.Center;
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
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState[Keys.Escape] == KeyState.Down && previousKeyboardState[Keys.Escape] == KeyState.Up ||
                keyboardState[Keys.Back] == KeyState.Down && previousKeyboardState[Keys.Back] == KeyState.Up ||
                gamePadState.Buttons.Back == ButtonState.Pressed && previousGamePadState.Buttons.Back == ButtonState.Released)
            {
                World.thingsToDo.Add(() =>
                    {
                        world.ClearStates();
                        world.ActivateMenuState("pauseMenu");
                    });
            }

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
            
            timeSinceLastButtonText.text = "Measure Time: " + rhythmHandler.timeSincePressed.TotalMilliseconds.ToString();
            rhythmTimeText.text = "Current Beat: " + rhythmHandler.rhythmTime.TotalMilliseconds.ToString();
            offBeatText.text = "Offby: " + rhythmHandler.offBeatTime.TotalMilliseconds.ToString();
            enemyHandler.Update(gameTime, graphics, player);
            player.Update(gameTime, graphics);
            background.Update(world.gameStates["game1"].camera1);
            foreground.Update(world.gameStates["game1"].camera1);
            rhythmHandler.Update(gameTime, player, enemyHandler, boss);
            if (enemyHandler.enemiesKilled == 10)
            {
                boss.Spawn(player);
            }
            boss.Update(gameTime, graphics);
            colorSheet.pos = new Vector2(player.pos.X - graphics.GraphicsDevice.Viewport.Width / 2, 0);
            colorSheetAlpha.Update(gameTime);
            colorSheet.alpha = colorSheetAlpha.Value;
            colorSheet.Update(gameTime, graphics);
            world.gameStates["game1"].camera1.pan.Value = new Vector2(player.pos.X, graphics.GraphicsDevice.Viewport.Height / 2);
            DebugText.pos = new Vector2(world.gameStates["game1"].camera1.pan.Value.X - graphics.GraphicsDevice.Viewport.Width / 2,
                world.gameStates["game1"].camera1.pan.Value.Y - graphics.GraphicsDevice.Viewport.Height / 2);
            previousGamePadState = gamePadState;
            previousKeyboardState = keyboardState;
            world.gameStates["game1"].UpdateCurrentCamera(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            world.DrawWorld();
            base.Draw(gameTime);
        }

        void MainDraw()
        {
            world.BeginDraw();
            world.Draw(background.Draw);
            world.Draw(colorSheet.DrawRect);
            world.Draw(foreground.Draw);
            world.Draw(enemyHandler.Draw);
            world.Draw(boss.Draw);
            world.Draw(player.Draw);
            world.Draw(rhythmHandler.Draw);
            world.Draw(DebugText.Draw);
            world.EndDraw();
        }
    }
}
