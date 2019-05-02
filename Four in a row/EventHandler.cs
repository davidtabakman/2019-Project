using System;
using Controller;
using GameCenter;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using static Helper.Helper;

namespace Four_in_a_row
{
    public class EventHandle
    {
        private static Game game { get; set; }
        private static ByPriority ByPriority;

        public static void InitEventHandle(Game game)
        {
            EventHandle.game = game;
            // Subscribe to the LeftButtonReleased event
            Mousey.LeftButtonReleased += LBReleasedHandle;
            ByPriority = new ByPriority();
        }

        private static void LBReleasedHandle(object sender, LBReleasedEventArgs eventArgs)
        {
            Vector2 position = eventArgs.Position;
            if (game.IsActive && position.X > 0 && position.X < Game1.w_width && position.Y > 0 && position.Y < Game1.w_height)
            {
                List<ControlBase> controls = Game1.Screen.controls.Keys.ToList();
                controls.Sort(ByPriority);
                foreach (ControlBase control in controls)
                {
                    if (control.Running)
                        if (control.HandleClick(eventArgs.Position))
                            break;
                }

            }
            
        }
    }
}