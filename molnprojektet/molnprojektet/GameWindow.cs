﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using System.Diagnostics;

namespace molnprojektet
{
    class GameWindow : Window
    {
        Sprite background;
        GraphicsHandler graphicsHandler;
        KeyboardState oldState;

        List<Sprite> raindropsList = new List<Sprite>();
        List<Plant> plantList = new List<Plant>();
        List<Sprite> spriteList = new List<Sprite>();
        List<Sprite> backgroundSprites = new List<Sprite>();
        private int dropDelay = 300;
        const float dropSpeed = 200;
        private Stopwatch timer = new Stopwatch();
        public readonly object dropLock = new object();

        private Player playerCloud;

        public Player PlayerCloud
        {
            get { return playerCloud; }
        }

        public void Initialize(SpriteBatch batch)
        {

            timer.Start();

            Plant plant = new Plant();
            plant.Position = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2 , Game1.graphics.PreferredBackBufferHeight - plant.GetSize().Y);
            plantList.Add(plant);

            Plant plant2 = new Plant();
            plant2.Position = new Vector2(Game1.graphics.PreferredBackBufferWidth * 3 / 4, Game1.graphics.PreferredBackBufferHeight - plant2.GetSize().Y);
            plantList.Add(plant2);

            Plant plant3 = new Plant();
            plant3.Position = new Vector2(Game1.graphics.PreferredBackBufferWidth / 4, Game1.graphics.PreferredBackBufferHeight - plant3.GetSize().Y);
            plantList.Add(plant3);

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

        public void Update(GameTime gameTime)
        {
            releaseRainDrops();
            playerCloud.Update(gameTime);
            UpdateFallingRaindrops(gameTime);

            #region Key States
            KeyboardState newState = Keyboard.GetState();
            
            if(newState.IsKeyDown(Keys.Down) && !keysDown[0])
            {
                //cloud.Position += new Vector2(0,3);   
                SwipeDown(Arm.Left);
                keysDown[0] = true;
            }
            if (newState.IsKeyUp(Keys.Down))
                keysDown[0] = false;
            if (newState.IsKeyDown(Keys.Up) && !keysDown[1])
            {
                //cloud.Position -= new Vector2(0, 3);
                SwipeUp(Arm.Right);
                keysDown[1] = true;
            }
            if (newState.IsKeyUp(Keys.Up))
                keysDown[1] = false;
            if (newState.IsKeyDown(Keys.Right) && !keysDown[2])
            {
                //cloud.Position += new Vector2(3, 0);
                SwipeRight(Arm.Right);
                keysDown[2] = true;
            }
            if (newState.IsKeyUp(Keys.Right))
                keysDown[2] = false;
            if (newState.IsKeyDown(Keys.Left) && !keysDown[3])
            {
                //cloud.Position -= new Vector2(3, 0);
                SwipeLeft(Arm.Left);
                keysDown[3] = true;
            }
            if (newState.IsKeyUp(Keys.Left))
                keysDown[3] = false;

            oldState = newState;
            #endregion
        }

        public void releaseRainDrops()
        {

            if (timer.ElapsedMilliseconds > dropDelay)
            {
                Sprite drop = new Sprite();
                drop.Initialize();
                drop.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Drop");
                Random rand  = new Random();
                float xValue = rand.Next((int)playerCloud.Position.X , (int)playerCloud.Position.X + (int)playerCloud.GetSize().X);
                float yValue = playerCloud.Position.Y + playerCloud.GetSize().Y;
                drop.Position = new Vector2(xValue, yValue);

                lock(dropLock)
                    raindropsList.Add(drop);

                timer.Restart();
            }
        }

        public void UpdateFallingRaindrops(GameTime gameTime)
        {
            for (int i = raindropsList.Count - 1; i >= 0; i--)
            {
                Sprite drop = raindropsList.ElementAt(i);
                drop.Position += new Vector2(0, dropSpeed * gameTime.ElapsedGameTime.Milliseconds/1000);

                foreach (Plant plant in plantList)
                {   
                    if (plant.CheckCollisionWithRaindrops(drop))
                        raindropsList.Remove(drop);
                }
                
                if(drop.Position.Y + drop.Size.Y >= background.Size.Y)
                    raindropsList.Remove(drop);
            }
        }

        public void SwipeUp(Arm arm)
        {
            playerCloud.AddWindPuff((float)Math.PI / 2, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(0,200);
        }
        public void SwipeDown(Arm arm)
        {
            playerCloud.AddWindPuff((float)-Math.PI / 2, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(0, -200);
        }
        public void SwipeLeft(Arm arm)
        {
            playerCloud.AddWindPuff(0, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(200, 0);
        }
        public void SwipeRight(Arm arm)
        {
            playerCloud.AddWindPuff((float)Math.PI, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(-200, 0);
        }


        public void Draw(GameTime gameTime)
        {
            graphicsHandler.DrawSprites(backgroundSprites);
            foreach (Plant plant in plantList)
            {
                plant.Draw(graphicsHandler);
            }
            graphicsHandler.DrawSprites(raindropsList);
            graphicsHandler.DrawSprites(spriteList);
            playerCloud.Draw(graphicsHandler);
        }
    }
}
