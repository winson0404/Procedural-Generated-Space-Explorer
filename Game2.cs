using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using TwoManSky.Sprites;
using System.Diagnostics;
using System.Windows;

namespace TwoManSky
{
    public class Game2 : Game
    {
        private GraphicsDeviceManager _graphics;
        public static GameWindow Screen;
        private SpriteBatch _spriteBatch;
        public static LinkedList<Sprite> sprites;
        public static Dictionary<string, Texture2D> textures;
        public static Random Random = new Random();
        public enum CombatState { NEUTRAL, HOSTILE, PEACE};
        public static CombatState gameState = CombatState.NEUTRAL;
        private int _enemyCount;
        public static int ScreenWidth = 1600;
        public static int ScreenHeight = 900;
        public static int enemyLevel = 1;
        public static float enemy_confidence_index = 0;
        private Texture2D background;
        private SpriteFont font;

        public Game2(int difficulty)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Screen = this.Window;
            enemyLevel = difficulty;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            _enemyCount = Game2.Random.Next(6, 12);


            // Make mouse visible
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            sprites = new LinkedList<Sprite>();
            textures = new Dictionary<string, Texture2D>();

            //load font
            font = Content.Load<SpriteFont>("details");

            //initalize textures
            background = Content.Load<Texture2D>("planet_background");
            textures.Add("spaceship", Content.Load<Texture2D>("spaceship"));
            textures.Add("cursor", Content.Load<Texture2D>("cursor"));
            textures.Add("missile_harpoon", Content.Load<Texture2D>("missile_harpoon"));
            textures.Add("red_glow", Content.Load<Texture2D>("red_glow"));
            textures.Add("shrapnel_glow", Content.Load<Texture2D>("shrapnel_glow"));
            textures.Add("bullet", Content.Load<Texture2D>("bullet"));
            textures.Add("EnemyShip", Content.Load<Texture2D>("EnemyShip"));
            textures.Add("mine", Content.Load<Texture2D>("mine"));

            // Add particles to particle systems
            textures.Add("smoke01", Content.Load<Texture2D>("smoke01"));
            textures.Add("smoke02", Content.Load<Texture2D>("smoke02"));
            textures.Add("smoke03", Content.Load<Texture2D>("smoke03"));
            textures.Add("smoke04", Content.Load<Texture2D>("smoke04"));

            //initialize cursor
            sprites.AddLast(new Cursor());

            //spawn player
            sprites.AddLast(new Ship());

            //spawn enemy
            for (int i = 0; i < _enemyCount; i++)
            {
                sprites.AddLast(new Enemy());
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            LinkedListNode<Sprite> node = sprites.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value.IsAlive)
                    node.Value.Update(gameTime);
                node = next;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            PostUpdate(gameTime);
            base.Update(gameTime);
        }

        //remove dead sprites
        private void PostUpdate(GameTime gameTime)
        {
            if (enemy_confidence_index == 0) {
                enemy_confidence_index = (enemyLevel - GetPlayer().Level) * (-0.18f);
            }
            LinkedListNode<Sprite> node = sprites.First;
            while (node != null)
            {
                var next = node.Next;
                if (!node.Value.IsAlive || node.Value.Health <=0)
                {
                    if (node.Value.GetType() == typeof(Enemy))
                    {
                        enemy_confidence_index += 0.1f;
                        GetPlayer().LevelUp();
                        GetPlayer().Health += 10;
                    }
                    if (node.Value.GetType() == typeof(Ship))
                    {
                        Debug.WriteLine("Player Death, " + node.Value.Health);
                        node = next;
                        continue;
                    }
                    sprites.Remove(node);
                }
                node = next;
            }

            // check if change State
            if ((int)(gameTime.TotalGameTime.TotalSeconds % 3) < 1 && enemy_confidence_index >= 0.54)
            {
                //trigger with the chance of 55% (example)
                if (Random.Next(1, 100) > Game2.enemy_confidence_index * 100)
                {
                    Game2.gameState = Game2.CombatState.PEACE;
                }

            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20, 255));

            _spriteBatch.Begin();


            _spriteBatch.DrawString(font, "Health: " + GetPlayer().Health + "/100", new Vector2(100, 100), Color.White);
            _spriteBatch.DrawString(font, "Level: " + GetPlayer().Level+ "/4", new Vector2(100, 120), Color.White);
            _spriteBatch.DrawString(font, "Exp: " + GetPlayer().Exp+ "/100", new Vector2(100, 140), Color.White);
            _spriteBatch.DrawString(font, "Fire Mode: " + GetPlayer()._fire_mode, new Vector2(100, 160), Color.White);
            _spriteBatch.Draw(background,
                              new Vector2(0.0f, 0.0f),
                              null,
                              Color.White * 0.4f,
                              0f,
                              new Vector2(0.0f, 0.0f),
                              2,
                              SpriteEffects.None,
                              0);
            foreach (var sprite in sprites)
                sprite.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public static List<Sprite> GetEnemies()
        {
            List<Sprite> enemies = new List<Sprite>();
            foreach (Sprite obj in sprites)
                if (obj.GetType() == typeof(Enemy) && obj.IsAlive)
                    enemies.Add(obj);

            return enemies;
        }

        public static Sprite GetPlayer() {
            Sprite temp = new Sprite();
            foreach (Sprite obj in sprites)
                if (obj.GetType() == typeof(Ship) && obj.IsAlive)
                {
                    temp = obj;
                    break;
                }
            return temp;
        }
    }
}
