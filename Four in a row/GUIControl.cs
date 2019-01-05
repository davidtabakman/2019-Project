using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Controller;
using GameCenter;
using Learning;

namespace GUI
{
    public class GUIControl : ControlBase
    {
        public LinkedList<GUIBase> GUIList;
        public Dictionary<string, int> GUIIDs;
        private int CountItems;

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

        public override void HandleClick(Vector2 Position)
        {
            if (Game1.GUIControl.GUIList.Count > 0)
            {
                LinkedListNode<GUIBase> currNode = Game1.GUIControl.GUIList.First;
                LinkedListNode<GUIBase> nextNode;
                while (currNode != null)
                {
                    nextNode = currNode.Next;
                    currNode.Value.ClickedOn(Position);
                    currNode = nextNode;
                }
            }
        }
    }
}