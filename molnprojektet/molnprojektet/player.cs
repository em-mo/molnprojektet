using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace molnprojektet
{
    class player
    {
        Sprite cloudSprite;
        Sprite leftHumerusSprite;
        Sprite leftUlnaSprite;
        Sprite leftHandSprite;
        Sprite rightHumerusSprite;
        Sprite rightUlnaSprite;
        Sprite rightHandSprite;

        public List<Sprite> getSprites()
        {
            List<Sprite> sprites = new List<Sprite>;
            sprites.Add(cloudSprite);
            sprites.Add(leftHumerusSprite);
            sprites.Add(leftUlnaSprite);
            sprites.Add(leftHandSprite);
            sprites.Add(rightHumerusSprite);
            sprites.Add(rightUlnaSprite);
            sprites.Add(rightHandSprite);

            return sprites;
        }

        public player()
        {
            cloudSprite.Texture = Game1.contentManager.Load<Texture2D>("Cloud.png");
            leftHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>("Humerus_left.png");
            leftUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>("Ulna_left.png");
            leftHandSprite.Texture = Game1.contentManager.Load<Texture2D>("Hand_left.png");
            rightHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>("Humerus_right.png");
            rightUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>("Ulna_right.png");
            rightHandSprite.Texture = Game1.contentManager.Load<Texture2D>("Hand_right.png");  

        }

        public void setLeftArmRotation(float humerusRotation, float ulnaRotation)
        {
            leftHumerusSprite.Rotation = humerusRotation;

            Vector2 newUlnaPosition;
        }
    }
}
