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
using static Four_in_a_row.FourInARowControl;
using static Controller.GameControlBase;

namespace GameCenter
{
    public enum Priorities
    {
        GUI = 1,
        Game = 2
    }

    /// <summary>
    /// Commands for buttons
    /// </summary>
    public class Commands
    {
        private static string NET_SAVE_NAME = "net1";
        private static string OPPONENT_SAVE_NAME = "opponent";
        private static Modes FourMode = Modes.Quick;
        private static Players BotTurn = Players.Player1;
        public static GameTime gameTime { get; set; }

        public static bool StartFourInARow(GraphicsDevice GraphicsDevice)
        {
            int[] args = { 7, 6, (int)FourMode }; // Arguments to pass into the game
            Game1.Screen.SetGUI(PresetGuis.InGame);
            FourInARowControl fourInARowControl = new FourInARowControl((int)Priorities.Game);
            fourInARowControl.AttachBot(new DQN(false), BotTurn);
            Game1.Screen.AddControl(fourInARowControl, args);
            Game1.Screen.Start(GraphicsDevice);
            return true;
        }

        public static bool StartTicTac(GraphicsDevice GraphicsDevice)
        {
            int[] args = { 3, 3, 3 }; // Arguments to pass into the game
            Game1.Screen.SetGUI(PresetGuis.InGame);
            TicTacControl ticTacControl = new TicTacControl((int)Priorities.Game);
            ticTacControl.AttachBot(new DQN(false), BotTurn);
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

        public static bool StartLearn()
        {
            foreach(ControlBase control in Game1.Screen.controls.Keys)
            {
                if (control.IsLearnable)
                {
                    GameControlBase CastControl = (GameControlBase)control;
                    CastControl.StartLearn();
                    return true;
                }
            }
            return false;
        }

        internal static bool LoadOpponent()
        {
            foreach (ControlBase control in Game1.Screen.controls.Keys)
            {
                if (control.IsLearnable)
                {
                    GameControlBase CastControl = (GameControlBase)control;
                    LearningBot bot = NetworkLoader.LoadLearningBot(OPPONENT_SAVE_NAME);
                    bot.Setup(CastControl, bot.BotTurn);
                    CastControl.SetOpponent(bot);
                    return true;
                }
            }
            return false;
        }

        public static bool StopLearn()
        {
            foreach (ControlBase control in Game1.Screen.controls.Keys)
            {
                if (control.IsLearnable)
                {
                    GameControlBase CastControl = (GameControlBase)control;
                    CastControl.StopLearn();
                    return true;
                }
            }
            return false;
        }

        public static bool LoadBot()
        {
            foreach (ControlBase control in Game1.Screen.controls.Keys)
            {
                if (control.IsLearnable)
                {
                    GameControlBase CastControl = (GameControlBase)control;
                    LearningBot bot = NetworkLoader.LoadLearningBot(NET_SAVE_NAME);
                    bot.Setup(CastControl, bot.BotTurn);
                    CastControl.SetBot(bot);
                    return true;
                }
            }
            return false;
        }

        public static bool SaveBot()
        {
            foreach (ControlBase control in Game1.Screen.controls.Keys)
            {
                if (control.IsLearnable)
                {
                    GameControlBase CastControl = (GameControlBase)control;
                    LearningBot bot = CastControl.GetBot();
                    NetworkLoader.SaveLearningBot(NET_SAVE_NAME, bot);
                    return true;
                }
            }
            return false;
        }

        public static bool SetFourMode()
        {
            if (FourMode == Modes.Quick)
            {
                FourMode = Modes.Graphical;
                Game1.Screen.GetGUI().ShowNotification("Four in a row mode set to Graphical", new Vector2(300, 300), 1000, gameTime);
                return true;
            }
            if (FourMode == Modes.Graphical)
            {
                FourMode = Modes.Quick;
                Game1.Screen.GetGUI().ShowNotification("Four in a row mode set to Quick", new Vector2(300, 300), 1000, gameTime);
                return true;
            }
            return false;

        }

        public static bool StartSettings()
        {
            Game1.Screen.Clear();
            Game1.Screen.SetGUI(PresetGuis.Settings);
            return true;
        }

        public static bool SetSaveName(string newName)
        {
            NET_SAVE_NAME = newName;
            Game1.Screen.GetGUI().ShowNotification("Bot save/load name set to " + newName, new Vector2(300, 300), 1000, gameTime);
            return true;
        }

        public static bool SetOpponentName(string newName)
        {
            OPPONENT_SAVE_NAME = newName;
            Game1.Screen.GetGUI().ShowNotification("Opponent save/load name set to " + newName, new Vector2(300, 300), 1000, gameTime);
            return true;
        }

        public static bool ChangePlayerTurn()
        {
            if (BotTurn == Players.Player1)
            {
                BotTurn = Players.Player2;
                Game1.Screen.GetGUI().ShowNotification("Player's turn changed to Player1", new Vector2(300, 300), 1000, gameTime);
            }
            else
            {
                BotTurn = Players.Player1;
                Game1.Screen.GetGUI().ShowNotification("Player's turn changed to Player2", new Vector2(300, 300), 1000, gameTime);
            }
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
            NetworkVectors net2 = new NetworkVectors(new List<int>() { 3, 3, 2 });
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
            Commands.gameTime = gameTime;
            Mousey.Update();
            KeyBoardey.Update();
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
