using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace molnprojektet
{
    class GraphicsHandler
    {
        SpriteBatch batch;
        List<Sprite> spriteList;

        public void Initialize(SpriteBatch batch)
        {
            this.batch = batch;
            spriteList = new List<Sprite>();
        }

        public void AddSprite(Sprite sprite)
        {
            spriteList.Add(sprite);
        }

        public void FlushSprites()
        {
            batch.End();
        }

        public void DrawSprites()
        {
            foreach (Sprite sprite in spriteList)
	        {
                if(sprite.Size != Vector2.Zero && sprite.IsShowing)
                    batch.Draw(sprite.Texture, sprite.Position, new Rectangle((int)sprite.Position.X, (int)sprite.Position.Y, (int)sprite.Size.X, (int)sprite.Size.Y), sprite.Color, sprite.Rotation, sprite.Origin, sprite.Scale, sprite.Effects, sprite.Layer);
                else if(sprite.IsShowing)
                    batch.Draw(sprite.Texture, new Rectangle((int)sprite.Position.X, (int)sprite.Position.Y, (int)sprite.Size.X, (int)sprite.Size.Y), null, sprite.Color, sprite.Rotation, sprite.Scale, sprite.Effects, sprite.Layer);   
            }       
        }
    }
}
