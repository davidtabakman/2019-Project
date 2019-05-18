using Microsoft.Xna.Framework;
using Helper;
using Microsoft.Xna.Framework.Graphics;
using GameCenter;
using System;
using Microsoft.Xna.Framework.Input;

namespace GUI
{
    /// <summary>
    /// Base class of any GUI object (such as buttons)
    /// </summary>
    public abstract class GUIBase
    {
        public int ID { get; }
        public bool IsVisible { get; set; }
        public bool IsAlive { get; protected set; }

        public GUIBase(int id, bool IsVisible)
        {
            ID = id;
            this.IsVisible = IsVisible;
        }

        public abstract void Draw(SpriteBatch sb);
        public abstract bool ClickedOn(Vector2 position);
        public abstract bool ButtonPressed(Keys key);
        /// <summary>
        /// Dispose of the textures
        /// </summary>
        public abstract void Dispose();
    }

    public class Button : GUIBase
    {

        private string Text { get; set; }
        private Rectangle Bounds { get; set; }
        private Color BackgroundColor { get; set; }
        private RenderTarget2D Texture { get; set; }
        private Func<bool> buttonAction; // Delegate of the function action

        /// <summary>
        /// Create a button.
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="bounds">Location and size</param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="id"></param>
        /// <param name="action">Function to execute when the button is clicked on</param>
        /// <param name="IsVisible"></param>
        public Button(GraphicsDevice gd, Rectangle bounds, string text, Color color, int id, Func<bool> action, bool IsVisible) : base(id, IsVisible)
        {
            BackgroundColor = color;

            Bounds = bounds;
            Text = text;
            
            Texture = new RenderTarget2D(gd, bounds.Width, bounds.Height);
            // Draw the button rectangle
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(Texture);
            SpriteBatch sb = new SpriteBatch(gd);
            sb.Begin();
            sb.Draw(ShapeCreator.Rect(gd, color, bounds.Width, bounds.Height), Vector2.Zero, Color.White);
            // Draw the text
            sb.DrawString(Game1.MainFont, Text, Vector2.Zero, Color.White);
            sb.End();
            gd.SetRenderTargets(last);
            buttonAction = action;

            IsAlive = true;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsVisible)
                sb.Draw(Texture, Bounds, Color.White);
        }

        /// <summary>
        /// Chech if the position is inside the button, if yes execute the button function.
        /// </summary>
        /// <returns>Was the function executed?</returns>
        public override bool ClickedOn(Vector2 position)
        {
            if (IsVisible)
                if (Bounds.Contains(position))
                {
                    buttonAction();
                    return true;
                }
            return false;
        }
        
        /// <summary>
        /// Dispose the texture
        /// </summary>
        public override void Dispose()
        {
            Texture.Dispose();
        }

