using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace molnprojektet
{
    class DeathFactory
    {
        private Sprite FactorySprite = new Sprite();
        private static Texture2D FactoryTexture;

        private double nextCloudTime;
        private TimeSpan cloudTimer = new TimeSpan();

        private const double CLOUD_SPAWN_AVERAGE = 3;
        private const double CLOUD_SPAWN_DEVIATION = 1;
        
        // Reference for cloud spawning
        private GameWindow game;

        /// <summary>
        /// Factory that spews out poison!
        /// </summary>
        /// <param name="position"></param>
        /// <param name="game"></param>
        public DeathFactory(Vector2 position, GameWindow game)
        {
            FactorySprite.Initialize();
            FactorySprite.Texture = FactoryTexture;
            FactorySprite.Position = position;

            this.game = game;

            nextCloudTime = GetNextCloudTime();
        }

        public static void LoadContent()
        {
            FactoryTexture = Game1.contentManager.Load<Texture2D>(@"Images/Factory_1");
        }

        private double GetNextCloudTime()
        {
            return CLOUD_SPAWN_AVERAGE + (Shared.Random.NextDouble() - Shared.Random.NextDouble()) * CLOUD_SPAWN_DEVIATION;
        }

        public void Update(GameTime gameTime)
        {
            cloudTimer.Add(gameTime.ElapsedGameTime);
            if (cloudTimer.TotalSeconds > nextCloudTime)
            {
                cloudTimer.Subtract(TimeSpan.FromSeconds(nextCloudTime));
                nextCloudTime = GetNextCloudTime();

                Vector2 poisonPosition = FactorySprite.Position;
                poisonPosition.X += FactoryTexture.Width / 2;

                game.AddPoisonCloud(poisonPosition);
            }
        }

        public void draw(GraphicsHandler g, GameTime gameTime)
        {
            g.DrawSprite(FactorySprite);
        }
    }
}
