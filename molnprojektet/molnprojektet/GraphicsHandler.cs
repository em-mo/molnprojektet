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
    /* Define methods for Drawing sprites, adding sprites to an internal array, and initializating all variables */
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

        #region Draw
        public void DrawSprites()
        {
            foreach (Sprite sprite in spriteList)
	        {
                if(sprite.IsShowing)
                    batch.Draw(sprite.Texture, sprite.Position, new Rectangle((int)sprite.Position.X, (int)sprite.Position.Y, (int)sprite.Size.X, (int)sprite.Size.Y), sprite.Color, sprite.Rotation, sprite.Origin, sprite.Scale, sprite.Effects, sprite.Layer);
	        }       
        }

        public void DrawWithAdjustedSize()
        {
            foreach (Sprite sprite in spriteList)
            {
                if (sprite.IsShowing)
                    batch.Draw(sprite.Texture, new Rectangle((int)sprite.Position.X, (int)sprite.Position.Y, (int)sprite.Size.X, (int)sprite.Size.Y), null, sprite.Color, sprite.Rotation, sprite.Scale, sprite.Effects, sprite.Layer);
             }
        }

        /*
        public virtual void Draw()
        {
            if (this.isVisable)
                this.spriteBatch.Draw(this.texture, this.position, this.source, this.color, this.rotation, this.origin, this.scale, this.effects, this.layer);
        }

        public virtual void Draw(Vector2 drawPosition)
        {
            if (this.isVisable)
                this.spriteBatch.Draw(this.texture, drawPosition, this.source, this.color, this.rotation, this.origin, this.scale, this.effects, this.layer);
        }


        #endregion

        // Null kommer behövas sättas som .Size, annars kommer inte clipSize att fungera,
        

        public void DrawWithTextureSize()
        {
            if (this.isVisable)
                this.spriteBatch.Draw(this.texture, new Rectangle((int)this.position.X, (int)this.position.Y, this.texture.Width, this.texture.Height), null, this.color, this.rotation, new Vector2(0, 0), SpriteEffects.None, this.layer);
        }
         * */
        #endregion
    }
}
