

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwoManSky.Sprites
{
    public class ExplosionEffect : ParticleEmitter
    {
        private int timer = 0;

        public ExplosionEffect(Vector2 Position, int Count, List<Texture2D> Textures)
        {
            this.Position = Position;
            this.Count = Count;

            this.Textures = Textures;
            this.Particles = new LinkedList<Particle>();
        }

        protected override void CheckDeleteEmitter()
        {
            if(this.timer >= 5)
            {
                this.IsAlive = false;
                return;
            }
            timer++;
        }

        protected override Particle GenerateNewParticle()
        {
            Texture2D texture = Textures[Game2.Random.Next(Textures.Count)];
            Vector2 position = this.Position;
            Vector2 velocity = new Vector2(60f * (float)(Game2.Random.NextDouble() * 2 - 1),
                                           60f * (float)(Game2.Random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 6f * (float)(Game2.Random.NextDouble() * 2 - 1);
            // Color color = new Color(255, Game2.Random.Next(100, 256), 0);
            Color color = Color.DarkGray;
            float lifespan = 0.4f + (float)Game2.Random.NextDouble() * 0.8f;
            float opacity = 0.3f;
            float opacityChange = -0.3f;
            float size = (float)Game2.Random.NextDouble() * 2f;
            float sizeChange = 0f;

            return new Particle(texture, position, velocity, angle, angularVelocity,
                                color, lifespan, opacity, opacityChange, size, sizeChange);
        }
    }
}