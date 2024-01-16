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
    class Ship : Sprite
    {
        private MouseState _lastMouseState = new MouseState();
        private double _speed;
        private double _lastShot;
        private double _firingRate;
        // bloodstorm variables
        private double m_firingRate = 1.2; // 1.2 volleys per second
        private double m_lastShot = 0;
        private float m_gunPoint = 5.0f; // the distance away from the origin
        private int missile_drift = 1;
        private bool _missile_firing = false;
        private TimeSpan _missile_delay = TimeSpan.FromMilliseconds(50);
        private TimeSpan _last_missile = new TimeSpan();
        private int _current_missile = 0;
        private int _total_missiles = 14;
        private Texture2D Texture;
        private float bonus_damage = 0.25f; 
        private SpriteFont font;

        // Bullet Types
        private Mine _mine;
        private NormalShots _normal_shot;
        private BloodStorm _blood_storm;

        public Ship()
        {
            setTexture(Game2.textures["spaceship"]);
            Texture = getTexture();
            Health = 100;
            Position = new Vector2((int)(Game2.Screen.ClientBounds.Width * 0.1), Game2.Screen.ClientBounds.Height / 2.0f);
            Origin = new Vector2(Texture.Width / 3f, Texture.Height / 2.0f);
            _lastMouseState = new MouseState();
            _speed = 200.0;
            _lastShot = 0;
            _firingRate = 1.2;
            _fire_mode = "1";
            //Initialize new bullets
            _mine = new Mine();
            _normal_shot = new NormalShots();
            _blood_storm = new BloodStorm();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            // Calculate rotation
            Heading = mouse.Position.ToVector2() - Position;
            if (Heading.LengthSquared() > 0)
                Heading.Normalize();
            _rotation = Vector2ToRadian(Heading);

            ////change fire mode
            getFireMode(keyboard);

            if (_missile_firing) {
                fireBloodStorms(mouse, gameTime);
            }
            ////handle all ship attack move
            if (_fire_mode != "3" && mouse.LeftButton == ButtonState.Pressed) {
                if (Game2.gameState != Game2.CombatState.PEACE)
                    Game2.gameState = Game2.CombatState.HOSTILE;
                Attack(mouse, gameTime);
            }
            else if (_fire_mode == "3" && mouse.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released)
            {
                if (Game2.gameState != Game2.CombatState.PEACE)
                    Game2.gameState = Game2.CombatState.HOSTILE;
                double newTime = gameTime.TotalGameTime.TotalSeconds;
                double dt = newTime - m_lastShot;
                double secondsPerBullet = 1.0 / m_firingRate;
                // Valid fire (within firing rate)
                if (dt >= secondsPerBullet)
                {
                    // TODO : Spawn many missiles per click
                    _missile_firing = true;
                    _current_missile = 0;

                }
            }

            //handle ship movements
            ShipMovement(keyboard, gameTime.ElapsedGameTime.TotalSeconds);
            _lastMouseState = mouse;
        }
        
        public override void LevelUp()
        {
            Exp += 20;
            if (Exp == 100 && Level <= 4) {
                Level += 1;
                Exp = 0;
                Debug.WriteLine("lvl up");
            }
        }

        private void ShipMovement(KeyboardState keyboard, double elapsedSeconds)
        {
            // movement direction
            float distance = (float)(_speed * elapsedSeconds);

            // Keyboard Input
            if (keyboard.IsKeyDown(Keys.W)) // Up
                Position = Position + new Vector2(0f, -1f) * distance;
            if (keyboard.IsKeyDown(Keys.S)) // Down
                Position = Position + new Vector2(0f, 1f) * distance;
            if (keyboard.IsKeyDown(Keys.A)) // Left
                Position = Position + new Vector2(-1f, 0f) * distance;
            if (keyboard.IsKeyDown(Keys.D)) // Right
                Position = Position + new Vector2(1f, 0f) * distance;
        }

        private void getFireMode(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.D1))
            {
                _fire_mode = "1";
            }
            else if (keyboard.IsKeyDown(Keys.D2))
            {
                _fire_mode = "2";
            }
            else if (keyboard.IsKeyDown(Keys.D3))
            {
                _fire_mode = "3";
            }
        }

        private void Attack(MouseState mouse, GameTime gameTime)
        {
            if (_fire_mode == "1")
            {
                fireNormalShots(mouse, gameTime);
            }
            if (_fire_mode == "2")
            {
                fireMines(mouse, gameTime);
            }
            if (_fire_mode == "3")
            {
                fireBloodStorms(mouse, gameTime);
            }
        }

        private void fireNormalShots(MouseState mouse, GameTime gameTime)
        {
            double newTime = gameTime.TotalGameTime.TotalSeconds;
            double time_diffrence = newTime - _lastShot;
            double secondsPerBullet = 1.0 / _firingRate;
            // Valid fire (within firing rate)
            if (time_diffrence >= secondsPerBullet)
            {
                // System.Diagnostics.Debug.WriteLine("Here");
                _lastShot = newTime;
                NormalShots bulletInstance = _normal_shot.Copy();
                bulletInstance.Position = Position + Heading;
                bulletInstance.Heading = mouse.Position.ToVector2() - Position;
                bulletInstance.Heading.Normalize();
                bulletInstance.set_damage_factor(1 + bonus_damage * (Level - 1));
                bulletInstance.SpawnTime = gameTime.TotalGameTime;
                // Add the bullet to the world
                Game2.sprites.AddLast(bulletInstance);
            }
        }

        private void fireMines(MouseState mouse, GameTime gameTime)
        {
            double newTime = gameTime.TotalGameTime.TotalSeconds;
            double dt = newTime - _lastShot;
            double secondsPerBullet = 1.0 / _firingRate;
            // Valid fire (within firing rate)
            if (dt >= secondsPerBullet)
            {
                // System.Diagnostics.Debug.WriteLine("Here");
                for (int i = -45; i <= 45; i += 15)
                {
                    _lastShot = newTime;
                    Mine bulletInstance = _mine.Copy();
                    bulletInstance.Position = Position - Heading;
                    bulletInstance.Heading = -mouse.Position.ToVector2() + Position;
                    // use the direction of player +/- i degree
                    double theta = MathHelper.ToRadians(i) + (float)Math.Atan2(bulletInstance.Heading.Y, bulletInstance.Heading.X);
                    float X_offset = (float)Math.Cos(theta);
                    float Y_offset = (float)Math.Sin(theta);
                    bulletInstance.Heading = new Vector2(X_offset, Y_offset);
                    bulletInstance.Heading.Normalize();
                    bulletInstance.SpawnTime = gameTime.TotalGameTime;
                    bulletInstance.set_damage_factor(1 + bonus_damage * (Level - 1));
                    // Add the bullet to the world
                    Game2.sprites.AddLast(bulletInstance);
                }
            }
        }

        private void fireBloodStorms(MouseState mouse, GameTime gameTime)
        {
            // Missile still firing
                double newTime = gameTime.TotalGameTime.TotalSeconds;
                if (gameTime.TotalGameTime - _last_missile >= _missile_delay)
                {
                    _current_missile++;
                    if (_current_missile >= _total_missiles)
                    {
                        _missile_firing = false;
                    }

                    // TODO : Optimize using radians
                    m_lastShot = newTime;
                    BloodStorm bulletInstance = _blood_storm.Copy();
                    bulletInstance.Position = Position + Heading * m_gunPoint;
                    bulletInstance.Heading = mouse.Position.ToVector2() - Position;
                    bulletInstance.Heading.Normalize();

                    // Make missile shot angles a bit spread out
                    bulletInstance.Heading.X += ((float)Game2.Random.NextDouble() - 0.5f) * 1.2f;
                    bulletInstance.Heading.Y += ((float)Game2.Random.NextDouble() - 0.5f) * 1.2f;
                    bulletInstance.Heading.Normalize();

                    bulletInstance.SetFireTime(gameTime);
                    bulletInstance.SetMissileDrift(missile_drift);
                    bulletInstance.Parent = this;
                    bulletInstance.damage_factor += 1 + bonus_damage * (Level - 1);

                missile_drift ^= 1;
                    _last_missile = gameTime.TotalGameTime;

                    // Add the bullet to the world
                    Game2.sprites.AddLast(bulletInstance);
                }
        }

    }
}
