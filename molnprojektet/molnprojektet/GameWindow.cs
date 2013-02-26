using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;

namespace molnprojektet
{
    class GameWindow : Window
    {
        Sprite background;
        GraphicsHandler graphicsHandler;
        KeyboardState oldState;

        List<Sprite> spriteList = new List<Sprite>();
        List<Sprite> backgroundSprites = new List<Sprite>();

        private Player playerCloud;

        public Player PlayerCloud
        {
            get { return playerCloud; }
        }

        public void Initialize(SpriteBatch batch)
        {
            graphicsHandler = new GraphicsHandler();
            graphicsHandler.Initialize(batch);
            oldState = new KeyboardState();

            background = new Sprite();
            background.Initialize();
            background.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Gradient");
            background.Size = new Vector2(Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);

            playerCloud = new Player(background.Size);

            backgroundSprites.Add(background);
        }

        private bool[] keysDown = new bool[4];

        public void Update()
        {
            playerCloud.UpdatePosition();

            #region Key States
            KeyboardState newState = Keyboard.GetState();
            
            if(newState.IsKeyDown(Keys.Down) && !keysDown[0])
            {
                //cloud.Position += new Vector2(0,3);   
                SwipeDown();
                keysDown[0] = true;
            }
            if (newState.IsKeyUp(Keys.Down))
                keysDown[0] = false;
            if (newState.IsKeyDown(Keys.Up) && !keysDown[1])
            {
                //cloud.Position -= new Vector2(0, 3);
                SwipeUp();
                keysDown[1] = true;
            }
            if (newState.IsKeyUp(Keys.Up))
                keysDown[1] = false;
            if (newState.IsKeyDown(Keys.Right) && !keysDown[2])
            {
                //cloud.Position += new Vector2(3, 0);
                SwipeRight();
                keysDown[2] = true;
            }
            if (newState.IsKeyUp(Keys.Right))
                keysDown[2] = false;
            if (newState.IsKeyDown(Keys.Left) && !keysDown[3])
            {
                //cloud.Position -= new Vector2(3, 0);
                SwipeLeft();
                keysDown[3] = true;
            }
            if (newState.IsKeyUp(Keys.Left))
                keysDown[3] = false;

            oldState = newState;
            #endregion
        }

        public void SwipeUp()
        {
            playerCloud.AddWindPuff((float)Math.PI / 2, Arm.Right);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(0,-2);
        }
        public void SwipeDown()
        {
            playerCloud.AddWindPuff((float)-Math.PI / 2, Arm.Left);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(0, 2);
        }
        public void SwipeLeft()
        {
            playerCloud.AddWindPuff(0, Arm.Right);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(-2, 0);
        }
        public void SwipeRight()
        {
            playerCloud.AddWindPuff((float)Math.PI, Arm.Left);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(2, 0);
        }


        public void Draw()
        {
            graphicsHandler.DrawSprites(backgroundSprites);
            graphicsHandler.DrawSprites(spriteList);
            playerCloud.Draw(graphicsHandler);
        }
    }
}
