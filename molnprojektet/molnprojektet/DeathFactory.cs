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
        
        // Reference for cloud spawning
        private GameWindow game;

        public DeathFactory(Vector2 position, GameWindow game)
        {
            FactorySprite.Initialize();
            FactorySprite.Texture = FactoryTexture;
            FactorySprite.Position = position;

            this.game = game;
        }

        public static void LoadContent()
        {
            FactoryTexture = Game1.contentManager.Load<Texture2D>(@"Images/Factory_1");
        }

        public void Update(GameTime gameTime)
        {

        }

        public void draw(GraphicsHandler g, GameTime gameTime)
        {
            g.DrawSprite(FactorySprite);
        }
    }
}
