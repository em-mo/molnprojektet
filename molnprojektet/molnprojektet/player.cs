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
        private Vector2 speed = Vector2.Zero;
        private DateTime currentTime = DateTime.Now;
        private DateTime previusTime = DateTime.Now;

        private const int acceleration = -4;
        private const int MAX_SPEED = 30;

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

        public Vector2 Speed
        {
            get { return speed; }
            set 
            { 
                if(value.X <= MAX_SPEED)
                    speed.X = value.X;
                if (value.Y <= MAX_SPEED)
                    speed.Y = value.Y;
            }
        }

        public readonly object locker;

        public List<Sprite> getSprites()
        {
            List<Sprite> sprites = new List<Sprite>();
            sprites.Add(cloudSprite);
            sprites.Add(leftHumerusSprite);
            sprites.Add(leftUlnaSprite);
            sprites.Add(leftHandSprite);
            sprites.Add(rightHumerusSprite);
            sprites.Add(rightUlnaSprite);
            sprites.Add(rightHandSprite);

            return sprites;
        }

        public Player()
        {
            cloudSprite = new Sprite();
            leftHumerusSprite = new Sprite();
            leftUlnaSprite = new Sprite();
            leftHandSprite = new Sprite();
            rightHumerusSprite = new Sprite();
            rightUlnaSprite = new Sprite();
            rightHandSprite = new Sprite();

            cloudSprite.Initialize();
            leftHumerusSprite.Initialize();
            leftUlnaSprite.Initialize();
            leftHandSprite.Initialize();
            rightHumerusSprite.Initialize();
            rightUlnaSprite.Initialize();
            rightHandSprite.Initialize();



            cloudSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Cloud");
            leftHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Humerus_left");
            leftUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Ulna_left");
            leftHandSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Hand_left");
            rightHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Humerus_right");
            rightUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Ulna_right");
            rightHandSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Hand_right");

            // Origin to right mid
            leftHumerusSprite.Origin = new Vector2(leftHumerusSprite.Texture.Width, leftHumerusSprite.Texture.Height / 2);
            leftUlnaSprite.Origin = new Vector2(leftUlnaSprite.Texture.Width, leftUlnaSprite.Texture.Height / 2);
            leftHandSprite.Origin = new Vector2(leftHandSprite.Texture.Width, leftHandSprite.Texture.Height / 2);

            //Origin to left mid
            rightHumerusSprite.Origin = new Vector2(0, rightHumerusSprite.Texture.Height / 2);
            rightUlnaSprite.Origin = new Vector2(0, rightUlnaSprite.Texture.Height / 2);
            rightHandSprite.Origin = new Vector2(0, rightHandSprite.Texture.Height / 2);

        }

        public void UpdatePosition()
        {
            UpdateSpeed();
            Position += speed;
        }
        private const double speedFactor = 15;
        
        private void UpdateSpeed()
        {
            currentTime = DateTime.Now;
            Vector2 newSpeed;
            newSpeed.X = acceleration * (currentTime - previusTime).Milliseconds/1000 + speed.X;
            newSpeed.Y = acceleration * (currentTime - previusTime).Milliseconds/1000 + speed.Y;
            if (newSpeed.X < 0)
                newSpeed.X = 0;
            if (newSpeed.Y < 0)
                newSpeed.Y = 0;
            speed = newSpeed;
            previusTime = currentTime;
        }

        public void setLeftArmRotation(float humerusRotation, float ulnaRotation)
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

        public void setRightArmRotation(float humerusRotation, float ulnaRotation)
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
}
