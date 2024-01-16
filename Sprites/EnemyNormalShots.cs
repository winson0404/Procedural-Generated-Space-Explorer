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
    class EnemyNormalShots : Bullet
    {
        // Common attributes for all DotBullet instances
        private const double m_SPEED = 400;
        private float _damage = 10;

        private float m_rotation;
        private Texture2D Texture;

        public EnemyNormalShots()
        {
            this.Initialize();
        }

        public EnemyNormalShots(Vector2 position, Vector2 heading)
            : base(position, heading)
        {
            this.Initialize();
        }

        public EnemyNormalShots(EnemyNormalShots rhs)
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
            _damage = 10;
            _damage *= damage_factor;

        }
        public override void Update(GameTime gameTime)
        {
            Position += Heading * (float)(m_SPEED * gameTime.ElapsedGameTime.TotalSeconds);

            // check if hit enemy
            if (this.Intersects(Game2.GetPlayer()))
                this.OnCollide(Game2.GetPlayer());
        }

        public EnemyNormalShots Copy()
        {
            EnemyNormalShots copy = new EnemyNormalShots(this);
            return copy;
        }

        public override void OnCollide(Sprite sprite)
        {
            //Debug.WriteLine("Hitted Player with damage of " + _damage);
            sprite.Health -= _damage;
            this.IsAlive = false;
        }
    }
}
