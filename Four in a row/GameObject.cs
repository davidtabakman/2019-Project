using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Four_in_a_row
{
    public class GameObject
    {
        public Vector2 Location { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Speed { get; set; }
        public Rectangle Bounds { get; set; }
        public bool Frozen { get; set; }

        public GameObject(Rectangle Bounds)
        {
            Location = new Vector2(Bounds.X, Bounds.Y);
            Frozen = true;
            Size = new Vector2(Bounds.Width, Bounds.Height);
            this.Bounds = Bounds;
        }

        public void Freeze()
        {
            Frozen = true;
        }

        public virtual void Update(LinkedList<GameObject> gameObjects)
        {
            Location += Speed;
            Bounds = new Rectangle((int)Location.X, (int)Location.Y, (int)Size.X, (int)Size.Y);
        }

        public void EditHitbox(Vector2 newSize)
        {
            Size = newSize;
        }

        public virtual void Draw(SpriteBatch sb)
        {

        }

        public virtual void Move(Vector2 delta)
        {
            Location += delta;
        }

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

        public virtual bool Collides(Rectangle with)
        {
            if (Bounds.Intersects(with))
                return true;
            return false;
        }

        public virtual bool Collides(GameObject with)
        {
            if(Collides(with.Bounds))
                return true;
            return false;
        }

        public virtual bool isPhysical()
        {
            return false;
        }

        public virtual bool isDrawable()
        {
            return false;
        }

    }
}