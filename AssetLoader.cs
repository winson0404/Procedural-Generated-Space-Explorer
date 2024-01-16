using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwoManSky
{
    public class AssetLoader
    {
        // Space background
        public Texture2D spaceBackgroundTexture;

        // Player ship
        public Texture2D playerShipTexture;

        // Stars
        public Texture2D[,] starTexture = new Texture2D[5,4];

        // Planets
        public Texture2D[] planetTexture = new Texture2D[6];

        // Planet Difficulty
        public Texture2D planetDifficultyTexture;

        // Star highlight
        public Texture2D starHighlightTexture;

        // Star menu
        public Texture2D starMenuTexture;

        public AssetLoader(ContentManager Content)
        {
            // Load space background
            spaceBackgroundTexture = Content.Load<Texture2D>("space_background");

            // Load player ship
            playerShipTexture = Content.Load<Texture2D>("player_ship");

            // Load stars
            string[] starColours = {"blue", "white", "yellow", "orange", "red"};
            for(int i = 0; i < 5; ++i)
            {
                for(int j = 0; j < 4; ++j)
                {
                    starTexture[i,j] = Content.Load<Texture2D>("star_" + starColours[i] + "0" + (j+1).ToString());
                }
            }

            // Load planets
            for(int i = 0; i < 6; ++i)
            {
                planetTexture[i] = Content.Load<Texture2D>("planet_0" + (i+1).ToString());
            }

            // Load planet difficulty
            planetDifficultyTexture = Content.Load<Texture2D>("skull");

            // Load star highlight
            starHighlightTexture = Content.Load<Texture2D>("star_highlight");

            // Load star menu
            starMenuTexture = Content.Load<Texture2D>("star_menu");
        }

        public Vector2 GetOrigin(Texture2D texture)
        {
            return new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}