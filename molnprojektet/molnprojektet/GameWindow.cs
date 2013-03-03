using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

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
        List<PoisonCloud> poisonCloudList = new List<PoisonCloud>();
        List<DeathFactory> deathFactoryList = new List<DeathFactory>();

        SoundEffect notCarrie;
        SoundEffectInstance notCarrieInstance;

        private int dropDelay = 300;
        const float dropSpeed = 200;
        private const int moveSpeed = 135;

        private Stopwatch timer = new Stopwatch();
        Random rand = new Random();

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
            plant.Position = new Vector2(Game1.graphics.PreferredBackBufferWidth / 8, Game1.graphics.PreferredBackBufferHeight - plant.GetSize().Y);
            plantList.Add(plant);

            Plant plant2 = new Plant();
            plant2.Position = new Vector2(Game1.graphics.PreferredBackBufferWidth * 6 / 8, Game1.graphics.PreferredBackBufferHeight - plant2.GetSize().Y);
            plantList.Add(plant2);

            
            DeathFactory factory = new DeathFactory(this);
            factory.Position = new Vector2(Game1.graphics.PreferredBackBufferWidth / 2, Game1.graphics.PreferredBackBufferHeight - factory.GetSize().Y);
            deathFactoryList.Add(factory);

            graphicsHandler = new GraphicsHandler();
            graphicsHandler.Initialize(batch);
            oldState = new KeyboardState();

            background = new Sprite();
            background.Initialize();
            background.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Gradient");
            background.Size = new Vector2(Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
            background.Layer = 0;

            playerCloud = new Player(background.Size);

            backgroundSprites.Add(background);

            notCarrie = Game1.contentManager.Load<SoundEffect>(@"Sounds/carrie2");
            notCarrieInstance = notCarrie.CreateInstance();
        }

        public void StartNotCarrie()
        {
            if (notCarrieInstance.State == SoundState.Stopped)
            {
                notCarrieInstance.Volume = 0.75f;
                notCarrieInstance.IsLooped = true;
                notCarrieInstance.Play();
            }
            else if (notCarrieInstance.State == SoundState.Paused)
                notCarrieInstance.Resume();
        }

        public void StopNotCarrie()
        {
            if (notCarrieInstance.State == SoundState.Playing)
                notCarrieInstance.Pause();
        }

        private bool[] keysDown = new bool[4];

        public void Update(GameTime gameTime)
        {
            playerCloud.Update(gameTime);
            UpdateFallingRaindrops(gameTime);
            UpdateFactories(gameTime);
            UpdatePoisonClouds(gameTime);
            CheckForResetFlowers();
            CheckForCloudCollision();

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

        private void CheckForCloudCollision()
        {
            foreach (PoisonCloud  cloud in poisonCloudList)
            {
                if (cloud.GetSprite().Bounds.Intersects(playerCloud.GetBounds()))
                {
                    playerCloud.IsSick = true;
                }
            }
        }

        private void CheckForResetFlowers()
        {
            bool reset = true;
            foreach (Plant plant in plantList)
            {
                if (plant.GetGrowthStage() != 2)
                {
                    reset = false;
                    break;
                }
            }
            if(reset)
                foreach (Plant plant in plantList)
                {
                    plant.Reset();
                }
        }

        public void releaseRainDrops()
        {
            if (timer.ElapsedMilliseconds > dropDelay)
            {
                Sprite drop = new Sprite();
                drop.Initialize();
                drop.Texture = Game1.contentManager.Load<Texture2D>(@"Images\Drop");
                float xValue = rand.Next((int)(playerCloud.Position.X +  0.2*playerCloud.GetSize().X) , (int)(playerCloud.Position.X + (playerCloud.GetSize().X) * 0.8));
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
                lock (dropLock)
                {
                    Sprite drop = raindropsList.ElementAt(i);

                    drop.Position += new Vector2(0, dropSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000);

                    foreach (Plant plant in plantList)
                    {
                        if (plant.CheckCollisionWithRaindrops(drop))    
                            raindropsList.Remove(drop);
                    }

                    if (drop.Position.Y + drop.Size.Y >= background.Size.Y)
                            raindropsList.Remove(drop);
                }
            }
        }

        private void UpdateFactories(GameTime gameTime)
        {
            foreach (DeathFactory factory in deathFactoryList)
            {
                factory.Update(gameTime);
            }
        }

        private void UpdatePoisonClouds(GameTime gameTime)
        {
            foreach (PoisonCloud cloud in poisonCloudList)
            {
                cloud.Update(gameTime);
            }
        }

        public void SwipeUp(Arm arm)
        {
            playerCloud.AddWindPuff((float)Math.PI / 2, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(0,120);
        }
        public void SwipeDown(Arm arm)
        {
            playerCloud.AddWindPuff((float)-Math.PI / 2, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(0, -120);
        }
        public void SwipeLeft(Arm arm)
        {
            playerCloud.AddWindPuff(0, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(120, 0);
        }
        public void SwipeRight(Arm arm)
        {
            playerCloud.AddWindPuff((float)Math.PI, arm);
            lock (playerCloud.locker)
                playerCloud.Speed += new Vector2(-200, 0);
        }

        public void AddPoisonCloud(Vector2 position)
        {
            poisonCloudList.Add(new PoisonCloud(position));
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

            foreach (PoisonCloud cloud in poisonCloudList)
            {
                cloud.Draw(graphicsHandler);
            }

            foreach (DeathFactory factory in deathFactoryList)
            {
                factory.Draw(graphicsHandler);
            }
        }
    }
}
