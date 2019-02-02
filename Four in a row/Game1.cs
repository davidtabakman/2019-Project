using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Four_in_a_row;
using GUI;
using TicTacToe;
using System.Collections.Generic;
using System;
using Learning;
using Controller;
using System.Threading;

namespace GameCenter
{
    
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Constatnts
        // Window size constansts (static)
        public const int w_width = 1000;
        public const int w_height = 1000;

        public static FourInARowControl FourInARowControl;
        public static GUIControl GUIControl;
        public static TicTacControl TicTacControl;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static SpriteFont MainFont { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private bool StartFourInARow()
        {
            int[] args = { 7, 6, 1 };
            GUIControl.Clear();
            GUIControl.AddButton(GraphicsDevice, new Rectangle(0, w_height - 25, 120, 25), "Exit", Color.Gray, "Exit", OnPressExit);
            FourInARowControl.Start(GraphicsDevice, args);
            return true;
        }

        

        private bool StartTicTac()
        {
            int[] args = { 3, 3, 3 };
            GUIControl.Clear();
            GUIControl.AddButton(GraphicsDevice, new Rectangle(0, w_height - 25, 120, 25), "Exit", Color.Gray, "Exit", OnPressExit);
            TicTacControl.Start(GraphicsDevice, args);

            return true;
        }

        private bool OnPressExit()
        {
            Exit();
            return true;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GUIControl = new GUIControl();
            FourInARowControl = new FourInARowControl();
            TicTacControl = new TicTacControl();
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            // Change initialization size to constants
            graphics.PreferredBackBufferWidth = w_width;
            graphics.PreferredBackBufferHeight = w_height;
            graphics.ApplyChanges();
            //Initialize event handler
            EventHandle.InitEventHandle(this);
            MainFont = Content.Load<SpriteFont>("Basic");
            Network.Network net = new Network.Network( new List<int>() { 2, 2, 1 }, Network.Activation_Functions.Sigmoid, 0.0);
            net.Print();
            net.Feed(new int[] { 1, 2 });
            GUIControl.Start(GraphicsDevice, null);
            GUIControl.AddButton(GraphicsDevice, new Rectangle(0, 0, 120, 25), "Start 4 in a row", Color.Gray, "Four Start" , StartFourInARow);
            GUIControl.AddButton(GraphicsDevice, new Rectangle(130, 0, 120, 25), "Start Tic Tac Tow", Color.Gray, "Tic Start", StartTicTac);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            // Add Control.Unload()
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // Update mouse manager
            Mousey.Update();
            if (FourInARowControl.Running)
                FourInARowControl.Update(gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            // Draw the state in Controller
            
            if (FourInARowControl.Running)
                FourInARowControl.Draw(spriteBatch);
            if (TicTacControl.Running)
                TicTacControl.Draw(spriteBatch);
            if (GUIControl.Running)
                GUIControl.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
