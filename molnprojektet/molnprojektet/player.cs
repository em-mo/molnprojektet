using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace molnprojektet
{
    class Player
    {
        private Sprite cloudSprite;
        private Sprite leftHumerusSprite;
        private Sprite leftUlnaSprite;
        private Sprite leftHandSprite;
        private Sprite rightHumerusSprite;
        private Sprite rightUlnaSprite;
        private Sprite rightHandSprite;

        private List<Sprite> spriteList;

        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                Vector2 diffVector = value - position;

                Utils.addToSpritePosition(leftHumerusSprite, diffVector);
                Utils.addToSpritePosition(leftUlnaSprite, diffVector);
                Utils.addToSpritePosition(leftHandSprite, diffVector);
                Utils.addToSpritePosition(rightHumerusSprite, diffVector);
                Utils.addToSpritePosition(rightUlnaSprite, diffVector);
                Utils.addToSpritePosition(rightHandSprite, diffVector);

                cloudSprite.Position = value;
                position = value;
            }
        }

        public readonly object locker;

        public List<Sprite> getSprites()
        {
            return spriteList;
        }

        public Player()
        {
            cloudSprite.Texture = Game1.contentManager.Load<Texture2D>("Cloud.png");
            leftHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>("Humerus_left.png");
            leftUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>("Ulna_left.png");
            leftHandSprite.Texture = Game1.contentManager.Load<Texture2D>("Hand_left.png");
            rightHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>("Humerus_right.png");
            rightUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>("Ulna_right.png");
            rightHandSprite.Texture = Game1.contentManager.Load<Texture2D>("Hand_right.png");

            // Origin to right mid
            leftHumerusSprite.Origin = new Vector2(leftHumerusSprite.Texture.Width, leftHumerusSprite.Texture.Height / 2);
            leftUlnaSprite.Origin = new Vector2(leftUlnaSprite.Texture.Width, leftUlnaSprite.Texture.Height / 2);
            leftHandSprite.Origin = new Vector2(leftHandSprite.Texture.Width, leftHandSprite.Texture.Height / 2);

            //Origin to left mid
            rightHumerusSprite.Origin = new Vector2(0, rightHumerusSprite.Texture.Height / 2);
            rightUlnaSprite.Origin = new Vector2(0, rightUlnaSprite.Texture.Height / 2);
            rightHandSprite.Origin = new Vector2(0, rightHandSprite.Texture.Height / 2);

            spriteList = new List<Sprite>();
            spriteList.Add(cloudSprite);
            spriteList.Add(leftHumerusSprite);
            spriteList.Add(leftUlnaSprite);
            spriteList.Add(leftHandSprite);
            spriteList.Add(rightHumerusSprite);
            spriteList.Add(rightUlnaSprite);
            spriteList.Add(rightHandSprite);

        }

        public void setLeftArmRotation(float humerusRotation, float ulnaRotation)
        {
            lock (locker)
            {
                leftHumerusSprite.Rotation = humerusRotation;

                Vector2 newUlnaPosition = new Vector2();
                newUlnaPosition.X = leftHumerusSprite.Position.X - (int)(Math.Cos(humerusRotation) * leftHumerusSprite.Texture.Width);
                newUlnaPosition.Y = leftHumerusSprite.Position.Y - (int)(Math.Sin(humerusRotation) * leftHumerusSprite.Texture.Width);

                leftUlnaSprite.Position = newUlnaPosition;
                leftUlnaSprite.Rotation = ulnaRotation;

                Vector2 newHandPosition = new Vector2();
                newHandPosition.X = newUlnaPosition.X - (int)(Math.Cos(ulnaRotation) * leftUlnaSprite.Texture.Width);
                newHandPosition.X = newUlnaPosition.Y - (int)(Math.Sin(ulnaRotation) * leftUlnaSprite.Texture.Width);

                leftHandSprite.Position = newHandPosition;
                leftHandSprite.Rotation = ulnaRotation;
            }
        }

        public void setRightArmRotation(float humerusRotation, float ulnaRotation)
        {
            lock (locker)
            {
                rightHumerusSprite.Rotation = humerusRotation;

                Vector2 newUlnaPosition = new Vector2();
                newUlnaPosition.X = rightHumerusSprite.Position.X + (int)(Math.Cos(humerusRotation) * rightHumerusSprite.Texture.Width);
                newUlnaPosition.Y = rightHumerusSprite.Position.Y - (int)(Math.Sin(humerusRotation) * rightHumerusSprite.Texture.Width);

                rightUlnaSprite.Position = newUlnaPosition;
                rightUlnaSprite.Rotation = ulnaRotation;

                Vector2 newHandPosition = new Vector2();
                newHandPosition.X = newUlnaPosition.X + (int)(Math.Cos(ulnaRotation) * rightUlnaSprite.Texture.Width);
                newHandPosition.X = newUlnaPosition.Y - (int)(Math.Sin(ulnaRotation) * rightUlnaSprite.Texture.Width);

                rightHandSprite.Position = newHandPosition;
                rightHandSprite.Rotation = ulnaRotation;
            }
        }

        public void draw(GraphicsHandler g)
        {
            lock(locker)
                g.DrawSprites(spriteList);
        }
    }
}
