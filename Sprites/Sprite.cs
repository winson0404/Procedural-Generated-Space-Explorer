using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoManSky.Sprites
{
    public class Sprite : ICloneable
    {
        protected KeyboardState _currentKey;
        protected KeyboardState _previousKey;

        public float Health = 100f;
        public bool IsAlive = true;
        public float _rotation;
        private Texture2D Texture;
        public Vector2 Position;
        public Vector2 Origin;
        public Vector2 Heading;
        public float RotationVelocity = 3f;
        public float LinearVelocity = 4f;
        public Sprite Parent;
        public TimeSpan SpawnTime;
        public Color[] TextureData;
        public string _fire_mode = "1";
        //attributes
        public int Level = 1;
        public double Exp = 0;
        

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                  Matrix.CreateRotationZ(_rotation) *
                  Matrix.CreateTranslation(new Vector3(Position, 0));
            }
        }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, Texture.Width, Texture.Height);
            }
        }

        public Sprite() { }

        public void setTexture(Texture2D texture)
        {
            Texture = texture;
            Position = Vector2.Zero;
            //Heading = new Vector2(1f, 0f);
            TextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);

            // The default origin in the centre of the sprite
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

        }

        public Texture2D getTexture() {
            return Texture;
        }

        protected virtual void Initialize()
        {

        }


        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _rotation = Vector2ToRadian(Heading);
            spriteBatch.Draw(Texture, Position, null, Color.White, _rotation, Origin, 1f, SpriteEffects.None, 0);
        }
        public bool Intersects(Sprite sprite)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            var transformAToB = this.Transform * Matrix.Invert(sprite.Transform);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            var stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            var stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            var yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < this.Rectangle.Height; yA++)
            {
                // Start at the beginning of the row
                var posInB = yPosInB;

                for (int xA = 0; xA < this.Rectangle.Width; xA++)
                {
                    // Round to the nearest pixel
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < sprite.Rectangle.Width &&
                        0 <= yB && yB < sprite.Rectangle.Height)
                    {
                        // Get the colors of the overlapping pixels
                        var colourA = this.TextureData[xA + yA * this.Rectangle.Width];
                        var colourB = sprite.TextureData[xB + yB * sprite.Rectangle.Width];

                        // If both pixel are not completely transparent
                        if (colourA.A != 0 && colourB.A != 0)
                        {
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        public virtual void OnCollide(Sprite sprite)
        {

        }

        public virtual void LevelUp() { }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        protected float Vector2ToRadian(Vector2 heading)
        {
            // Calculate rotation relative to Vector(1, 0)
            return (float)Math.Atan2(heading.Y, heading.X);
        }
    }
}
