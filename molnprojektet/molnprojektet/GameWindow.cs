using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace molnprojektet
{
    class GameWindow : Window
    {
        Sprite cloud;
        Sprite background;
        GraphicsHandler graphicsHandler;
        KeyboardState oldState;
        List<Sprite> spriteList = new List<Sprite>();
        List<Sprite> backgroundSprites = new List<Sprite>();

        public void Initialize(SpriteBatch batch)
        {
            graphicsHandler = new GraphicsHandler();
            graphicsHandler.Initialize(batch);
            
            oldState = new KeyboardState();

            cloud = new Sprite();
            cloud.Initialize();
            cloud.Position = new Vector2(0, 0);
            cloud.Size = new Vector2(320, 160);
            cloud.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Cloud");
            
            background = new Sprite();
            background.Initialize();
            background.Size = new Vector2(Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
            background.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Gradient");

            spriteList.Add(cloud);

            backgroundSprites.Add(background);
        }

        public void Update()
        {
            KeyboardState newState = Keyboard.GetState();

            if(newState.IsKeyDown(Keys.Down))
            {
                cloud.Position += new Vector2(0,3);   
            }
            else if(newState.IsKeyDown(Keys.Up))
            {
                cloud.Position -= new Vector2(0, 3);
            }
            if(newState.IsKeyDown(Keys.Right))
            {
                cloud.Position += new Vector2(3, 0);
            }
            else if (newState.IsKeyDown(Keys.Left))
            {
                cloud.Position -= new Vector2(3, 0);
            }

            oldState = newState;
        }

        public void Draw()
        {
            graphicsHandler.DrawSprites(backgroundSprites);
            graphicsHandler.DrawSprites(spriteList);
        }
    }
}
