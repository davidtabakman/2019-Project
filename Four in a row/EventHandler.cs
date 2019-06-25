using System;
using Controller;
using GameCenter;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using static Helper.Helper;
using Microsoft.Xna.Framework.Input;

namespace Four_in_a_row
{
    public class EventHandle
    {
        private static Game game { get; set; }
        private static ByPriority ByPriority; // A comparer class to compare priorities of different controls

        /// <summary>
        /// Initialize the event handler.
        /// </summary>
        /// <param name="game">The central game class in XNA.</param>
        public static void InitEventHandle(Game game)
        {
            EventHandle.game = game;
            // Subscribe to the LeftButtonReleased event
            Mousey.LeftButtonReleased += LBReleasedHandle;
            KeyBoardey.KeyboardPressed += KeyBoardReleaseHandle;
            ByPriority = new ByPriority();
        }

        /// <summary>
        /// Handle the event of left mouse button release.
        /// </summary>
        private static void LBReleasedHandle(object sender, LBReleasedEventArgs eventArgs)
        {
            Vector2 position = eventArgs.Position;
            if (game.IsActive && position.X > 0 && position.X < Game1.w_width && position.Y > 0 && position.Y < Game1.w_height) // If the click is in the game screen
            {
                // Go over all the control sorted by their priority until one of the controls claims the click or no controls are left
                List<ControlBase> controls = Game1.Screen.controls.Keys.ToList();
                controls.Sort(ByPriority);
                ControlBase ClaimedControl= null;
                foreach (var control in controls)
                {
                    if (control.Running)
                        if (control.HandleClick(eventArgs.Position)) {
                            ClaimedControl = control;
                            break;
                        }
                }
                // If the click has been claimed, go over all the other controls with the same prority and send them the click aswell
                if(ClaimedControl != null)
                {
                    foreach (var control in controls)
                    {
                        if (control != ClaimedControl && control.ClickPriority == ClaimedControl.ClickPriority)
                        {
                            control.HandleClick(eventArgs.Position);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle of keyboard key release event
        /// </summary>
        private static void KeyBoardReleaseHandle(object sender, KeyReleaseEventArgs eventArgs)
        {
            Keys key = eventArgs.PressedKey;
            if (game.IsActive)
            {
                // Go over all the controls and send them the button press event
                List<ControlBase> controls = Game1.Screen.controls.Keys.ToList();
                controls.Sort(ByPriority);

                foreach (var control in controls)
                {
                    if (control.Running)
                        if(control.HandleKeyPress(key))
                        {
                            break;
                        }
                }
            }

        }
    }
}