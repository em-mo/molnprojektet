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
        KeyboardState oldState;

        public override void Initialize(SpriteBatch batch)
        {
            base.Initialize(batch);
            
            oldState = new KeyboardState();

            cloud = new Sprite();
            cloud.Initialize();
            cloud.Position = new Vector2(0, 0);
            cloud.Size = new Vector2(320, 160);
            cloud.Texture = Game1.contentManager.Load<Texture2D>(@"Cloud");
            AddSpriteToHandler(cloud);            
        }

        public override void Update()
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

            // Is the SPACE key down?
            if (newState.IsKeyDown(Keys.Space))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.Space))
                {

                }
            }
            else if (oldState.IsKeyDown(Keys.Space))
            {
                // Key was down last update, but not down now, so
                // it has just been released.
            }

            // Update saved state.
            oldState = newState;
        }


        
    }
}
