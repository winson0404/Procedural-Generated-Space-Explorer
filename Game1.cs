using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwoManSky
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static ContentManager MyContent;
        public static Random random = new Random();
        public static AssetLoader assetLoader;

        // Window dimensions
        public static int ScreenWidth = 1600;
        public static int ScreenHeight = 900;
        public static int SectorSize = 64;

        // Mouse and Keyboard
        MouseState mouse;
        KeyboardState keyboard;
        private int sectorMouseX;
        private int sectorMouseY;
        private MouseState lastMouseState;

        // Hyperspace movement stats
        private float hyperspaceSpeed = 10.0f;

        // Player ship
        PlayerShip playerShip;
        private uint shipDirection = 6;

        // Galaxy position
        private Vector2 GalaxyOffset;
        private double shipOffsetX;
        private double shipOffsetY;
        private int galaxyMouseX;
        private int galaxyMouseY;
        Vector2 screenSector;

        // Star menu
        StarMenu starMenu;
        private bool starSelected = false;
        private StarSystem selectedStar;
        private bool starMenuButtonPressed = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Set window dimensions
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            // Set content manager
            MyContent = Content;

            // Asset Loader
            assetLoader = new AssetLoader(Content);

            // Star Menu
            starMenu = new StarMenu();

            playerShip = new PlayerShip(assetLoader);
            GalaxyOffset = new Vector2();
            lastMouseState = Mouse.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Keyboard input
            // --------------
            keyboard = Keyboard.GetState();

            if(keyboard.IsKeyDown(Keys.W))
            {
                if(keyboard.IsKeyDown(Keys.D))
                {
                    GalaxyOffset.Y -= 0.71f * hyperspaceSpeed * elapsedSeconds;
                    GalaxyOffset.X += 0.71f * hyperspaceSpeed * elapsedSeconds;
                    shipDirection = 7;
                }
                else if(keyboard.IsKeyDown(Keys.A))
                {
                    GalaxyOffset.Y -= 0.71f * hyperspaceSpeed * elapsedSeconds;
                    GalaxyOffset.X -= 0.71f * hyperspaceSpeed * elapsedSeconds;
                    shipDirection = 5;
                }
                else if(keyboard.IsKeyUp(Keys.S))
                {
                    GalaxyOffset.Y -= hyperspaceSpeed * elapsedSeconds;
                    shipDirection = 6;
                }
            }
            else if(keyboard.IsKeyDown(Keys.S))
            {
                if(keyboard.IsKeyDown(Keys.D))
                {
                    GalaxyOffset.Y += 0.71f * hyperspaceSpeed * elapsedSeconds;
                    GalaxyOffset.X += 0.71f * hyperspaceSpeed * elapsedSeconds;
                    shipDirection = 1;
                }
                else if(keyboard.IsKeyDown(Keys.A))
                {
                    GalaxyOffset.Y += 0.71f * hyperspaceSpeed * elapsedSeconds;
                    GalaxyOffset.X -= 0.71f * hyperspaceSpeed * elapsedSeconds;
                    shipDirection = 3;
                }
                else
                {
                    GalaxyOffset.Y += hyperspaceSpeed * elapsedSeconds;
                    shipDirection = 2;
                }
            }
            else if(keyboard.IsKeyDown(Keys.D))
            {
                GalaxyOffset.X += hyperspaceSpeed * elapsedSeconds;
                shipDirection = 0;
            }
            else if(keyboard.IsKeyDown(Keys.A))
            {
                GalaxyOffset.X -= hyperspaceSpeed * elapsedSeconds;
                shipDirection = 4;
            }

            // Current offset from a sector
            shipOffsetX = GalaxyOffset.X - Math.Truncate(GalaxyOffset.X);
            shipOffsetY = GalaxyOffset.Y - Math.Truncate(GalaxyOffset.Y);

            // Clicking on a star
            // ------------------
            mouse = Mouse.GetState();

            // Get mouse clicking position in galaxy
            sectorMouseX = (mouse.X + SectorSize / 2 + (int)(shipOffsetX * SectorSize)) / SectorSize;
            sectorMouseY = (mouse.Y + SectorSize / 2 + (int)(shipOffsetY * SectorSize)) / SectorSize;
            galaxyMouseX = sectorMouseX + (int)GalaxyOffset.X;
            galaxyMouseY = sectorMouseY + (int)GalaxyOffset.Y;

            if(mouse.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                if(starSelected)
                {
                    if(mouse.X < 850 && mouse.Y > 450)
                    {
                        // Clicking on a planet in star menu
                        starMenu.Update(gameTime, selectedStar);
                    }
                    else
                    {
                        starSelected = false;
                    }
                }
                else
                {
                    selectedStar = new StarSystem(galaxyMouseX, galaxyMouseY);
                    starSelected = selectedStar.starExists;
                    starMenuButtonPressed = true;
                }
            }

            if(starMenuButtonPressed && mouse.LeftButton != ButtonState.Pressed)
            {
                starMenuButtonPressed = false;
            }

            lastMouseState = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20, 255));

            // TODO: Add your drawing code here

            // Number of sectors on screen
            int numSectorsX = ScreenWidth / SectorSize;
            int numsectorsY = ScreenHeight / SectorSize;
            screenSector = new Vector2(0, 0);

            _spriteBatch.Begin();

            // Draw space background
            // ---------------------
            _spriteBatch.Draw(assetLoader.spaceBackgroundTexture,
                              new Vector2(0.0f, 0.0f),
                              null,
                              Color.White * 0.5f,
                              0f,
                              new Vector2(0.0f, 0.0f),
                              1,
                              SpriteEffects.None,
                              0);

            // Draw stars on screen
            // --------------------
            for(screenSector.X = -1; screenSector.X <= numSectorsX; ++screenSector.X)
            {
                for(screenSector.Y = -1; screenSector.Y <= numsectorsY; ++screenSector.Y)
                {
                    StarSystem star = new StarSystem((int)screenSector.X + (int)GalaxyOffset.X,
                                                     (int)screenSector.Y + (int)GalaxyOffset.Y);

                    if(star.starExists)
                    {
                        mouse = Mouse.GetState();

                        if(sectorMouseX == screenSector.X && sectorMouseY == screenSector.Y)
                        {
                            star.DrawHighlight(_spriteBatch, assetLoader, 
                                               new Vector2(screenSector.X * SectorSize - (float)shipOffsetX * SectorSize,
                                                           screenSector.Y * SectorSize - (float)shipOffsetY * SectorSize),
                                               1.0f);
                        }
                        
                        star.Draw(_spriteBatch, assetLoader, 
                                  new Vector2(screenSector.X * SectorSize - (float)shipOffsetX * SectorSize,
                                              screenSector.Y * SectorSize - (float)shipOffsetY * SectorSize),
                                  1.0f);
                    }
                }
            }

            // Draw player ship
            // ----------------
            playerShip.Draw(_spriteBatch, shipDirection);

            // Draw star menu
            // --------------
            if(starSelected)
            {
                starMenu.Draw(_spriteBatch, assetLoader, selectedStar);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
