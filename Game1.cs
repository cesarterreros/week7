using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LoopyTeddies
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WindowWidth = 800;
        const int WindowHeight = 600;

        const int InitialNumTeddies = 1;

        // random teddy bear support
        Random rand = new Random();
        List<Texture2D> sprites = new List<Texture2D>();

        // spawning support
        const int TotalSpawnDelayMilliseconds = 1000;
        int elapsedSpawnDelayMilliseconds = 0;
        // saved for efficiency
        Texture2D explosionSprite;

        // game objects
        List<TeddyBear> bears = new List<TeddyBear>();
        List<Explosion> explosions = new List<Explosion>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // load sprites for efficiency
            sprites.Add(Content.Load<Texture2D>(@"graphics\teddybear0"));
            sprites.Add(Content.Load<Texture2D>(@"graphics\teddybear1"));
            sprites.Add(Content.Load<Texture2D>(@"graphics\teddybear2"));
            explosionSprite = Content.Load<Texture2D>(@"graphics\explosion");

            // create initial game objects
            for (int i = 0; i < InitialNumTeddies; i++)
            {
                bears.Add(GetRandomTeddyBear());
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // get user input
            MouseState mouse = Mouse.GetState();

            // spawn teddies as appropriate
            elapsedSpawnDelayMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedSpawnDelayMilliseconds >= TotalSpawnDelayMilliseconds)
            {
                elapsedSpawnDelayMilliseconds = 0;
                bears.Add(GetRandomTeddyBear());
            }

            // update and blow up teddy bears
            foreach (TeddyBear teddyBear in bears)
            {
                teddyBear.Update(gameTime);
                if (teddyBear.CollisionRectangle.Contains(
                    mouse.X, mouse.Y))
                {
                    teddyBear.Active = false;
                    explosions.Add(new Explosion(explosionSprite,
                        teddyBear.CollisionRectangle.Center.X,
                        teddyBear.CollisionRectangle.Center.Y));
                }
            }

            // update explosions
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            // remove dead teddies
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                if (!bears[i].Active)
                {
                    bears.RemoveAt(i);
                }
            }

            // remove dead explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (!explosions[i].Active)
                {
                    explosions.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw game entities
            spriteBatch.Begin();
            foreach (TeddyBear teddyBear in bears)
            {
                teddyBear.Draw(spriteBatch);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Gets a random teddy bear
        /// </summary>
        /// <returns>random teddy bear</returns>
        private TeddyBear GetRandomTeddyBear()
        {
            Texture2D sprite = sprites[rand.Next(3)];
            return new TeddyBear(sprite,
                rand.Next(WindowWidth - sprite.Width),
                rand.Next(WindowHeight - sprite.Height),
                WindowWidth, WindowHeight);
        }
    }
}
