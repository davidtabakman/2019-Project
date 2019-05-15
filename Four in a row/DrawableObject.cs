using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Four_in_a_row
{
    public class DrawableObject : GameObject
    {
        protected Texture2D Texture;

        public DrawableObject(Vector2 Location, Texture2D text) : 
            base(
                new Rectangle(new Point((int)Location.X, (int)Location.Y),
                text.Bounds.Size)
                )
        {
            Texture = text;
        }

        public override void Update(LinkedList<GameObject> gameObjects)
        {
            base.Update(gameObjects);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsVisible)
                sb.Draw(Texture, Location, Color);
        }

        public override bool isPhysical()
        {
            return false;
        }

        public override bool isDrawable()
        {
            return false;
        }

        public override void Dispose()
        {
            Texture.Dispose();
        }
    }
}