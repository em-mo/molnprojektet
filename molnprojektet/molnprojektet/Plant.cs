using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace molnprojektet
{
    class Plant
    {
        private Sprite plantSprite;
        static List<Texture2D> growthSpriteList = new List<Texture2D>();
        private int raindropsCount = 0;

        public Vector2 Position
        {
            get { return plantSprite.Position; }
            set { plantSprite.Position = value;}
        }

        public Vector2 GetSize()
        {
            return plantSprite.Size;
        }


        public Plant()
        {
            plantSprite = new Sprite();
            plantSprite.Initialize();
            plantSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\growthStage1");

        }

        
        public static void LoadContent()
        {
            growthSpriteList = new List<Texture2D>();
            Texture2D stage2 = Game1.contentManager.Load<Texture2D>(@"Images\growthStage2");
            growthSpriteList.Add(stage2);
            Texture2D stage3 = Game1.contentManager.Load<Texture2D>(@"Images\growthStage3");
            growthSpriteList.Add(stage3);
            Texture2D stage4 = Game1.contentManager.Load<Texture2D>(@"Images\growthStage4");
            growthSpriteList.Add(stage4);
        }

        public bool CheckCollisionWithRaindrops(Sprite raindrop)
        {
            if(raindropsCount != 12)
            {
                if (CheckForCollision(raindrop))
                {
                    raindropsCount++;
                    CheckForEvolve();
                    return true;
                }
            }
            return false;
        }

        private bool CheckForCollision(Sprite rainDrop)
        {
            if (plantSprite.Bounds.Contains(rainDrop.Bounds))
                return true;
            return false;
        }

        private void CheckForEvolve()
        {
            if (raindropsCount % 4 == 0)
            {
                Texture2D newTexture = Plant.growthSpriteList[raindropsCount / 4 - 1];
                Vector2 sizeDiff = new Vector2(plantSprite.Size.X - newTexture.Width, plantSprite.Size.Y - newTexture.Height);
                plantSprite.Texture = newTexture;
                plantSprite.Position += sizeDiff;
            }
        }

        public void Draw(GraphicsHandler graphicsHandler)
        {
            graphicsHandler.DrawSprite(plantSprite);
        }

    }
}
