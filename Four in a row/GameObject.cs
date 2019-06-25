using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Four_in_a_row
{
    /// <summary>
    /// An object in a game, has location, size speed etc.
    /// </summary>
    public class GameObject
    {
        public Vector2 Location { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Speed { get; set; }
        public Rectangle Bounds { get; set; }
        public bool Frozen { get; set; } // Can it move?
        public bool IsVisible { get; set; }
        public bool IsPhysical { get; set; }
        public Color Color { get; set; }

        /// <summary>
        /// Create a game object at a specific location and with a specific size
        /// </summary>
        public GameObject(Rectangle Bounds)
        {
            Location = new Vector2(Bounds.X, Bounds.Y);
            Frozen = true;
            Size = new Vector2(Bounds.Width, Bounds.Height);
            this.Bounds = Bounds;
        }

        /// <summary>
        /// Stop moving
        /// </summary>
        public void Freeze()
        {
            Frozen = true;
        }

        /// <summary>
        /// Move according to its speed
        /// </summary>
        public virtual void Update(List<GameObject> gameObjects)
        {
            Location += Speed;
            Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)Size.X, (int)Size.Y);
        }

        /// <summary>
        /// Change the size of the object (width, height)
        /// </summary>
        public void EditHitbox(Vector2 newSize)
        {
            Size = newSize;
        }

        public void SetLocation(float x, float y)
        {
            Location = new Vector2(x, y);
        }

        public virtual void Draw(SpriteBatch sb)
        {

        }

        public virtual void Move(Vector2 delta)
        {
            Location += delta;
        }

        /// <summary>
        /// Check if the object contains a point
        /// </summary>
        public virtual bool Contains(Vector2 Point)
        {
            if (Location.X < Point.X && Location.X + Size.X > Point.X)
            {
                if (Location.Y < Point.Y && Location.Y + Size.Y > Point.Y)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether or not the other <c>Rectangle</c> collides with this game object
        /// </summary>
        public virtual bool Collides(Vector2 location, Vector2 size)
        {
            if (Location.Y + Size.Y < location.Y
                || Location.Y > location.Y + size.Y)
            {
                return false;
            }
            if (Location.X + Size.X < location.X
                 || Location.X > location.X + size.X)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks whether or not the other <c>GameObject</c> collides with this game object
        /// </summary>
        public virtual bool Collides(GameObject with)
        {
            if(Collides(with.Location, with.Size))
                return true;
            return false;
        }

        public virtual bool IsDrawable()
        {
            return false;
        }

        public virtual void Dispose()
        {

        }

    }
}