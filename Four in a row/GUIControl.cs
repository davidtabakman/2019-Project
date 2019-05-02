﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Controller;
using GameCenter;
using Learning;

namespace GUI
{
    public class PresetGuis
    {
        private static GraphicsDevice gd { get; set; }

        public static GUIControl Menu = new GUIControl((int)Priorities.GUI);

        public static GUIControl InGame = new GUIControl((int)Priorities.GUI);

        private static void SetupMenu()
        {
            Menu.Start(gd, null);
            Menu.AddButton(gd, new Rectangle(0, 0, 120, 25), "Start 4 in a row", Color.Gray, "Four Start", new Func<bool>(() => Commands.StartFourInARow(gd)));
            Menu.AddButton(gd, new Rectangle(130, 0, 120, 25), "Start Tic Tac Tow", Color.Gray, "Tic Start", new Func<bool>(() => Commands.StartTicTac(gd)));
        }

        private static void SetupInGame()
        {
            InGame = new GUIControl((int)Priorities.GUI);
            InGame.Start(gd, new int[] { });
            InGame.AddButton(gd, new Rectangle(0, Game1.w_height - 25, 120, 25), "Exit", Color.Gray, "Exit", new Func<bool>(() => Commands.OnPressExit()));
            InGame.AddButton(gd, new Rectangle(130, Game1.w_height - 25, 120, 25), "Start Learning", Color.Gray, "Learn", new Func<bool>(() => Commands.StartLearn()));
            InGame.AddButton(gd, new Rectangle(260, Game1.w_height - 25, 120, 25), "Stop Learning", Color.Gray, "Stop", new Func<bool>(() => Commands.StopLearn()));
            InGame.AddButton(gd, new Rectangle(390, Game1.w_height - 25, 120, 25), "Save the bot", Color.Gray, "Save", new Func<bool>(() => Commands.SaveBot()));
            InGame.AddButton(gd, new Rectangle(520, Game1.w_height - 25, 120, 25), "Load the bot", Color.Gray, "Load", new Func<bool>(() => Commands.LoadBot()));
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

        public void AddButton(GraphicsDevice gd, Rectangle bounds, string text, Color color, string name, Func<bool> onPress)
        {
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

        public override bool HandleClick(Vector2 Position)
        {
            if (Game1.Screen.GetGUI().GUIList.Count > 0)
            {
                LinkedListNode<GUIBase> currNode = Game1.Screen.GetGUI().GUIList.First;
                LinkedListNode<GUIBase> nextNode;
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