using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwoManSky.Sprites
{
    class NormalShots : Bullet
    {
        // Common attributes for all DotBullet instances
        private const double m_SPEED = 700;
        private float _damage = 100;

        private float m_rotation;
        private Texture2D Texture;

        public NormalShots()
        {
            this.Initialize();
        }

        public NormalShots(Vector2 position, Vector2 heading)
            : base(position, heading)
        {
            this.Initialize();
        }

        public NormalShots(NormalShots rhs)
            : this(rhs.Position, rhs.Heading)
        {
            this.Initialize();
        }



        // public override Rectangle Bound()
        // {
        //     Vector2 pos = Position - Origin;
        //     return new Rectangle((int)pos.X, (int)pos.Y, m_texture.Width, m_texture.Height);
        // }

        protected override void Initialize()
        {
            setTexture(Game2.textures["bullet"]);
            Texture = getTexture();
            _damage *= damage_factor;
        }

        public void set_damage_factor(float damage_factor)
        {
            this.damage_factor = damage_factor;
            _damage = 34;
            _damage *= damage_factor;

        }

        public override void Update(GameTime gameTime)
        {
            Position += Heading * (float)(m_SPEED * gameTime.ElapsedGameTime.TotalSeconds);

            // check if hit enemy
            foreach (Sprite enemy in Game2.GetEnemies())
            {
                if (this.Intersects(enemy))
                    this.OnCollide(enemy);
            }
        }

        public NormalShots Copy()
        {
            NormalShots copy = new NormalShots(this);
            return copy;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            _rotation = Vector2ToRadian(Heading);
            spriteBatch.Draw(Texture, Position, null, Color.GreenYellow, _rotation, Origin, 1f, SpriteEffects.None, 0);
        }
        public override void OnCollide(Sprite sprite)
        {
            //Debug.WriteLine("Hitted Enemy with damage of " + _damage);
            sprite.Health -= _damage;
            this.IsAlive = false;
        }
    }
}