        public override bool ButtonPressed(Keys key)
        {
            return false;
        }
    }

    public class Notification : GUIBase
    {
        private string Message;
        private GameTime gameTime;
        private Color Color;
        private Vector2 Location;
        private double StartTime;
        private double ShowTime;

        /// <summary>
        /// Create a notification
        /// </summary>
        /// <param name="Message">The notification's message</param>
        /// <param name="ShowTime">The time it will be shown in milliseconds</param>
        /// <param name="time">GameTime</param>
        public Notification(Vector2 Location, string Message, double ShowTime, GameTime time, int id, Color color) : base(id, true)
        {
            this.ShowTime = ShowTime;
            this.Message = Message;
            this.Location = Location;
            gameTime = time;
            Color = color;
            StartTime = gameTime.TotalGameTime.TotalMilliseconds;
            IsAlive = true;
        }

        public override bool ButtonPressed(Keys key)
        {
            return false;
        }

        public override bool ClickedOn(Vector2 position)
        {
            return false;
        }

        public override void Dispose()
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - StartTime > ShowTime)
                IsAlive = false;
            else
                sb.DrawString(Game1.MainFont, Message, Location, Color);
        }
    }


    public class TextBox : GUIBase
    {
        private string Text { get; set; }
        private Rectangle Bounds { get; set; }
        private Color BackgroundColor { get; set; }
        private Color TextColor { get; set; }
        private RenderTarget2D Texture { get; set; }
        private bool Focus;
        private int MaxLength;
        private bool Capital;

        public TextBox(GraphicsDevice gd, Rectangle bounds, string initText, Color color, Color TextColor, int id, bool IsVisible, int maxLength) : base(id, IsVisible)
        {
            // Initialize text box variables
            BackgroundColor = color;
            this.TextColor = TextColor;
            Capital = false;
            MaxLength = maxLength;

            Bounds = bounds;
            Text = initText;

            Texture = new RenderTarget2D(gd, bounds.Width, bounds.Height);
            // Draw the button rectangle
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(Texture);
            SpriteBatch sb = new SpriteBatch(gd);
            sb.Begin();
            sb.Draw(ShapeCreator.Rect(gd, color, bounds.Width, bounds.Height), Vector2.Zero, Color.White);
            sb.Draw(ShapeCreator.Rect(gd, Color.White, bounds.Width - 2, bounds.Height - 2), Vector2.One, Color.White);
            // Draw the text
            sb.End();
            gd.SetRenderTargets(last);

            IsAlive = true;
        }

        /// <summary>
        /// What the textbox does when it is click on
        /// </summary>
        /// <param name="position">Where the click occured</param>
        /// <returns>Whether the click was claimed</returns>
        public override bool ClickedOn(Vector2 position)
        {
            if (IsVisible)
            {
                if (Bounds.Contains(position))
                {
                    // If the click was on the textbox, make it take focus by claiming all future keyboard inputs. 
                    Focus = true;
                    return true;
                }
            }
            // If the click wasn't on the textbox, release the textbox and ignore future keyboard input
            Focus = false;
            return false;
        }

        public override void Dispose()
        {
            Texture.Dispose();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsVisible)
            {
                sb.Draw(Texture, Bounds, Color.White);
                sb.DrawString(Game1.MainFont, Text, Bounds.Location.ToVector2(), TextColor);
            }
        }

        public string GetText()
        {
            return Text;
        }

        public override bool ButtonPressed(Keys key)
        {
            if (Focus)
            {
                // Keys with no use, to be ingnored
                if (key == Keys.End || key == Keys.Enter || key == Keys.Down || key == Keys.Left || key == Keys.Right
                    || key == Keys.Up || key == Keys.PageDown || key == Keys.PageUp || key == Keys.Home || key == Keys.Insert
                    || key == Keys.LeftWindows || key == Keys.RightWindows || key == Keys.RightAlt || key == Keys.LeftAlt
                    || key == Keys.LeftControl || key == Keys.RightControl || key == Keys.Tab || key == Keys.LeftShift
                    || key == Keys.RightShift || key == Keys.NumLock || key == Keys.Apps || key == Keys.F1
                    || key == Keys.F2 || key == Keys.F3 || key == Keys.F4 || key == Keys.F5 || key == Keys.Escape
                    || key == Keys.F6 || key == Keys.F7 || key == Keys.F8 || key == Keys.F9 || key == Keys.F10
                    || key == Keys.F11 || key == Keys.F12 || key == Keys.PrintScreen || key == Keys.Pause || key == Keys.Scroll) return true;

                // Utility keys
                if (key == Keys.Back)
                {
                    BackSpace();
                    return true;
                }
                if (key == Keys.CapsLock)
                {
                    Capital = !Capital;
                    return true;
                }

                // Check if the text is at max characters
                if (Text.Length >= MaxLength)
                {
                    return true;
                }

                // Adding keys
                switch (key)
                {
                    case Keys.Subtract:
                    case Keys.OemMinus:
                        Text += "-";
                        break;
                    case Keys.Add:
                    case Keys.OemPlus:
                        Text += "+";
                        break;
                    case Keys.Multiply:
                        Text += "*";
                        break;
                    case Keys.Divide:
                        Text += "/";
                        break;
                    case Keys.Space:
                        Text += " ";
                        break;
                    case Keys.OemSemicolon:
                        Text += ";";
                        break;
                    case Keys.Decimal:
                    case Keys.OemPeriod:
                        Text += ".";
                        break;
                    case Keys.OemBackslash:
                        Text += "/";
                        break;
                    case Keys.OemComma:
                        Text += ",";
                        break;
                    case Keys.OemOpenBrackets:
                        Text += "[";
                        break;
                    case Keys.OemCloseBrackets:
                        Text += "]";
                        break;
                    case Keys.OemQuotes:
                        Text += "'";
                        break;
                    case Keys.OemPipe:
                        Text += "\\";
                        break;
                    case Keys.OemQuestion:
                        Text += "/";
                        break;
                    case Keys.OemTilde:
                        Text += "`";
                        break;
                    default:
                        RegularKey(key);
                        break;
                }
                return true;
            }
            return false;

        }

        // Helper functions for key switch:
        private void BackSpace()
        {
            string newString = "";
            for (int i = 0; i < Text.Length - 1; i++)
            {
                newString += Text[i];
            }
            Text = newString;
        }

        private void RegularKey(Keys key)
        {
            switch (key)
            {
                case Keys.D0:
                case Keys.NumPad0:
                    Text += "0";
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    Text += "1";
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    Text += "2";
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    Text += "3";
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    Text += "4";
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    Text += "5";
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    Text += "6";
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    Text += "7";
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    Text += "8";
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    Text += "9";
                    break;
                default:
                    // If approached here, the key is a letter.
                    // Capitalize it if need to
                    if (Capital)
                        Text += key.ToString();
                    else
                        Text += key.ToString().ToLower();
                    break;
            }
            
        }
    }
}