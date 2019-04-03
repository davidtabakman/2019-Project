using System.Collections.Generic;
using System.Threading;
using Controller;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameCenter
{
    public class Screen
    {
        public Dictionary<ControlBase, int[]> controls { get; private set; }
        private GUIControl GUIControl;

        public Screen()
        {
            controls = new Dictionary<ControlBase, int[]>();
        }

        public void AddControl(ControlBase control, int[] args)
        {
            controls.Add(control, args);
        }

        public void SetGUI(GUIControl gui)
        {
            if (GUIControl != null)
            {
                controls.Remove(GUIControl);
            }
            GUIControl = gui;
            controls.Add(gui, new int[] { });
        }

        public GUIControl GetGUI()
        {
            return GUIControl;
        }

        public void Start(GraphicsDevice gd)
        {
            
            foreach (ControlBase control in controls.Keys)
            {
                if (control != GUIControl)
                    control.Start(gd, controls[control]);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (ControlBase control in controls.Keys)
            {
                control.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ControlBase control in controls.Keys)
            {
                if (control != GUIControl)
                    control.Draw(spriteBatch);
            }
            GUIControl.Draw(spriteBatch);
        }

        public void Clear()
        {
            foreach (ControlBase control in controls.Keys)
            {
                if (control != GUIControl)
                    control.Clear();
            }
            controls = new Dictionary<ControlBase, int[]>();
            GUIControl = null;
        }
    }
}