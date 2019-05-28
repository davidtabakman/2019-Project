using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Controller;
using GameCenter;
using Learning;
using Microsoft.Xna.Framework.Input;
using static GameCenter.Game1;

namespace GUI
{
    /// <summary>
    /// Common guis
    /// </summary>
    public static class PresetGuis
    {
        private static GraphicsDevice gd { get; set; }

        public static GUIControl Menu = new GUIControl((int)Priorities.GUI);

        public static GUIControl InGame = new GUIControl((int)Priorities.GUI);

        public static GUIControl Settings = new GUIControl((int)Priorities.GUI);

        private static void SetupMenu()
        {
            Menu.Start(gd, null);
            Menu.AddButton(gd, new Rectangle(0, 0, 120, 25), "Start 4 in a row", Color.Chocolate, "Four Start", new Func<bool>(() => Commands.StartFourInARow(gd)));
            Menu.AddButton(gd, new Rectangle(130, 0, 120, 25), "Start Tic Tac Tow", Color.Chocolate, "Tic Start", new Func<bool>(() => Commands.StartTicTac(gd)));
            Menu.AddButton(gd, new Rectangle(260, 0, 120, 25), "Settings", Color.Chocolate, "Settings", new Func<bool>(() => Commands.StartSettings()));
        }

        private static void SetupInGame()
        {
            InGame = new GUIControl((int)Priorities.GUI);
            InGame.Start(gd, new int[] { });
            InGame.AddButton(gd, new Rectangle(0, Game1.w_height - 25, 120, 25), "Exit", Color.Chocolate, "Exit", new Func<bool>(() => Commands.OnPressExit()));
            InGame.AddButton(gd, new Rectangle(130, Game1.w_height - 25, 120, 25), "Start Learning", Color.Chocolate, "Learn", new Func<bool>(() => Commands.StartLearn()));
            InGame.AddButton(gd, new Rectangle(260, Game1.w_height - 25, 120, 25), "Stop Learning", Color.Chocolate, "Stop", new Func<bool>(() => Commands.StopLearn()));
            InGame.AddButton(gd, new Rectangle(390, Game1.w_height - 25, 120, 25), "Save the bot", Color.Chocolate, "Save", new Func<bool>(() => Commands.SaveBot()));
            InGame.AddButton(gd, new Rectangle(520, Game1.w_height - 25, 120, 25), "Load the bot", Color.Chocolate, "Load", new Func<bool>(() => Commands.LoadBot()));
            InGame.AddButton(gd, new Rectangle(650, Game1.w_height - 25, 120, 25), "Load opponent", Color.Chocolate, "LoadOpp", new Func<bool>(() => Commands.LoadOpponent()));
            InGame.AddButton(gd, new Rectangle(780, Game1.w_height - 25, 120, 25), "Change enemy", Color.Chocolate, "ChangeOpp", new Func<bool>(() => Commands.ChangeOpponent()));
        }

        private static void SetupSettings()
        {
            Settings.Start(gd, new int[] { });
            Settings.AddButton(gd, new Rectangle(0, Game1.w_height - 25, 120, 25), "Exit", Color.Chocolate, "Exit", new Func<bool>(() => Commands.OnPressExit()));
            Settings.AddButton(gd, new Rectangle(130, Game1.w_height - 25, 250, 25), "Change Four in a Row mode", Color.Chocolate, "SetMode", new Func<bool>(() => Commands.SetFourMode()));
            Settings.AddButton(gd, new Rectangle(390, Game1.w_height - 25, 120, 25), "Change your turn", Color.Chocolate, "ChangeTurn", new Func<bool>(() => Commands.ChangePlayerTurn()));
            Settings.AddTextBox(gd, new Rectangle(100, 100, 300, 25), "", Color.Gray, Color.Chocolate, "BotNameTextbox", 30);
            Settings.AddButton(gd, new Rectangle(100, 130, 300, 25), "Set the load/save bot name", Color.Chocolate, "BotNameButton", new Func<bool>(() => Commands.SetSaveName(((TextBox)Settings.getGUI("BotNameTextbox")).GetText())));
            Settings.AddTextBox(gd, new Rectangle(450, 100, 300, 25), "", Color.Gray, Color.Chocolate, "OpponentNameTextbox", 30);
            Settings.AddButton(gd, new Rectangle(450, 130, 300, 25), "Set the load/save opponent name", Color.Chocolate, "OpponentNameButton", new Func<bool>(() => Commands.SetOpponentName(((TextBox)Settings.getGUI("OpponentNameTextbox")).GetText())));
        }

        public static void Setup(GraphicsDevice graphics)
        {
            gd = graphics;

            SetupMenu();
            SetupInGame();
            SetupSettings();
        }

        
    }

