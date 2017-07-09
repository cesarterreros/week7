using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TeddyMineExplosion;

namespace ProgrammingAssignment5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Mine support
        Texture2D mineSprite;
        List<Mine> mines = new List<Mine>();

        //Teddy support
        Texture2D teddySprite;

        //Explosion support
        Texture2D explosionSprite;

        //Click processing
        bool leftClickStarted = false;
        bool leftButtonReleased = true;

        //Random teddy bear support
        Random rand = new Random();

        //Spawning support
        int[] DelayMilliseconds = new[] { 1000, 2000, 3000 } ;
        int TotalSpawnDelayMilliseconds;
        int elapsedSpawnDelayMilliseconds = 0;

        //Game objects
        List<TeddyBear> bears = new List<TeddyBear>();
        List<Explosion> explosions = new List<Explosion>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = GameConstants.WindowWidth;
            graphics.PreferredBackBufferHeight = GameConstants.WindowHeight;
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
            
            mineSprite = Content.Load<Texture2D>(@"graphics/mine");
            teddySprite = Content.Load<Texture2D>(@"graphics/teddybear");
            explosionSprite = Content.Load<Texture2D>(@"graphics/explosion");

            TotalSpawnDelayMilliseconds = DelayMilliseconds[rand.Next(0, 3)];
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
            
            //Get current mouse state
            MouseState mouse = Mouse.GetState();

            //spawn teddies as appropriate
            elapsedSpawnDelayMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedSpawnDelayMilliseconds >= TotalSpawnDelayMilliseconds)
            {
                elapsedSpawnDelayMilliseconds = 0;
                TotalSpawnDelayMilliseconds = DelayMilliseconds[rand.Next(0,3)];
                bears.Add(new TeddyBear(teddySprite, GenerateRandomVelocity(), GameConstants.WindowWidth, 
                    GameConstants.WindowHeight));
            }

            foreach (var teddyBear in bears)
            {
                teddyBear.Update(gameTime);

                foreach (var mine in mines)
                {
                    if (teddyBear.CollisionRectangle.Contains(
                        mine.CollisionRectangle.Center.X, mine.CollisionRectangle.Center.Y))
                    {
                        teddyBear.Active = false;
                        mine.Active = false;

                        explosions.Add(new Explosion(explosionSprite,
                            teddyBear.CollisionRectangle.Center.X,
                            teddyBear.CollisionRectangle.Center.Y));
                    }
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

            // remove dead mines
            for (int i = mines.Count - 1; i >= 0; i--)
            {
                if (!mines[i].Active)
                {
                    mines.RemoveAt(i);
                }
            }

            // remove dead explosions
            //for (int i = explosions.Count - 1; i >= 0; i--)
            //{
            //    if (!explosions[i].Active)
            //    {
            //        explosions.RemoveAt(i);
            //    }
            //}

            if (mouse.LeftButton == ButtonState.Pressed && leftButtonReleased)
            {
                leftClickStarted = true;
                leftButtonReleased = false;
            }
            else if(mouse.LeftButton == ButtonState.Released)
            {
                leftButtonReleased = true;

                //if left click finished
                if (leftClickStarted)
                {
                    leftClickStarted = false;
                    mines.Add(new Mine(mineSprite, mouse.X, mouse.Y));
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

            spriteBatch.Begin();
            foreach (var mine in mines)
            {
                mine.Draw(spriteBatch);
            }

            foreach (var teddy in bears)
            {
                teddy.Draw(spriteBatch);
            }

            foreach (var explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Vector2 GenerateRandomVelocity()
        {
            var velocityX = 0f;
            while (velocityX == 0)
            {
                velocityX = (float)(rand.NextDouble() * (.4 + .5) - .5);
            }

            var velocityY = 0f;
            while (velocityY == 0)
            {
                velocityY = (float)(rand.NextDouble() * (.4 + .5) - .5);
            }

            return new Vector2(velocityX, velocityY);
        }
    }
}
