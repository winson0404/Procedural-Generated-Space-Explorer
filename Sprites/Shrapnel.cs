using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwoManSky.Sprites
{
    // Sabot Shrapnel
    class Shrapnel : Bullet
    {
        private float _damage = 1;
        private float opacity = 0.7f;
        private TimeSpan shrapnel_lifetime;
        public TimeSpan shrapnel_spawn;
        public float speed_multiplier;

        private double shrapnel_speed = 850.0;

        public Shrapnel()
        {
            setTexture(Game2.textures["shrapnel_glow"]);
            shrapnel_lifetime = TimeSpan.FromMilliseconds(Game2.Random.Next(250, 311));
            speed_multiplier = (float)Game2.Random.NextDouble() * 0.6f + 0.7f;
            _damage *= damage_factor;
        }

        public void set_damage_factor(float damage_factor)
        {
            this.damage_factor = damage_factor;
            _damage = 1;
            _damage *= damage_factor;

        }

        public override void Update(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime - shrapnel_spawn) * speed_multiplier >= shrapnel_lifetime)
            {
                this.opacity -= 0.05f;
                this.shrapnel_speed *= 0.9f;

                if(opacity <= 0)
                {
                    this.IsAlive = false;
                }
            }

            this.Position = this.Position + this.Heading
                          * (float)(shrapnel_speed * gameTime.ElapsedGameTime.TotalSeconds)
                          * speed_multiplier;


            foreach (Sprite enemy in Game2.GetEnemies())
            {
                if (this.Intersects(enemy))
                    this.OnCollide(enemy);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(IsAlive)
                spriteBatch.Draw(getTexture(), Position, null, Color.White * opacity, Vector2ToRadian(Heading), Origin, 1f, SpriteEffects.None, 1f);
        }

        public void SetShrapnelSpawn(GameTime gameTime)
        {
            this.shrapnel_spawn = gameTime.TotalGameTime;
        }

        public Bullet Copy()
        {
            Shrapnel copy = new Shrapnel();
            copy.setTexture(this.getTexture());
            return copy;
        }
        public override void OnCollide(Sprite sprite)
        {
            Debug.WriteLine("Hitted Enemy with damage of " + _damage);
            sprite.Health -= _damage;
            this.IsAlive = false;
        }
    }
}