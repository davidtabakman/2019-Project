using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Four_in_a_row
{
    public class LBReleasedEventArgs : EventArgs
    {
        // Where the mouse is pressed
        public Vector2 Position { get; }

        /// <summary>
        /// Creates an EventArgs for mouse pressed, containing a position
        /// </summary>
        /// <param name="pos"></param>
        public LBReleasedEventArgs(Vector2 pos)
        {
            Position = pos;
        }
    }

    public class Mousey
    {
        static private Vector2 Position;
        static public MouseState mouseState;
        static public MouseState previousState;

        static public void Update()
        {
            previousState = mouseState;
            mouseState = Mouse.GetState();
            Position.X = mouseState.X;
            Position.Y = mouseState.Y;
            // Check if a click occured, if yes, raise an event
            LBReleased();
        }

        static public bool LBPressed()
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
                return true;
            return false;
        }

        static public bool RBPressed()
        {
            if (mouseState.RightButton == ButtonState.Pressed)
                return true;
            return false;
        }

        static public bool LBReleased()
        {
            if (mouseState.LeftButton == ButtonState.Released && previousState.LeftButton == ButtonState.Pressed)
            {
                OnLBReleased();
                return true;
            }
            return false;
        }

        static private void OnLBReleased()
        {
            // Call the delegate event function
            LeftButtonReleased(null, new LBReleasedEventArgs(Position));
        }

        public static event EventHandler<LBReleasedEventArgs> LeftButtonReleased;
    }
}