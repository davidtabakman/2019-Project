using Microsoft.Xna.Framework;
using Helper;
using Microsoft.Xna.Framework.Graphics;
using GameCenter;
using System;

namespace GUI
{
    /// <summary>
    /// Base class of any GUI object (such as buttons)
    /// </summary>
    public abstract class GUIBase
    {
        public int ID { get; }
        public bool IsVisible { get; set; }

        public GUIBase(int id, bool IsVisible)
        {
            ID = id;
            this.IsVisible = IsVisible;
        }

        public abstract void Draw(SpriteBatch sb);
        public abstract bool ClickedOn(Vector2 position);
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
                if (Bounds.Contains(new Point((int)position.X, (int)position.Y)))
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
    }
}