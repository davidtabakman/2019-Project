using Microsoft.Xna.Framework;
using Helper;
using Microsoft.Xna.Framework.Graphics;
using GameCenter;
using System;

namespace GUI
{
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
        public abstract void Move();
        public abstract void ClickedOn(Vector2 position);
        public abstract void Dispose();
    }

    public class Button : GUIBase
    {

        private string Text { get; set; }
        private Rectangle Bounds { get; set; }
        private Color BackgroundColor { get; set; }
        private RenderTarget2D Texture { get; set; }
        private Func<bool> buttonAction;

        public Button(GraphicsDevice gd, Rectangle bounds, string text, Color color, int id, Func<bool> action, bool IsVisible) : base(id, IsVisible)
        {
            BackgroundColor = color;

            Bounds = bounds;
            Text = text;
            
            Texture = new RenderTarget2D(gd, bounds.Width, bounds.Height);
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(Texture);
            SpriteBatch sb = new SpriteBatch(gd);
            sb.Begin();
            sb.Draw(ShapeCreator.rectangle(gd, color, bounds.Width, bounds.Height), Vector2.Zero, Color.White);
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

        public override void Move()
        {
            throw new System.NotImplementedException();
        }

        public override void ClickedOn(Vector2 position)
        {
            if (IsVisible)
                if (Bounds.Contains(new Point((int)position.X, (int)position.Y)))
                {
                    buttonAction();
                }
        }

        public override void Dispose()
        {
            Texture.Dispose();
        }
    }
}