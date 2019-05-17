using System.Collections.Generic;
using System.Threading;
using Controller;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameCenter
{
    /// <summary>
    /// A class that concetrates all that happens on the screen (controls etc.)
    /// </summary>
    public class Screen
    {
        public Dictionary<ControlBase, int[]> controls { get; private set; } // The controls with their arguments
        private GUIControl GUIControl; // Only one GUI control possible, needs to be treated differently sometimes

        /// <summary>
        /// Create a new screen.
        /// </summary>
        public Screen()
        {
            controls = new Dictionary<ControlBase, int[]>();
        }

        /// <summary>
        /// Add a control to the screen
        /// </summary>
        /// <param name="args">control's arguments</param>
        public void AddControl(ControlBase control, int[] args)
        {
            controls.Add(control, args);
        }

        public void SetGUI(GUIControl gui)
        {
            if (GUIControl != null)
            {
                controls.Remove(GUIControl); // Delete it from the controls dictionary
            }
            GUIControl = gui;
            controls.Add(gui, new int[] { });
        }

        public GUIControl GetGUI()
        {
            return GUIControl;
        }

        /// <summary>
        /// Start all the controls in the screen
        /// </summary>
        public void Start(GraphicsDevice gd)
        {
            foreach (ControlBase control in controls.Keys)
            {
                if (control != GUIControl)
                    control.Start(gd, controls[control]);
            }
        }

        /// <summary>
        /// Update all the controls in the screen
        /// </summary>
        public void Update(GameTime gameTime)
        {
            foreach (ControlBase control in controls.Keys)
            {
                control.Update(gameTime);
            }
        }

        /// <summary>
        /// Draw all the control in the screen
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ControlBase control in controls.Keys)
            {
                if (control != GUIControl)
                    control.Draw(spriteBatch);
            }
            // The gui will always be in the front
            GUIControl.Draw(spriteBatch);
        }

        /// <summary>
        /// Clear the screen (dispose the textures etc.)
        /// </summary>
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