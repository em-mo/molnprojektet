using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace molnprojektet
{
    class Plant
    {
        private Sprite plantSprite;
        static List<Texture2D> growthSpriteList;
        private int raindropsCount = 0;

        static void InitializeGrowthList()
        {
            Texture2D stage1 = Game1.contentManager.Load<Texture2D>(@"Images\growthStage1");
            growthSpriteList.Add(stage1);
            Texture2D stage2 = Game1.contentManager.Load<Texture2D>(@"Images\growthStage2");
            growthSpriteList.Add(stage2);
            Texture2D stage3 = Game1.contentManager.Load<Texture2D>(@"Images\growthStage3");
            growthSpriteList.Add(stage3);
            Texture2D stage4 = Game1.contentManager.Load<Texture2D>(@"Images\growthStage4");
            growthSpriteList.Add(stage4);
        }

        public bool Update(Sprite raindrop)
        {
            if(raindropsCount != 16)
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
            if (plantSprite.Texture.Bounds.Contains(rainDrop.Texture.Bounds))
                return true;
            return false;
        }



        private void CheckForEvolve()
        {
            if (raindropsCount % 4 == 0)
            {
                plantSprite.Texture = growthSpriteList[raindropsCount / 4];
            }
        }

    }
}
