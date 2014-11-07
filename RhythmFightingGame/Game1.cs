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
        SpriteBatch spriteBatch;
        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        Player player;
        World world;
        GameTimeWrapper mainGameTime;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1700;
            graphics.PreferredBackBufferHeight = 900;
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player = new Player(Content.Load<Texture2D>("Robanimation"));
            player.pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, 500);
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

            if (gamePadState.Triggers.Left >= 0.5 && previousGamePadState.Triggers.Left < 0.5)
            {
                player.Dash(Player.Facing.Left);
            }
            if (gamePadState.Triggers.Right >= 0.5 && previousGamePadState.Triggers.Right < 0.5)
            {
                player.Dash(Player.Facing.Right);
            }

            player.Update(gameTime, graphics.GraphicsDevice);
            previousGamePadState = gamePadState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
