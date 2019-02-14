using System;
using Controller;
using GameCenter;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Four_in_a_row
{
    public class EventHandle
    {
        private static Game game { get; set; }

        public static void InitEventHandle(Game game)
        {
            EventHandle.game = game;
            // Subscribe to the LeftButtonReleased event
            Mousey.LeftButtonReleased += LBReleasedHandle;
        }

        private static void LBReleasedHandle(object sender, LBReleasedEventArgs eventArgs)
        {
            Vector2 position = eventArgs.Position;
            if (game.IsActive && position.X > 0 && position.X < Game1.w_width && position.Y > 0 && position.Y < Game1.w_height)
            {
                List<ControlBase> controls = Game1.Screen.controls.Keys.ToList();
                foreach (ControlBase control in controls)
                {
                    if (control.Running)
                        control.HandleClick(eventArgs.Position);
                }

            }
            
        }
    }
}