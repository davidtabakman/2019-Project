using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Four_in_a_row
{
    /// <summary>
    /// A game object that can be drawn.
    /// </summary>
    public class DrawableObject : GameObject
    {
        protected Texture2D Texture;

        /// <summary>
        /// Create a new Drawable Object.
        /// </summary>
        /// <param name="text">The drawable object's texture</param>
        public DrawableObject(Vector2 Location, Texture2D text) : 
            base(
                new Rectangle(new Point((int)Location.X, (int)Location.Y),
                text.Bounds.Size)
                )
        {
            Texture = text;
        }

        public override void Update(List<GameObject> gameObjects)
        {
            base.Update(gameObjects);
        }
        
        /// <summary>
        /// Draw the Object if its visible
        /// </summary>
        public override void Draw(SpriteBatch sb)
        {
            if (IsVisible)
                sb.Draw(Texture, Location, Color);
        }

        public override bool IsPhysical()
        {
            return false;
        }

        public override bool IsDrawable()
        {
            return false;
        }

        public override void Dispose()
        {
            Texture.Dispose();
        }
    }
}