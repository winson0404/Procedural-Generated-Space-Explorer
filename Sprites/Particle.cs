//
// Using reference from
// http://rbwhitaker.wikidot.com/2d-particle-engine-1
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwoManSky.Sprites
{
    public class Particle : Sprite
    {
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public Color Color { get; set; }
        public float Lifespan { get; set; }
        public float Opacity { get; set; } = 1f;
        public float OpacityChange { get; set; } = 0f;
        public float Size { get; set; } = 1f;
        public float SizeChange { get; set; } = 0f;
        private Texture2D Texture;


        public Particle(Texture2D Texture, Vector2 Position, Vector2 Velocity, float Angle, float AngularVelocity,
                        Color Color, float Lifespan)
        {
            setTexture(Texture);
            this.Texture = getTexture();
            this.Position = Position;
            this.Velocity = Velocity;
            this.Angle = Angle;
            this.AngularVelocity = AngularVelocity;
            this.Color = Color;
            this.Lifespan = Lifespan;
        }

        public Particle(Texture2D Texture, Vector2 Position, Vector2 Velocity, float Angle, float AngularVelocity,
                        Color Color, float Lifespan, float Opacity, float OpacityChange, float Size, float SizeChange)
        {
            setTexture(Texture);
            this.Texture = getTexture();
            this.Position = Position;
            this.Velocity = Velocity;
            this.Angle = Angle;
            this.AngularVelocity = AngularVelocity;
            this.Color = Color;
            this.Lifespan = Lifespan;
            this.Opacity = Opacity;
            this.OpacityChange = OpacityChange;
            this.Size = Size;
            this.SizeChange = SizeChange;
            this.IsAlive = true;
        }

        public override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * totalSeconds;
            Angle += AngularVelocity * totalSeconds;
            Opacity += OpacityChange * totalSeconds;
            Size += SizeChange * totalSeconds;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            Lifespan -= totalSeconds;

            if(Lifespan <= 0)
                this.IsAlive = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color * Opacity, Angle, Origin,
                                Size, SpriteEffects.None, 0f);
        }
    }
}