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
    public class ParticleEmitter : Sprite
    {
        public int Count { get; set; }

        protected List<Texture2D> Textures;
        protected LinkedList<Particle> Particles;

        protected int ParticleID = 0;

        public ParticleEmitter()
        {
        }

        public ParticleEmitter(Vector2 Position, int Count, List<Texture2D> Textures, Sprite Parent)
        {
            this.Position = Position;
            this.Count = Count;

            this.Textures = Textures;
            this.Particles = new LinkedList<Particle>();

            this.Parent = Parent;
            this.IsAlive = true;
        }

        public override void Update(GameTime gameTime)
        {
            CheckDeleteEmitter();

            // Emit particles
            for (int i = 0; i < Count; ++i)
            {
                Particle particle = GenerateNewParticle();
                particle.IsAlive = true;
                Game2.sprites.AddLast(particle);
            }
        }

        protected virtual void CheckDeleteEmitter()
        {
            if (this.Parent.IsAlive == false)
            {
                this.IsAlive = false;
                return;
            }

            this.Position = Parent.Position + Parent.Heading * -15f;
        }

        // Generate new particle from emitter
        protected virtual Particle GenerateNewParticle()
        {
            Texture2D texture = Textures[Game2.Random.Next(Textures.Count)];
            Vector2 position = this.Position;
            Vector2 velocity = new Vector2(60f * (float)(Game2.Random.NextDouble() * 2 - 1),
                                           60f * (float)(Game2.Random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 6f * (float)(Game2.Random.NextDouble() * 2 - 1);
            Color color = Color.White;
            float lifespan = 0.4f + (float)Game2.Random.NextDouble() * 0.6f;
            float opacity = 0.3f;
            float opacityChange = -0.4f;
            float size = (float)Game2.Random.NextDouble();
            float sizeChange = 0f;

            return new Particle(texture, position, velocity, angle, angularVelocity,
                                color, lifespan, opacity, opacityChange, size, sizeChange);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }


    }


}