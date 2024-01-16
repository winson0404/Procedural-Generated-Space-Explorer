using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwoManSky.Sprites
{
    class BloodStorm : Bullet
    {
        // Common attributes for all SelwynKessler instances
        private static Texture2D m_texture;
        private const string m_texture_name = "missile_harpoon";
        private const double m_acceleration = 10;
        private const double max_speed = 300.0;
        private float turn_speed = 0.02f;
        private TimeSpan launch_delay = TimeSpan.FromSeconds(1);
        private TimeSpan missile_lifetime = TimeSpan.FromSeconds(8);
        private float jitter_strength = 0.2f;
        private TimeSpan jitter_delay = TimeSpan.FromMilliseconds(300);
        private TimeSpan last_jitter;

        // Attributes for this specific missile
        private double m_SPEED = 0.0; 
        private TimeSpan fire_time;
        private int missile_drift;

        // Drift on stage one
        private float drift_speed = (float)Game2.Random.NextDouble() * 150f;
        private float random_drift = (float)Game2.Random.NextDouble() * 0.5f;

        // Initialize thrusters
        private bool thrusters_on = false;

        // Trails particles
        ParticleEmitter smokeTrails;
        List<Texture2D> smokeTextures;
        private int smokeParticlesCount = 0;

        // Sabot shots
        Shrapnel shrapnel;
        private double sabot_trigger_distance = 120;
        private Texture2D Texture;

        // Explosion effect
        ExplosionEffect explosionEffect;

        public BloodStorm()
        {
            this.Initialize();
        }

        public BloodStorm(Vector2 position, Vector2 heading)
            : base(position, heading)
        {
            this.Initialize();
        }

        public BloodStorm(BloodStorm rhs)
            : this(rhs.Position, rhs.Heading)
        {
            this.Initialize();
        }

        protected override void Initialize()
        {
            setTexture(Game2.textures[m_texture_name]);
            Texture = getTexture();
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);

            // Set trails particles
            smokeTextures = new List<Texture2D>();
            smokeTextures.Add(Game2.textures["smoke01"]);
            smokeTextures.Add(Game2.textures["smoke02"]);
            smokeTextures.Add(Game2.textures["smoke03"]);
            smokeTextures.Add(Game2.textures["smoke04"]);
            smokeTrails = new ParticleEmitter(this.Position + (this.Heading * -10f), 5, smokeTextures, this);

            // Set shrapnels
            shrapnel = new Shrapnel();

            // Set Explosion effect
            explosionEffect = new ExplosionEffect(this.Position, 5, smokeTextures);

        }

        // Set time of shot
        public void SetFireTime(GameTime gameTime)
        {
            this.fire_time = gameTime.TotalGameTime;
        }

        public void SetMissileDrift(int right)
        {
            missile_drift = right;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            if(gameTime.TotalGameTime - fire_time > missile_lifetime)
            {
                Explode();
            }

            // Select missile stage
            if(gameTime.TotalGameTime - fire_time > launch_delay)
            {
                this.SecondStage(mouse, gameTime);
            }
            else
            {
                this.FirstStage(gameTime);
            }

            Position = Position + Heading * (float)(m_SPEED * elapsedSeconds);
        }

        

        private void FirstStage(GameTime gameTime)
        {
            // Missile drift to sides
            float drift_radian = Vector2ToRadian(this.Heading);
            drift_radian =  drift_radian + (missile_drift * (float)Math.PI) + random_drift;
            this.Position += new Vector2((float)Math.Cos(drift_radian - Math.PI / 2), (float)Math.Sin(drift_radian - Math.PI / 2))
                            * drift_speed
                            * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Drift deceleration
            drift_speed *= 0.97f;
        }

        private void SecondStage(MouseState mouse, GameTime gameTime)
        {
            // Missile acceleration
            m_SPEED += m_acceleration;
            m_SPEED = Math.Min(m_SPEED, max_speed);

            // Guided missile turning algorithm
            Vector2 TurnVec = mouse.Position.ToVector2() - Position;
            TurnVec.Normalize();
            this.Heading += TurnVec * turn_speed;

            // Missile jitter
            if(gameTime.TotalGameTime - last_jitter >= jitter_delay)
            {
                Vector2 jitter = new Vector2((float)Game2.Random.NextDouble() - 0.5f,
                                             (float)Game2.Random.NextDouble() - 0.5f);
                this.Heading += jitter * jitter_strength;
                last_jitter = gameTime.TotalGameTime;
            }
            this.Heading.Normalize();

            // Sabot burst
            Vector2 turn_difference_vector = TurnVec - Heading;
            Double turn_difference_rad = Math.Abs(turn_difference_vector.X) + Math.Abs(turn_difference_vector.Y) ;

            List <Sprite> enemies = Game2.GetEnemies();
            foreach(Sprite enemy in enemies)
            {
                if(DistanceToTarget(enemy) <= sabot_trigger_distance &&turn_difference_rad <=0.3)
                {
                    this.SabotBurst(gameTime);
                    this.IsAlive = false;
                    break;
                }
            }

            // Rocket Thruster
            if(thrusters_on == false)
            {
                // Rocket Thrusters
                Rocket rocket = new Rocket();
                rocket.Parent = this;
                rocket.Position = this.Position + this.Heading * -10f;
                thrusters_on = true;
                last_jitter = gameTime.TotalGameTime;
                Game2.sprites.AddLast(rocket);

                // Smoke Trail Emitter
                smokeTrails.IsAlive = true;
                Game2.sprites.AddLast(smokeTrails);
            }
        }

        // Calculate Euclidean distance to cursor
        private double DistanceToTarget(Sprite target)
        {
            return Math.Sqrt(Math.Pow(this.Position.X - target.Position.X, 2)
                           + Math.Pow(this.Position.Y - target.Position.Y, 2));
        }

        // Spawn sabot shrapnels
        private void SabotBurst(GameTime gameTime)
        {
            for(int i = 0; i < 21; ++i)
            {
                Shrapnel shrapnelInstance = (Shrapnel)shrapnel.Copy();
                shrapnelInstance.Position = Position;
                Double shrapnel_radian = Vector2ToRadian(this.Heading) - 0.5f + 0.05f * i;
                shrapnelInstance.Heading = new Vector2((float)Math.Cos(shrapnel_radian),
                                                       (float)Math.Sin(shrapnel_radian));
                shrapnelInstance.Heading.Normalize();
                shrapnelInstance.SetShrapnelSpawn(gameTime);
                shrapnelInstance.set_damage_factor(this.damage_factor);

                Game2.sprites.AddLast(shrapnelInstance);
            }
        }

        public void Explode()
        {
            explosionEffect.IsAlive = true;
            explosionEffect.Position = this.Position;
            Game2.sprites.AddLast(explosionEffect);
            this.IsAlive = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                float rotation = Vector2ToRadian(Heading);
                spriteBatch.Draw(Texture, Position, null, Color.White, rotation, Origin, 1.2f, SpriteEffects.None, 1f);
            }
        }

        public BloodStorm Copy()
        {
            BloodStorm copy = new BloodStorm(this);
            return copy;
        }
    }

    // Rocket Thruster
    class Rocket : Sprite
    {
        private Texture2D Texture;
        public Rocket(){
            this.Initialize();
        }

        protected override void Initialize()
        {
            setTexture(Game2.textures["red_glow"]);
            Texture = getTexture();
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
        }

        public override void Update(GameTime gameTime)
        {
            if(IsAlive == false)
            {
                Game2.sprites.Remove(this);
            }
            if(Parent.IsAlive == false)
            {
                this.IsAlive = false;
            }
            else
                this.Position = Parent.Position + Parent.Heading * -10f;            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Parent.IsAlive)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Origin, 0.05f, SpriteEffects.None, 1f);
            }
        }
    }
}