    public class GUIControl : ControlBase
    {
        private LinkedList<GUIBase> GUIList;
        private Dictionary<string, int> GUIIDs;
        private int CountItems;

        public GUIControl(int ClickPriority) : base(ClickPriority)
        {

        }

        /// <summary>
        /// Start the GUI control
        /// </summary>
        /// <param name="args">Doesn't do anything</param>
        public override void Start(GraphicsDevice gd, int[] args)
        {
            CountItems = 0;
            GUIList = new LinkedList<GUIBase>();
            GUIIDs = new Dictionary<string, int>();
            Running = true;
        }

        public override void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Add a new button to the GUI control
        /// </summary>
        /// <param name="bounds">Its location and size</param>
        /// <param name="text">Its text</param>
        /// <param name="color">Its color</param>
        /// <param name="onPress">Function to do on press</param>
        public void AddButton(GraphicsDevice gd, Rectangle bounds, string text, Color color, string name, Func<bool> onPress)
        {
            // Add a button to the gui list with the id of count of the items added throughout all the running time (means it will allways be unique)
            GUIList.AddLast(new Button(gd, bounds, text, color, CountItems, onPress, true));
            if (!GUIIDs.ContainsKey(name))
                GUIIDs[name] = CountItems;
            else
                throw new ArgumentException();
            CountItems++;
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            List<GUIBase> ToRemove = new List<GUIBase>();
            foreach (GUIBase gb in GUIList)
            {
                if (gb.IsAlive)
                    gb.Draw(spriteBatch);
                else
                    ToRemove.Add(gb);
            }
            foreach(GUIBase gb in ToRemove)
            {
                GUIList.Remove(gb);
            }
        }

        public void DeleteGUIElement(string name)
        {
            if (GUIIDs.ContainsKey(name)) {
                foreach (GUIBase gb in GUIList)
                    if (gb.ID == GUIIDs[name])
                    {
                        gb.Dispose();
                        GUIList.Remove(gb);
                        break;
                    }
            }
            else
                throw new ArgumentException();
            
        }

        public override void Clear()
        {
            foreach (string key in GUIIDs.Keys)
                DeleteGUIElement(key);
            GUIIDs.Clear();
        }

        public GUIBase getGUI(string guiName)
        {
            foreach (GUIBase gui in GUIList)
            {
                if (gui.ID == GUIIDs[guiName])
                    return gui;
            }
            return null;
        }

        /// <summary>
        /// Handle a click by transfering it to all the GUIs
        /// </summary>
        /// <param name="Position">Position of the click</param>
        /// <returns>Whether the click was claimed</returns>
        public override bool HandleClick(Vector2 Position)
        {
            if (GUIList.Count > 0)
            {
                LinkedListNode<GUIBase> currNode = GUIList.First;
                LinkedListNode<GUIBase> nextNode;
                // When one of the GUI elements claims the mouse press, return true
                while (currNode != null)
                {
                    nextNode = currNode.Next;
                    if (currNode.Value.ClickedOn(Position))
                        return true;
                    currNode = nextNode;
                }
            }
            return false;
        }

        /// <summary>
        /// Show a notification
        /// </summary>
        public void ShowNotification(string Message, Vector2 Location, double ShowTime, GameTime gameTime)
        {
            GUIList.AddLast(new Notification(Location, Message, ShowTime, gameTime, CountItems, Color.Chocolate));
            CountItems++;
        }

        /// <summary>
        /// Handle a press of a key on the keyboard
        /// </summary>
        /// <param name="key">The key pressed</param>
        public override bool HandleKeyPress(Keys key)
        {
            if (GUIList.Count > 0)
            {
                LinkedListNode<GUIBase> currNode = GUIList.First;
                LinkedListNode<GUIBase> nextNode;
                // When one of the GUI elements claims the mouse press, return true
                while (currNode != null)
                {
                    nextNode = currNode.Next;
                    if (currNode.Value.ButtonPressed(key))
                        return true;
                    currNode = nextNode;
                }
            }
            return false;
        }

        /// <summary>
        /// Add a textbox to the gui
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="bounds"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="name"></param>
        /// <param name="onPress"></param>
        public void AddTextBox(GraphicsDevice gd, Rectangle bounds, string text, Color color, Color textColor, string name, int maxChars)
        {
            // Add a textbox to the gui list with the id of count of the items added throughout all the running time (means it will allways be unique)
            GUIList.AddLast(new TextBox(gd, bounds, text, color, textColor, CountItems, true, maxChars));
            if (!GUIIDs.ContainsKey(name))
                GUIIDs[name] = CountItems;
            else
                throw new ArgumentException();
            CountItems++;
        }
    }
}