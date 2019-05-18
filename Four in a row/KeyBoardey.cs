using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using System;

namespace Four_in_a_row
{
    public class KeyReleaseEventArgs : EventArgs
    {
        // Where a key is pressed
        public Keys PressedKey { get; }

        /// <summary>
        /// Creates an <c>EventArgs</c> for key press, containing a key that has been pressed
        /// </summary>
        public KeyReleaseEventArgs(Keys key)
        {
            PressedKey = key;
        }
    }

    public class KeyBoardey
    {
        static private Keys[] PressedKeys;
        static private Keys[] LastPressedKeys;
        static public KeyboardState keyboardState;
        static public KeyboardState previousState;

        static public void Update()
        {
            previousState = keyboardState;
            keyboardState = Keyboard.GetState();
            PressedKeys = keyboardState.GetPressedKeys();
            LastPressedKeys = previousState.GetPressedKeys();
            // Raise an event on all the keyboard keys that were press and now aren't.
            KeyPressed();
        }

        /// <summary>
        /// Raise an event with all the keyboard keys that were not pressed and now are. 
        /// </summary>
        private static void KeyPressed()
        {

            foreach (Keys key in PressedKeys)
            {
                if (!LastPressedKeys.Contains(key))
                {
                    OnKeyPressed(key);
                }
            }
            
        } 

        /// <summary>
        /// What happens when a key is pressed
        /// </summary>
        /// <param name="key"></param>
        private static void OnKeyPressed(Keys key)
        {
            KeyboardPressed(null, new KeyReleaseEventArgs(key));
        }

        public static event EventHandler<KeyReleaseEventArgs> KeyboardPressed;
    }
}