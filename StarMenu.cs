

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwoManSky
{
    class StarMenu
    {
        public void Update(GameTime gameTime, StarSystem selectedStar)
        {
            MouseState mouse = Mouse.GetState();

            Vector2 planetMenuPositions = new Vector2(Game1.ScreenWidth / 7f + 100f, Game1.ScreenHeight / 1.35f);
            Planet[] planets = selectedStar.planets;

            if(mouse.LeftButton == ButtonState.Pressed)
            {
                for(int i = 0; i < planets.Length; ++i)
                {
                    if(planets[i].planetDifficulty > 0)
                    {
                        // Console.WriteLine(EuclideanDistance(new Vector2(mouse.X, mouse.Y), planetMenuPositions));
                        if(EuclideanDistance(new Vector2(mouse.X, mouse.Y), planetMenuPositions) <= planets[i].planetSize)
                        {
                            // WINSON : TRANSITION HERE
                            using (var game2 = new Game2((int)planets[i].planetDifficulty))
                                game2.Run();
                        }
                    }
                    planetMenuPositions.X += 100f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, AssetLoader assetLoader, StarSystem selectedStar)
        {
            spriteBatch.Draw(assetLoader.starMenuTexture,
                             new Vector2(Game1.ScreenWidth / 3.8f, Game1.ScreenHeight / 1.35f),
                             null,
                             Color.White,
                             0f,
                             assetLoader.GetOrigin(assetLoader.starMenuTexture),
                             1,
                             SpriteEffects.None,
                             0);
        
            selectedStar.Draw(spriteBatch, assetLoader, new Vector2(Game1.ScreenWidth / 9f, Game1.ScreenHeight / 1.35f), 2.0f);

            DrawPlanets(spriteBatch, assetLoader, selectedStar, new Vector2(Game1.ScreenWidth / 7f, Game1.ScreenHeight / 1.35f), 1.0f);
        }

        public void DrawPlanets(SpriteBatch spriteBatch, AssetLoader assetLoader, StarSystem selectedStar, Vector2 drawPosition, float scale)
        {
            Planet[] planets = selectedStar.planets;

            for(int i = 0; i < planets.Length; ++i)
            {
                Texture2D texture = assetLoader.planetTexture[(int)planets[i].planetType];
                Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

                drawPosition.X += 100;

                spriteBatch.Draw(texture,
                             drawPosition,
                             null,
                             Color.White,
                             0f,
                             Origin,
                             (0.4f + 0.5f * ((float)planets[i].planetSize / 100.0f)) * scale,
                             SpriteEffects.None,
                             0);
                
                if(planets[i].planetDifficulty > 0)
                {
                    DrawPlanetDifficulty(spriteBatch, assetLoader, selectedStar, drawPosition, scale, i);
                }
            }
        }

        public void DrawPlanetDifficulty(SpriteBatch spriteBatch, AssetLoader assetLoader, StarSystem selectedStar, Vector2 drawPosition, float scale, int planetNum)
        {
            Texture2D texture = assetLoader.planetDifficultyTexture;
            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Planet[] planets = selectedStar.planets;

            uint difficulty = planets[planetNum].planetDifficulty;
            Color difficultyColor = Color.White;

            switch(difficulty)
            {
                case 1:
                    difficultyColor = Color.White;
                    break;
                case 2:
                    difficultyColor = Color.Yellow;
                    break;
                case 3:
                    difficultyColor = Color.Orange;
                    break;
                case 4:
                    difficultyColor = Color.Red;
                    break;
            }

            drawPosition.Y += planets[planetNum].planetSize * 1.2f;

            drawPosition.X -= 7.5f * (difficulty - 1);

            for(int j = 0; j < difficulty; ++j)
            {
                spriteBatch.Draw(texture,
                                 drawPosition,
                                 null,
                                 difficultyColor,
                                 0f,
                                 Origin,
                                 0.3f,
                                 SpriteEffects.None,
                                 0);
                
                drawPosition.X += 15;
            }
        }

        private float EuclideanDistance(Vector2 vec1, Vector2 vec2)
        {
            return (float)Math.Sqrt(Math.Pow(vec1.X - vec2.X, 2) +
                                    Math.Pow(vec1.Y - vec2.Y, 2));
        }
    }
}