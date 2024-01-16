




using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwoManSky
{
    class PlayerShip
    {
        // Texture
        private Texture2D texture;

        public PlayerShip(AssetLoader assetLoader)
        {
            texture = assetLoader.playerShipTexture;
        }

        public void Draw(SpriteBatch spriteBatch, uint direction)
        {
            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            float rotation = direction * 45.0f * ((float)Math.PI / 180.0f);

            spriteBatch.Draw(texture,
                             new Vector2(800, 450),
                             null,
                             Color.White,
                             rotation,
                             Origin,
                             0.5f,
                             SpriteEffects.None,
                             0);
        }
    }
}