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

        
        public void Update()
        {
            playerCloud.UpdatePosition();

            #region Key States
            KeyboardState newState = Keyboard.GetState();

            if(newState.IsKeyDown(Keys.Down))
            {
                //cloud.Position += new Vector2(0,3);   
            }
            else if(newState.IsKeyDown(Keys.Up))
            {
                //cloud.Position -= new Vector2(0, 3);
            }
            if(newState.IsKeyDown(Keys.Right))
            {
                //cloud.Position += new Vector2(3, 0);
            }
            else if (newState.IsKeyDown(Keys.Left))
            {
                //cloud.Position -= new Vector2(3, 0);
            }

            oldState = newState;
            #endregion
        }

        public void SwipeUp()
        {
            playerCloud.Speed += new Vector2(0,-2);
        }
        public void SwipeDown()
        {
            playerCloud.Speed += new Vector2(0, 2);
        }
        public void SwipeLeft()
        {
            playerCloud.Speed += new Vector2(-2, 0);
        }
        public void SwipeRight()
        {
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
