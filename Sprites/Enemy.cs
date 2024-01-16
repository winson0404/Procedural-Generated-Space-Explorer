//
// Using reference from
// http://rbwhitaker.wikidot.com/2d-particle-engine-1
//

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwoManSky.Sprites
{
    class Enemy : Sprite, ICloneable
    {
        // Texture
        private TimeSpan flipTime;
        private TimeSpan rotateTime;
        private TimeSpan driftTime;
        private double driftTime_difference = 1;
        private float maxOrientation;
        private SpriteEffects s;
        private float speed;
        private float enemyRotation; 
        private double _lastShot;
        private double _firingRate;
        private EnemyNormalShots enemyNormalShots;
        private const double _radius = 400;
        private float bonus_damage = 0.25f;
        private Color color = Color.White;

        Vector2 drift_velocity = new Vector2(0, 0);
        private Texture2D Texture;
        public Rectangle GetRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
        public Enemy()
        {
            this.Initialize();
        }

        protected override void Initialize()
        {
            setTexture(Game2.textures["EnemyShip"]);
            Texture = getTexture();
            speed = 100f;
            maxOrientation = 180;
            Level = Game2.enemyLevel;
            enemyNormalShots = new EnemyNormalShots();
            _lastShot = 0;
            _firingRate = 1.2;

            flipTime = TimeSpan.Zero;

            rotateTime = TimeSpan.Zero;

            driftTime = TimeSpan.Zero;

            Position = new Vector2(Game2.Random.Next(Texture.Width / 2 + (int)(Game2.Screen.ClientBounds.Width * 0.2), Game2.Screen.ClientBounds.Width - Texture.Width / 2),
                                   Game2.Random.Next(Texture.Height / 2, Game2.Screen.ClientBounds.Height - Texture.Height / 2));

            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);

            if (Game2.Random.Next(0, 100) % 2 == 0)
            {
                s = SpriteEffects.FlipHorizontally;
            }

        }

        private float GetOrientation(Vector2 heading)
        {
            return (float)Math.Atan2(heading.Y, heading.X);
        }

        public override void Update(GameTime gameTime)
        {
            if (Game2.gameState == Game2.CombatState.NEUTRAL)
                WanderState(gameTime);
            else if (Game2.gameState == Game2.CombatState.HOSTILE)
                HostileState(gameTime);
            else if (Game2.gameState == Game2.CombatState.PEACE)
                SurrenderState(gameTime);


            // check if still alive
            if (Health <= 0)
            {
                IsAlive = false;
            }
        }

        private void WanderState(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            color = Color.LightGreen;
            //every 3 seconds, have 1/3 chance to flip
            if (gameTime.TotalGameTime.TotalSeconds - flipTime.TotalSeconds > 2)
            {
                flipTime = gameTime.TotalGameTime;

                if (Game2.Random.Next(1, 100) % 3 == 0)
                {
                    s = SpriteEffects.None;
                }
                else if (Game2.Random.Next(1, 100) % 3 == 0)
                {
                    s = SpriteEffects.FlipHorizontally;
                }
            }


            if (gameTime.TotalGameTime.TotalSeconds - rotateTime.TotalSeconds > 1)
            {
                rotateTime = gameTime.TotalGameTime;
                // calculate the random direction
                //NOTE: random.NextDouble() * (maximum - minimum) + minimum;  for random between (-1. 1)
                float maximum = 1f;
                float minimum = -1f;
                enemyRotation = 1.1f * (GetOrientation(Heading) + MathHelper.ToRadians(maxOrientation * ((float)Game2.Random.NextDouble() * (maximum - minimum) + minimum) * elapsedSeconds));

                // random rotation
                //System.Diagnostics.Debug.WriteLine(MathHelper.ToDegrees(enemyRotation));
                this.Heading = new Vector2((float)Math.Cos(enemyRotation),
                                        (float)Math.Sin(enemyRotation));
            }


            //moves according to the direction heading
            if (s == SpriteEffects.FlipHorizontally)
                this.Position -= this.Heading * speed * elapsedSeconds;
            else
                this.Position += this.Heading * speed * elapsedSeconds;

            // make enemy turn back if out of screen
            if (Position.X < 0 || Position.X >= Game2.Screen.ClientBounds.Width ||
                Position.Y < 0 || Position.Y >= Game2.Screen.ClientBounds.Height)
            {
                this.Heading = -Heading;
            }
        }

        private void HostileState(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            color = Color.Red;
            //when shots fire, will make enemy all points toward the player
            Vector2 TurnVec = Game2.GetPlayer().Position - Position;
            float distance = Math.Abs((Game2.GetPlayer().Position - Position).Length());
            TurnVec.Normalize();
            if (s == SpriteEffects.FlipHorizontally)
            {
                this.Heading = -TurnVec;
                if (distance > _radius)
                    this.Position -= this.Heading * speed * elapsedSeconds;

            }
            else
            {
                this.Heading = TurnVec;
                if (distance > _radius)
                    this.Position += this.Heading * speed * elapsedSeconds;
            }


            float drift_speed = (float)(50 * elapsedSeconds);
            float drift_distance = 1f;
            float maximum = 2f;
            float minimum = 0.5f;

            if (gameTime.TotalGameTime.TotalSeconds - driftTime.TotalSeconds > driftTime_difference)
            {
                driftTime = gameTime.TotalGameTime;
                driftTime_difference = Game2.Random.NextDouble() * (maximum - minimum);
                drift_velocity = new Vector2(0, 0);

                if (Game2.Random.Next(1, 100) % 2 == 0) {
                    this.fireEnemyNormalShots(gameTime);
                }

                //set certain chance for the plane to drift in different direction
                if (Game2.Random.Next(1, 100) % 7 == 0) // Up
                    drift_velocity += new Vector2(0f, -drift_distance);
                else if (Game2.Random.Next(1, 100) % 7 == 0) // Down
                    drift_velocity += new Vector2(0f, drift_distance);
                if (Game2.Random.Next(1, 100) % 2 == 0) // Left
                    drift_velocity += new Vector2(-drift_distance, 0f);
                else if (Game2.Random.Next(1, 100) % 2 == 0) // Right
                    drift_velocity += new Vector2(drift_distance, 0f);
            }

            Position += drift_velocity * drift_speed;

        }
        private void SurrenderState(GameTime gameTime)
        {
            color = Color.White;
            Health = 10000;
            Position += new Vector2(0, 0);

        }
        private void fireEnemyNormalShots(GameTime gameTime)
        {
            double newTime = gameTime.TotalGameTime.TotalSeconds;
            double time_diffrence = newTime - _lastShot;
            double secondsPerBullet = 1.0 / _firingRate;
            // Valid fire (within firing rate)
            if (time_diffrence >= secondsPerBullet)
            {
                // System.Diagnostics.Debug.WriteLine("Here");
                _lastShot = newTime;
                EnemyNormalShots bulletInstance = enemyNormalShots.Copy();
                bulletInstance.Position = Position + Heading;
                bulletInstance.Heading = Game2.GetPlayer().Position - Position;
                bulletInstance.Heading.Normalize();
                bulletInstance.SpawnTime = gameTime.TotalGameTime;
                bulletInstance.set_damage_factor(1 + bonus_damage * (Level - 1));
                // Add the bullet to the world
                Game2.sprites.AddLast(bulletInstance);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float rotation = Vector2ToRadian(Heading);
            spriteBatch.Draw(Texture, Position, null, color, rotation, Origin, 1f, s, 1f);
        }
    }
}
