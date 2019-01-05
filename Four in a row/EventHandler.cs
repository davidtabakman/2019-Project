using System;
using GUI;
using GameCenter;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
                if (Game1.FourInARowControl.Running)
                    Game1.FourInARowControl.HandleClick(eventArgs.Position);

                if (Game1.TicTacControl.Running)
                    Game1.TicTacControl.HandleClick(eventArgs.Position);

                if (Game1.GUIControl.Running)
                    Game1.GUIControl.HandleClick(eventArgs.Position);
            }
            
        }
    }
}