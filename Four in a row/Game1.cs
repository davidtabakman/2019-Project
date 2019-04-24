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
using Network;

namespace GameCenter
{
 
    public class Commands
    {
        public static bool StartFourInARow(GraphicsDevice GraphicsDevice)
        {
            int[] args = { 7, 6, 1 };
            Game1.Screen.SetGUI(PresetGuis.InGame);
            Game1.Screen.AddControl(new FourInARowControl(), args);
            Game1.Screen.Start(GraphicsDevice);
            return true;
        }

        public static bool StartTicTac(GraphicsDevice GraphicsDevice)
        {
            int[] args = { 3, 3, 3 };
            Game1.Screen.SetGUI(PresetGuis.InGame);
            TicTacControl ticTacControl = new TicTacControl();
            ticTacControl.AttachBot(new DQN());
            Game1.Screen.AddControl(ticTacControl, args);
            Game1.Screen.Start(GraphicsDevice);
            return true;
        }

        public static bool OnPressExit()
        {
            Game1.Screen.Clear();
            Game1.Screen.SetGUI(PresetGuis.Menu);
            return true;
        }
    }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Constatnts
        // Window size constansts (static)
        public const int w_width = 1000;
        public const int w_height = 1000;

        public static GUIControl GUIControl;
        public static Screen Screen;

        public static bool ShutDown = false;
        public static Random random;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static SpriteFont MainFont { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            // Change initialization size to constants
            graphics.PreferredBackBufferWidth = w_width;
            graphics.PreferredBackBufferHeight = w_height;
            graphics.ApplyChanges();
            //Initialize event handler
            EventHandle.InitEventHandle(this);
            random = new Random();
            MainFont = Content.Load<SpriteFont>("Basic");
            NetworkVectors net2 = new NetworkVectors(new List<int>() { 3, 3, 2 }, 1.5);
            NetworkLoader.SaveSerializable(net2, "net2");
            net2 = null;
            net2 = NetworkLoader.LoadNetworkVectors("net2");
            net2.Print();
            PresetGuis.Setup(GraphicsDevice);
            Screen = new Screen();
            Screen.SetGUI(PresetGuis.Menu);
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
            Screen.Update(gameTime);

            if (ShutDown)
                Exit();

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

            Screen.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
