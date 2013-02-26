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
        private Vector2 speed = new Vector2(0,0);
        private DateTime currentTime = DateTime.Now;
        private DateTime previusTime = DateTime.Now;

        enum CloudDirection {None, Left, Right}
        private Dictionary<CloudDirection, Texture2D> cloudTextures;
        private Sprite windPuff;

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

        private Queue<Vector2> shadePositions;
        private DateTime shadeTimer = DateTime.Now;
        // Amount of time between two shades
        private const int ShadeAddDelay = 40;
        // Minimum speed before shades appear
        private const int ShadeSpeedThreshold = 2;
        // Maximum number of shades
        private const int MaxShades = 5;
        // Increment of shade transparencye
        private const float ShadeTransparency = 1.2f;

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
                else if (position.X + spriteDict[PlayerSprites.Cloud].Size.X > ScreenOffset.X)
                    adjustedPosition.X = ScreenOffset.X - spriteDict[PlayerSprites.Cloud].Size.X;

                if (position.Y < 0)
                    adjustedPosition.Y = 0;
                else if (position.Y + spriteDict[PlayerSprites.Cloud].Size.Y > ScreenOffset.Y)
                    adjustedPosition.Y = ScreenOffset.Y - spriteDict[PlayerSprites.Cloud].Size.Y;

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

            spriteDict[PlayerSprites.Cloud].Position = v;
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

            spriteDict = new Dictionary<PlayerSprites, Sprite>();

            foreach (PlayerSprites sprite in Enum.GetValues(typeof(PlayerSprites)))
            {
                spriteDict.Add(sprite, new Sprite());
                spriteDict[sprite].Initialize();
            }
            windPuff = new Sprite();
            windPuff.Initialize();

            cloudTextures = new Dictionary<CloudDirection, Texture2D>();
            cloudTextures.Add(CloudDirection.None, Game1.contentManager.Load<Texture2D>(@"Images\Cloud"));
            cloudTextures.Add(CloudDirection.Left, Game1.contentManager.Load<Texture2D>(@"Images\Cloud_Move_Left"));
            cloudTextures.Add(CloudDirection.Right, Game1.contentManager.Load<Texture2D>(@"Images\Cloud_Move_Right"));

            windPuff.Texture = Game1.contentManager.Load<Texture2D>(@"Images\wind");
            spriteDict[PlayerSprites.Cloud].Texture = cloudTextures[CloudDirection.None];
            spriteDict[PlayerSprites.LeftHumerus].Texture = Game1.contentManager.Load<Texture2D>(@"Images\Humerus_left");
            spriteDict[PlayerSprites.LeftUlna].Texture = Game1.contentManager.Load<Texture2D>(@"Images\Ulna_left");
            spriteDict[PlayerSprites.LeftHand].Texture = Game1.contentManager.Load<Texture2D>(@"Images\Hand_left");
            spriteDict[PlayerSprites.RightHumerus].Texture = Game1.contentManager.Load<Texture2D>(@"Images\Humerus_right");
            spriteDict[PlayerSprites.RightUlna].Texture = Game1.contentManager.Load<Texture2D>(@"Images\Ulna_right");
            spriteDict[PlayerSprites.RightHand].Texture = Game1.contentManager.Load<Texture2D>(@"Images\Hand_right");

            spriteDict[PlayerSprites.LeftUlna].Layer = 0f;
            spriteDict[PlayerSprites.RightUlna].Layer = 0f;

            // Origin to right mid
            spriteDict[PlayerSprites.LeftHumerus].Origin = new Vector2(spriteDict[PlayerSprites.LeftHumerus].Texture.Width, spriteDict[PlayerSprites.LeftHumerus].Texture.Height / 2);
            spriteDict[PlayerSprites.LeftUlna].Origin = new Vector2(spriteDict[PlayerSprites.LeftUlna].Texture.Width, spriteDict[PlayerSprites.LeftUlna].Texture.Height / 2);
            spriteDict[PlayerSprites.LeftHand].Origin = new Vector2(spriteDict[PlayerSprites.LeftHand].Texture.Width, spriteDict[PlayerSprites.LeftHand].Texture.Height * 5 / 7);

            //Origin to left mid
            spriteDict[PlayerSprites.RightHumerus].Origin = new Vector2(0, spriteDict[PlayerSprites.RightHumerus].Texture.Height / 2);
            spriteDict[PlayerSprites.RightUlna].Origin = new Vector2(0, spriteDict[PlayerSprites.RightUlna].Texture.Height / 2);
            spriteDict[PlayerSprites.RightHand].Origin = new Vector2(0, spriteDict[PlayerSprites.RightHand].Texture.Height * 5 / 7);

            //Origin center
            windPuff.Origin = new Vector2(windPuff.Texture.Width / 2, windPuff.Texture.Height / 2);


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
            //Deccelerate
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

            // Change cloud image depending on movement direction and speed
            if (speed.X < -DirectionSpriteThreshold)
                spriteDict[PlayerSprites.Cloud].Texture = cloudTextures[CloudDirection.Left];
            else if (speed.X > DirectionSpriteThreshold)
                spriteDict[PlayerSprites.Cloud].Texture = cloudTextures[CloudDirection.Right];
            else
                spriteDict[PlayerSprites.Cloud].Texture = cloudTextures[CloudDirection.None];
        }

        private void InitArms()
        {
            leftHumerusOffsetX = (float)(spriteDict[PlayerSprites.Cloud].Texture.Width * 0.1);
            leftHumerusOffsetY = (float)(spriteDict[PlayerSprites.Cloud].Texture.Height * 0.6);
            leftUlnaOffset = -(float)(spriteDict[PlayerSprites.LeftHumerus].Texture.Width * 0.95);
            leftHandOffset = -(float)(spriteDict[PlayerSprites.LeftUlna].Texture.Width * 0.97);

            rightHumerusOffsetX = (float)(spriteDict[PlayerSprites.Cloud].Texture.Width * 0.8);
            rightHumerusOffsetY = (float)(spriteDict[PlayerSprites.Cloud].Texture.Height * 0.6);
            rightUlnaOffset = (float)(spriteDict[PlayerSprites.RightHumerus].Texture.Width * 0.95);
            rightHandOffset = (float)(spriteDict[PlayerSprites.RightUlna].Texture.Width * 0.97);

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

            spriteDict[PlayerSprites.LeftHumerus].Position = newHumerusPosition;
            spriteDict[PlayerSprites.LeftUlna].Position = newUlnaPosition;
            spriteDict[PlayerSprites.LeftHand].Position = newHandPosition;

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

            spriteDict[PlayerSprites.RightHumerus].Position = newHumerusPosition;
            spriteDict[PlayerSprites.RightUlna].Position = newUlnaPosition;
            spriteDict[PlayerSprites.RightHand].Position = newHandPosition;

        }

        // Angles in radians
        public void SetLeftArmRotation(float humerusRotation, float ulnaRotation)
        {

            spriteDict[PlayerSprites.LeftHumerus].Rotation = humerusRotation;

            Vector2 newUlnaPosition = new Vector2();
            newUlnaPosition.X = spriteDict[PlayerSprites.LeftHumerus].Position.X + (float)(Math.Cos(humerusRotation) * leftUlnaOffset);
            newUlnaPosition.Y = spriteDict[PlayerSprites.LeftHumerus].Position.Y + (float)(Math.Sin(humerusRotation) * leftUlnaOffset);

            Vector2 newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + (float)(Math.Cos(ulnaRotation) * leftHandOffset);
            newHandPosition.Y = newUlnaPosition.Y + (float)(Math.Sin(ulnaRotation) * leftHandOffset);

            lock (locker)
            {
                spriteDict[PlayerSprites.LeftUlna].Position = newUlnaPosition;
                spriteDict[PlayerSprites.LeftUlna].Rotation = ulnaRotation;

                spriteDict[PlayerSprites.LeftHand].Position = newHandPosition;
                spriteDict[PlayerSprites.LeftHand].Rotation = ulnaRotation;
            }
        }

        // Angles in radians
        public void SetRightArmRotation(float humerusRotation, float ulnaRotation)
        {

            spriteDict[PlayerSprites.RightHumerus].Rotation = humerusRotation;

            Vector2 newUlnaPosition = new Vector2();
            newUlnaPosition.X = spriteDict[PlayerSprites.RightHumerus].Position.X + (float)(Math.Cos(humerusRotation) * rightUlnaOffset);
            newUlnaPosition.Y = spriteDict[PlayerSprites.RightHumerus].Position.Y + (float)(Math.Sin(humerusRotation) * rightUlnaOffset);

            Vector2 newHandPosition = new Vector2();
            newHandPosition.X = newUlnaPosition.X + (float)(Math.Cos(ulnaRotation) * rightHandOffset);
            newHandPosition.Y = newUlnaPosition.Y + (float)(Math.Sin(ulnaRotation) * rightHandOffset);

            lock (locker)
            {
                spriteDict[PlayerSprites.RightUlna].Position = newUlnaPosition;
                spriteDict[PlayerSprites.RightUlna].Rotation = ulnaRotation;

                spriteDict[PlayerSprites.RightHand].Position = newHandPosition;
                spriteDict[PlayerSprites.RightHand].Rotation = ulnaRotation;
            }
        }
        
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

                    foreach (Sprite sprite in spriteDict.Values)
                    {
                        color = new Color(30, 30, 30, 30);
                        sprite.Color = color;
                    }
                    //TODO: Test if reverse order is needed
                    List<Vector2> reversedList = shadePositions.ToList<Vector2>();
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
                    }
                    foreach (Sprite sprite in spriteDict.Values)
                    {
                        sprite.Color = Color.White;
                    }
                    Position = startPosition;
                }
            }
        }

        private Queue<WindPuffMessage> windPuffQueue = new Queue<WindPuffMessage>();

        public void AddWindPuff(float direction, Arm arm)
        {
            windPuffQueue.Enqueue(new WindPuffMessage(direction, arm));
        }

        private void DrawWindPuff(GraphicsHandler g)
        {
            WindPuffMessage message;
            while (windPuffQueue.Count > 0)
            {
                message = windPuffQueue.Dequeue();
                
                Sprite hand;
                float offset;
                if (message.Arm == Arm.Left)
                {
                    hand = spriteDict[PlayerSprites.LeftHand];
                    offset = -hand.Size.X;
                }
                else
                {
                    hand = spriteDict[PlayerSprites.RightHand];
                    offset = hand.Size.X;
                }

                windPuff.Position = new Vector2(hand.Position.X + offset, hand.Position.Y);
                windPuff.Rotation = message.Direction;
                System.Console.WriteLine(Position);

                g.DrawSprite(windPuff);
            }
        }

        public void Draw(GraphicsHandler g)
        {
            lock (locker)
                DrawWindPuff(g);
                DrawShades(g);
                foreach(Sprite sprite in spriteDict.Values)
                    g.DrawSprite(sprite);
        }
    }
}
