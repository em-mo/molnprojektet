using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace molnprojektet
{
    class Window
    {
        private GraphicsHandler graphicsHandler;

        public GraphicsHandler GetGraphicsHandler
        {
            get { return graphicsHandler; }
        }


        public virtual void Initialize(SpriteBatch batch)
        {
            graphicsHandler = new GraphicsHandler();
            graphicsHandler.Initialize(batch);
        }

        public virtual void Update()
        {

        }

        public void Draw()
        {
            //graphicsHandler.DrawSprites();
            graphicsHandler.DrawWithAdjustedSize();
        }

        public void AddSpriteToHandler(Sprite sprite)
        {
            graphicsHandler.AddSprite(sprite);
        }
    }
}
