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
    class Mine : Bullet
    {
        // Common attributes for all PaintBall instances
        private const double m_SPEED = 100.0; // 300 pixels per second
        private const double _radius = 250;
        private float _damage = 10;
        private float _distance;
        private Boolean _targetDetected = false;
        private Vector2 _secondaryFire;
        private Texture2D Texture;

        public Mine()
        {
            this.Initialize();
        }

        public Mine(Vector2 position, Vector2 heading)
            : base(position, heading)
        {
            this.Initialize();
        }

        public Mine(Mine rhs)
            : this(rhs.Position, rhs.Heading)
        {
            this.Initialize();
        }

        protected override void Initialize()
        {
            setTexture(Game2.textures["mine"]);
            Texture = getTexture();
            _damage *= damage_factor;
        }

        public override void Update(GameTime gameTime)
        {
            Movement(gameTime);

            // check if hit enemy
            foreach (Sprite enemy in Game2.GetEnemies()) {
                if (this.Intersects(enemy))
                    this.OnCollide(enemy);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                float rotation = Vector2ToRadian(Heading);
                //float rotation = Vector2ToRadian(Heading);
                spriteBatch.Draw(Texture, Position, null, Color.White, rotation, Origin, 0.5f, SpriteEffects.None, 1f);
            }
        }

        public Mine Copy()
        {
            Mine copy = new Mine(this);
            return copy;
        }

        // with reference to KinematicArrive and KinematicSeek in Lab03 
        private void Movement(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            double livingTime = totalSeconds - SpawnTime.TotalSeconds;
            List<Sprite> enemies = Game2.GetEnemies();
            foreach (Sprite enemy in enemies)
            {
                Vector2 enemyPosition = enemy.Position;

                // get distance between mouse cursor and bullet
                _distance = (enemyPosition - Position).Length();

                // check if distance within radius and if its already homing to the cursor
                if (livingTime > 1 && (Math.Abs(_distance) < _radius || _targetDetected == true))
                {
                    if (!_targetDetected)
                    {
                        _secondaryFire = enemyPosition - Position;
                    }
                    _secondaryFire.Normalize();
                    Position += _secondaryFire * (float)((m_SPEED * 2) * elapsedSeconds);
                    _targetDetected = true;
                }
                else if (Math.Abs(_distance) >= _radius)
                {
                    Position += (Heading * (float)(m_SPEED * elapsedSeconds)) / (float)Math.Pow((totalSeconds - SpawnTime.TotalSeconds + 1), 2);
                }

                //kill the bullet if go out of screen
                if (Position.X < 0 || Position.X >= Game2.Screen.ClientBounds.Width ||
                    Position.Y < 0 || Position.Y >= Game2.Screen.ClientBounds.Height ||
                    livingTime > 5.0) // if its living longer than 5 sec than die || if it hits the cursor than die
                {
                    IsAlive = false;
                }
            }
        }

        public void set_damage_factor(float damage_factor)
        {
            this.damage_factor = damage_factor;
            _damage = 10;
            _damage *= damage_factor;

        }

        public override void OnCollide(Sprite sprite)
        {
            Debug.WriteLine("Hitted Enemy with damage of " + _damage);
            sprite.Health -= _damage;
            this.IsAlive = false;
        }
    }
}
