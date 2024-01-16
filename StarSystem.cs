
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwoManSky
{
    class StarSystem
    {
        public bool starExists = false;

        // Features of star
        public StarClass starClass;
        public uint starDiameter;
        public uint starVariation;
        public uint numPlanets;
        public Planet[] planets;

        private uint nLehmer = 0;

        public StarSystem(int x, int y, bool generateFullSystem = false)
        {
            nLehmer = (uint)((x & 0xFFFF) << 16 | (y & 0xFFFF));
            starExists = (rndInt(0, 20) == 1);

            if(!starExists)
                return;

            GenerateStar();
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, AssetLoader assetLoader, Vector2 drawPosition, float scale)
        {
            Texture2D texture = assetLoader.starTexture[(int)starClass, starVariation];

            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture,
                             drawPosition,
                             null,
                             Color.White,
                             0f,
                             Origin,
                             (float)starDiameter / 100.0f * scale,
                             SpriteEffects.None,
                             0);
        }

        public void DrawHighlight(SpriteBatch spriteBatch, AssetLoader assetLoader, Vector2 drawPosition, float scale)
        {
            Texture2D texture = assetLoader.starHighlightTexture;

            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture,
                             drawPosition,
                             null,
                             Color.White,
                             0f,
                             Origin,
                             (float)starDiameter / 70.0f * scale,
                             SpriteEffects.None,
                             0);
        }

        

        // Generate features of star
        private void GenerateStar()
        {
            // We generate stars based on the Harvard spectral classification.
            // Class O and B will be collapsed into a single class.
            // Class A and F will be collapsed into a single class.

            // Determine attributes of star based on its stellar classification
            uint probability = rndInt(0, 10001);
            if(probability <= 7645)
            {
                starClass = StarClass.M;
                starDiameter = rndInt(20, 36);
            }
            else if(probability <= 8855)
            {
                starClass = StarClass.K;
                starDiameter = rndInt(31, 41);
            }
            else if(probability <= 9615)
            {
                starClass = StarClass.G;
                starDiameter = rndInt(41, 51);
            }
            else if(probability <= 9985)
            {
                starClass = StarClass.AF;
                starDiameter = rndInt(51, 71);
            }
            else
            {
                starClass = StarClass.OB;
                starDiameter = rndInt(75, 101);
            }

            // Generate attributes independent of stellar classification
            // ---------------------------------------------------------

            // Variation of star sprite
            starVariation = rndInt(0, 4);

            // Generate planets
            numPlanets = rndInt(0, 6);
            planets = new Planet[numPlanets];
            for(int i = 0; i < numPlanets; ++i)
            {
                // Planet type
                uint planetType = rndInt(0, 6);
                uint planetSize = 0;
                uint planetDifficulty = 0;

                switch(planetType)
                {
                    // Gas giant
                    case 0:
                        planetSize = rndInt(80, 101);
                        break;

                    // Ice giant
                    case 1:
                        planetSize = rndInt(60, 76);
                        break;

                    // Desert
                    case 2:
                        planetSize = rndInt(20, 36);
                        planetDifficulty = rndInt(2, 4);
                        break;

                    // Barren
                    case 3:
                        planetSize = rndInt(10, 41);
                        break;

                    // Volcanic
                    case 4:
                        planetSize = rndInt(10, 21);
                        planetDifficulty = rndInt(3, 5);
                        break;

                    // Terran
                    case 5:
                        planetSize = rndInt(20, 36);
                        planetDifficulty = rndInt(1, 3);
                        break;
                }

                planets[i] = new Planet(planetType, planetSize, planetDifficulty);
            }
        }

        private uint Lehmer32()
        {
            nLehmer += 0xE120FC15;
            UInt64 tmp = nLehmer * 0x4A39B70D;
            UInt32 m1 = (UInt32)((tmp >> 32) ^ tmp);
            tmp = ((UInt64)m1 * 0x12FAD5C9);
            UInt32 m2 = (UInt32)((tmp >> 32) ^ tmp);
            return m2;
        }

        private uint rndInt(int min, int max)
        {
            return (uint)((Lehmer32() % (max - min)) + min);
        }
    }

    enum StarClass
    {
        OB,
        AF,
        G,
        K,
        M
    }
}