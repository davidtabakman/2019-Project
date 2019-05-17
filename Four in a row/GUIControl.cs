using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Controller;
using GameCenter;
using Learning;

namespace GUI
{
    /// <summary>
    /// Common guis
    /// </summary>
    public class PresetGuis
    {
        private static GraphicsDevice gd { get; set; }

        public static GUIControl Menu = new GUIControl((int)Priorities.GUI);

        public static GUIControl InGame = new GUIControl((int)Priorities.GUI);

        private static void SetupMenu()
        {
            Menu.Start(gd, null);
            Menu.AddButton(gd, new Rectangle(0, 0, 120, 25), "Start 4 in a row", Color.Chocolate, "Four Start", new Func<bool>(() => Commands.StartFourInARow(gd)));
            Menu.AddButton(gd, new Rectangle(130, 0, 120, 25), "Start Tic Tac Tow", Color.Chocolate, "Tic Start", new Func<bool>(() => Commands.StartTicTac(gd)));
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
        }

        public static void Setup(GraphicsDevice graphics)
        {
            gd = graphics;

            SetupMenu();
            SetupInGame();
        }

        
    }

    public class GUIControl : ControlBase
    {
        public LinkedList<GUIBase> GUIList;
        public Dictionary<string, int> GUIIDs;
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
            foreach (GUIBase gb in GUIList)
                gb.Draw(spriteBatch);
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
    }
}