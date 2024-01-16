using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwoManSky.Sprites
{
    public class Cursor : Sprite
    {
    private Texture2D Texture;

        public Cursor()
        {
            setTexture(Game2.textures["cursor"]);
            Texture = getTexture();
            Position = Mouse.GetState().Position.ToVector2();
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {
            Position = Mouse.GetState().Position.ToVector2();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 1f);
        }
    }
}
