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

        private const float acceleration = -3;
        private const float MAX_SPEED = 5;

        private float rightHumerusOffsetX;
        private float rightHumerusOffsetY;
        private float rightUlnaOffset;
        private float rightHandOffset;
        private float leftHumerusOffsetX;
        private float leftHumerusOffsetY;
        private float leftUlnaOffset;
        private float leftHandOffset;
        private Vector2 ScreenOffset;

        private Dictionary<PlayerSprites, Sprite> spriteDict;

        private Vector2 position;

        public readonly object locker = new object();

        public Vector2 Position
        {
            get { return position; }
            set
            {
                PositionHelper(value);
                Vector2 adjustedPosition = position;

                //Bound checking
                if (position.X < 0)
                    adjustedPosition.X = 0;
                else if (position.X + cloudSprite.Size.X > ScreenOffset.X)
                    adjustedPosition.X = ScreenOffset.X - cloudSprite.Size.X;

                if (position.Y < 0)
                    adjustedPosition.Y = 0;
                else if (position.Y + cloudSprite.Size.Y > ScreenOffset.Y)
                    adjustedPosition.Y = ScreenOffset.Y - cloudSprite.Size.Y;

                if (adjustedPosition != position)
                    PositionHelper(adjustedPosition);
            }
        }

        //Sets position of all sprites
        private void PositionHelper(Vector2 v)
        {
            Vector2 diffVector = v - position;

            foreach (Sprite sprite in spriteDict.Values)
                Utils.AddToSpritePosition(sprite, diffVector);

            cloudSprite.Position = v;
            position = v;
        }

        public Vector2 Speed
        {
            get { lock (this.locker) return speed; }
            set 
            {
                lock (this.locker)
                {
                    speed = value;
                    if (value.X > MAX_SPEED)
                        speed.X = MAX_SPEED;
                    else if (value.X < -MAX_SPEED)
                        speed.X = -MAX_SPEED;

                    if (value.Y > MAX_SPEED)
                        speed.Y = MAX_SPEED;
                    else if (value.Y < -MAX_SPEED)
                        speed.Y = -MAX_SPEED;
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

        private static float DirectionSpriteThreshold = 4;

        private void UpdateSpeed()
        {
            currentTime = DateTime.Now;
            Vector2 newSpeed = Vector2.Zero;
            if (speed != Vector2.Zero)
            {
                if (speed.X > 0)
                    newSpeed.X = acceleration * (float)Utils.TicksToSeconds((currentTime - previusTime).Ticks) + speed.X;
                else if (speed.X < 0)
                    newSpeed.X = (-acceleration) * (float)Utils.TicksToSeconds((currentTime - previusTime).Ticks) + speed.X;
                if (speed.Y > 0)
                    newSpeed.Y = acceleration * (float)Utils.TicksToSeconds((currentTime - previusTime).Ticks) + speed.Y;
                else if (speed.Y < 0)
                    newSpeed.Y = (-acceleration) * (float)Utils.TicksToSeconds((currentTime - previusTime).Ticks) + speed.Y;

                if (newSpeed.X < 0.15f && newSpeed.X > -0.15f)
                    newSpeed.X = 0;
                if (newSpeed.Y < 0.15f && newSpeed.Y > -0.15f)
                    newSpeed.Y = 0;
                Speed = newSpeed;
            }
            previusTime = currentTime;

            if (speed.X < -DirectionSpriteThreshold)
                cloudSprite.Texture = cloudTextures[CloudDirection.Left];
            else if (speed.X > DirectionSpriteThreshold)
                cloudSprite.Texture = cloudTextures[CloudDirection.Right];
            else
                cloudSprite.Texture = cloudTextures[CloudDirection.None];
        }

        private void InitArms()
        {
            leftHumerusOffsetX = (float)(cloudSprite.Texture.Width * 0.1);
            leftHumerusOffsetY = (float)(cloudSprite.Texture.Height * 0.6);
            leftUlnaOffset = -(float)(leftHumerusSprite.Texture.Width * 0.95);
            leftHandOffset = -(float)(leftUlnaSprite.Texture.Width * 0.97);

            rightHumerusOffsetX = (float)(cloudSprite.Texture.Width * 0.8);
            rightHumerusOffsetY = (float)(cloudSprite.Texture.Height * 0.6);
            rightUlnaOffset = (float)(rightHumerusSprite.Texture.Width * 0.95);
            rightHandOffset = (float)(rightUlnaSprite.Texture.Width * 0.97);

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
            newUlnaPosition.X = leftHumerusSprite.Position.X + (float)(Math.Cos(humerusRotation) * leftUlnaOffset);
            newUlnaPosition.Y = leftHumerusSprite.Position.Y + (float)(Math.Sin(humerusRotation) * leftUlnaOffset);

            Vector2 newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + (float)(Math.Cos(ulnaRotation) * leftHandOffset);
            newHandPosition.Y = newUlnaPosition.Y + (float)(Math.Sin(ulnaRotation) * leftHandOffset);

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
            newUlnaPosition.X = rightHumerusSprite.Position.X + (float)(Math.Cos(humerusRotation) * rightUlnaOffset);
            newUlnaPosition.Y = rightHumerusSprite.Position.Y + (float)(Math.Sin(humerusRotation) * rightUlnaOffset);

            Vector2 newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + (float)(Math.Cos(ulnaRotation) * rightHandOffset);
            newHandPosition.Y = newUlnaPosition.Y + (float)(Math.Sin(ulnaRotation) * rightHandOffset);

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
        // Amount of time between two shades
        private const int ShadeAddDelay = 50;
        // Minimum speed before shades appear
        private const int ShadeSpeedThreshold = 0;
        // Maximum number of shades
        private const int MaxShades = 5;
        // Initial shade transparency
        private const float ShadeTransparency = 0.4f;
        
        private void DrawShades(GraphicsHandler g)
        {
            if (shadeTimer < DateTime.Now) 
            {
                shadeTimer = DateTime.Now.AddMilliseconds(ShadeAddDelay);

                if (speed.Length() > ShadeSpeedThreshold)
                    shadePositions.Enqueue(position);
                else if (shadePositions.Count > 0)
                    shadePositions.Dequeue();

                if (shadePositions.Count > MaxShades)
                    shadePositions.Dequeue();
            }

            if (shadePositions.Count > 0)
            {
                float alpha = ShadeTransparency;
                Vector2 startPosition = position;

                lock (locker)
                {
                    Color color;
                    //TODO: Test if reverse order is needed
                    List<Vector2> reversedList = shadePositions.ToList<Vector2>();
                    reversedList.Reverse();
                    foreach (Vector2 v in reversedList)
                    {

                        Position = v;
                        foreach (Sprite sprite in spriteDict.Values)
                        {
                            color = sprite.Color;
                            color *= alpha;
                            sprite.Color = color;
                            g.DrawSprite(sprite);
                        }
                        alpha += 0.1f;
                    }
                    foreach (Sprite sprite in spriteDict.Values)
                    {
                        sprite.Color = Color.White;
                    }
                    Position = startPosition;
                }
            }
        }

        public void Draw(GraphicsHandler g)
        {
            lock (locker)
                DrawShades(g);
                foreach(Sprite sprite in spriteDict.Values)
                    g.DrawSprite(sprite);
        }
    }
}
