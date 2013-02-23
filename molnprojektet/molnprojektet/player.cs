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
        enum PlayerSprites { Cloud, LeftHumerus, LeftUlna, LeftHand, RightHumerus, RightUlna, RightHand };
        private Sprite cloudSprite;
        private Sprite leftHumerusSprite;
        private Sprite leftUlnaSprite;
        private Sprite leftHandSprite;
        private Sprite rightHumerusSprite;
        private Sprite rightUlnaSprite;
        private Sprite rightHandSprite;
        private Vector2 speed = new Vector2(0,0);
        private DateTime currentTime = DateTime.Now;
        private DateTime previusTime = DateTime.Now;

        enum CloudDirection {None, Left, Right}
        private Dictionary<CloudDirection, Texture2D> cloudTextures;

        private const int acceleration = -20;
        private const int MAX_SPEED = 4;

        private int rightHumerusOffsetX;
        private int rightHumerusOffsetY;
        private int rightUlnaOffset;
        private int rightHandOffset;
        private int leftHumerusOffsetX;
        private int leftHumerusOffsetY;
        private int leftUlnaOffset;
        private int leftHandOffset;
        private Vector2 ScreenOffset;

        private Dictionary<PlayerSprites, Sprite> spriteDict;

        private Vector2 position;

        public readonly object locker = new object();

        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (value.X > 0 && value.X + cloudSprite.Size.X < ScreenOffset.X &&
                    value.Y > 0 && value.Y + cloudSprite.Size.Y < ScreenOffset.Y)
                {
                    Vector2 diffVector = value - position;

                    foreach (Sprite sprite in spriteDict.Values)
                        Utils.addToSpritePosition(sprite, diffVector);

                    cloudSprite.Position = value;
                    position = value;
                }
            }
        }

        public Vector2 Speed
        {
            get { lock (this.locker) return speed; }
            set 
            {
                lock (this.locker)
                {
                    if (value.X <= MAX_SPEED)
                        speed.X = value.X;
                    if (value.Y <= MAX_SPEED)
                        speed.Y = value.Y;
                }
            }
        }


        public Player(Vector2 screenOffset)
        {
            spriteDict = new Dictionary<PlayerSprites, Sprite>();
            this.ScreenOffset = screenOffset;
            InitSprites();
            Position = new Vector2(400, 300);
            InitArms();

            shadePositions = new Queue<Vector2>();

            SetLeftArmRotation((float)Math.PI / 2, (float)Math.PI / 2);
            SetRightArmRotation(-(float)Math.PI / 2, -(float)Math.PI / 2);
            Position = new Vector2(200, 300);
            Position = new Vector2(100, 100);
            Position = new Vector2(600, 400);
        }

        private void InitSprites()
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

            cloudTextures = new Dictionary<CloudDirection, Texture2D>();
            cloudTextures.Add(CloudDirection.None, Game1.contentManager.Load<Texture2D>(@"Images\Cloud"));
            cloudTextures.Add(CloudDirection.Left, Game1.contentManager.Load<Texture2D>(@"Images\Cloud_Move_Left"));
            cloudTextures.Add(CloudDirection.Right, Game1.contentManager.Load<Texture2D>(@"Images\Cloud_Move_Right"));

            cloudSprite.Texture = cloudTextures[CloudDirection.None];
            leftHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Humerus_left");
            leftUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Ulna_left");
            leftHandSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Hand_left");
            rightHumerusSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Humerus_right");
            rightUlnaSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Ulna_right");
            rightHandSprite.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Hand_right");

            leftUlnaSprite.Layer = 0f;
            rightUlnaSprite.Layer = 0f;

            // Origin to right mid
            leftHumerusSprite.Origin = new Vector2(leftHumerusSprite.Texture.Width, leftHumerusSprite.Texture.Height / 2);
            leftUlnaSprite.Origin = new Vector2(leftUlnaSprite.Texture.Width, leftUlnaSprite.Texture.Height / 2);
            leftHandSprite.Origin = new Vector2(leftHandSprite.Texture.Width, leftHandSprite.Texture.Height * 5 / 7);

            //Origin to left mid
            rightHumerusSprite.Origin = new Vector2(0, rightHumerusSprite.Texture.Height / 2);
            rightUlnaSprite.Origin = new Vector2(0, rightUlnaSprite.Texture.Height / 2);
            rightHandSprite.Origin = new Vector2(0, rightHandSprite.Texture.Height * 5 / 7);

            spriteDict = new Dictionary<PlayerSprites, Sprite>();
            spriteDict.Add(PlayerSprites.Cloud, cloudSprite);
            spriteDict.Add(PlayerSprites.RightUlna, rightUlnaSprite);
            spriteDict.Add(PlayerSprites.LeftUlna, leftUlnaSprite);
            spriteDict.Add(PlayerSprites.LeftHumerus, leftHumerusSprite);
            spriteDict.Add(PlayerSprites.LeftHand, leftHandSprite);
            spriteDict.Add(PlayerSprites.RightHumerus, rightHumerusSprite);
            spriteDict.Add(PlayerSprites.RightHand, rightHandSprite);
        }

        public void UpdatePosition()
        {
            lock (this.locker)
            {
                UpdateSpeed();
                Position += speed;
            }
        }

        private static float DirectionSpriteThreshold = 5;

        private void UpdateSpeed()
        {
            currentTime = DateTime.Now;
            Vector2 newSpeed = Vector2.Zero;
            if (speed != Vector2.Zero)
            {
                if (speed.X > 0)
                    newSpeed.X = acceleration * (currentTime - previusTime).Milliseconds / 1000 + speed.X;
                else if (speed.X < 0)
                    newSpeed.X = (-acceleration) * (currentTime - previusTime).Milliseconds / 1000 + speed.X;
                if (speed.Y > 0)
                    newSpeed.Y = acceleration * (currentTime - previusTime).Milliseconds / 1000 + speed.Y;
                else if (speed.Y < 0)
                    newSpeed.Y = (-acceleration) * (currentTime - previusTime).Milliseconds / 1000 + speed.Y;

                if (newSpeed.X > 0 && newSpeed.X < 0.15f || newSpeed.X > -0.15f && newSpeed.X < 0)
                    newSpeed.X = 0;
                if (newSpeed.Y > 0 && newSpeed.Y < 0.15f || newSpeed.Y > -0.15f && newSpeed.Y < 0)
                    newSpeed.Y = 0;
                speed = newSpeed;
            }
            previusTime = currentTime;

            if (speed.X < -DirectionSpriteThreshold)
                cloudSprite.Texture = cloudTextures[CloudDirection.Left];
            if (speed.X > DirectionSpriteThreshold)
                cloudSprite.Texture = cloudTextures[CloudDirection.Right];
            else
                cloudSprite.Texture = cloudTextures[CloudDirection.None];
        }

        private void InitArms()
        {
            leftHumerusOffsetX = (int)(cloudSprite.Texture.Width * 0.1);
            leftHumerusOffsetY = (int)(cloudSprite.Texture.Height * 0.6);
            leftUlnaOffset = -(int)(leftHumerusSprite.Texture.Width * 0.95);
            leftHandOffset = -(int)(leftUlnaSprite.Texture.Width * 0.97);

            rightHumerusOffsetX = (int)(cloudSprite.Texture.Width * 0.8);
            rightHumerusOffsetY = (int)(cloudSprite.Texture.Height * 0.6);
            rightUlnaOffset = (int)(rightHumerusSprite.Texture.Width * 0.95);
            rightHandOffset = (int)(rightUlnaSprite.Texture.Width * 0.97);

            //Set left
            Vector2 newHumerusPosition = new Vector2();
            newHumerusPosition.X = position.X + leftHumerusOffsetX;
            newHumerusPosition.Y = position.Y + leftHumerusOffsetY;

            Vector2 newUlnaPosition = new Vector2();
            newUlnaPosition.X = newHumerusPosition.X + leftUlnaOffset;
            newUlnaPosition.Y = newHumerusPosition.Y;

            Vector2 newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + leftHandOffset;
            newHandPosition.Y = newUlnaPosition.Y;

            leftHumerusSprite.Position = newHumerusPosition;
            leftUlnaSprite.Position = newUlnaPosition;
            leftHandSprite.Position = newHandPosition;

            //Set right
            newHumerusPosition = new Vector2();
            newHumerusPosition.X = position.X + rightHumerusOffsetX;
            newHumerusPosition.Y = position.Y + rightHumerusOffsetY;

            newUlnaPosition = new Vector2();
            newUlnaPosition.X = newHumerusPosition.X + rightUlnaOffset;
            newUlnaPosition.Y = newHumerusPosition.Y;

            newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + rightHandOffset;
            newHandPosition.Y = newUlnaPosition.Y;

            rightHumerusSprite.Position = newHumerusPosition;
            rightUlnaSprite.Position = newUlnaPosition;
            rightHandSprite.Position = newHandPosition;

        }

        public void SetLeftArmRotation(float humerusRotation, float ulnaRotation)
        {

            leftHumerusSprite.Rotation = humerusRotation;

            Vector2 newUlnaPosition = new Vector2();
            newUlnaPosition.X = leftHumerusSprite.Position.X + (int)(Math.Cos(humerusRotation) * leftUlnaOffset);
            newUlnaPosition.Y = leftHumerusSprite.Position.Y + (int)(Math.Sin(humerusRotation) * leftUlnaOffset);

            Vector2 newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + (int)(Math.Cos(ulnaRotation) * leftHandOffset);
            newHandPosition.Y = newUlnaPosition.Y + (int)(Math.Sin(ulnaRotation) * leftHandOffset);

            lock (locker)
            {
                leftUlnaSprite.Position = newUlnaPosition;
                leftUlnaSprite.Rotation = ulnaRotation;

                leftHandSprite.Position = newHandPosition;
                leftHandSprite.Rotation = ulnaRotation;
            }
        }

        public void SetRightArmRotation(float humerusRotation, float ulnaRotation)
        {

            rightHumerusSprite.Rotation = humerusRotation;

            Vector2 newUlnaPosition = new Vector2();
            newUlnaPosition.X = rightHumerusSprite.Position.X + (int)(Math.Cos(humerusRotation) * rightUlnaOffset);
            newUlnaPosition.Y = rightHumerusSprite.Position.Y + (int)(Math.Sin(humerusRotation) * rightUlnaOffset);

            Vector2 newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + (int)(Math.Cos(ulnaRotation) * rightHandOffset);
            newHandPosition.Y = newUlnaPosition.Y + (int)(Math.Sin(ulnaRotation) * rightHandOffset);

            lock (locker)
            {
                rightUlnaSprite.Position = newUlnaPosition;
                rightUlnaSprite.Rotation = ulnaRotation;

                rightHandSprite.Position = newHandPosition;
                rightHandSprite.Rotation = ulnaRotation;
            }
        }

        private Queue<Vector2> shadePositions;
        private DateTime shadeTimer = DateTime.Now;
        
        private void DrawShades(GraphicsHandler g)
        {
            if (shadeTimer < DateTime.Now) 
            {
                shadePositions.Enqueue(position);
                if (shadePositions.Count > 5)
                    shadePositions.Dequeue();
            }

            byte alpha = 128;
            lock (locker)
            {
                //TODO: Test if reverse order is needed
                foreach (Vector2 v in shadePositions.ToList<Vector2>())
                {

                        Position = v;
                        Color color;
                        foreach (Sprite sprite in spriteDict.Values)
                        {
                            color = sprite.Color;
                            color.A = alpha;
                            g.DrawSprite(sprite);
                        }
                        alpha -= 20;
                }
            }
        }

        public void Draw(GraphicsHandler g)
        {
            lock(locker)
                foreach(Sprite sprite in spriteDict.Values)
                    g.DrawSprite(sprite);
        }
    }
}
