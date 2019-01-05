using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Helper;

namespace Four_in_a_row
{
    public class CircleObject : DrawableObject
    {
        /// <summary>
        /// Create a physical moving circle for the Four in a Row game
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="color"></param>
        /// <param name="gd"></param>
        /// <param name="radius"></param>
        public CircleObject(Vector2 Location, Color color, GraphicsDevice gd, int radius) : base(Location, ShapeCreator.circle(gd, color, radius))
        {
            Frozen = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObjects">List of all other game objects in current game</param>
        public override void Update(LinkedList<GameObject> gameObjects)
        {
            ApplyPhysics(gameObjects);
            base.Update(gameObjects);
        }

        /// <summary>
        /// Apply gravity, if collides with someone, freeze
        /// </summary>
        /// <param name="gameObjects">List of all other game objects for collision checking</param>
        public void ApplyPhysics(LinkedList<GameObject> gameObjects)
        {
            if (!Frozen)
            {
                Physics.ApplyGravity(this);
                foreach (GameObject go in gameObjects)
                {
                    // Check for collision, if collides set the location to be adjacent on the Y axis (above) and freeze
                    if (go != this && Collides(new Rectangle(new Point((int)go.Location.X - (int)Speed.X, (int)go.Location.Y - (int)Speed.Y), new Point((int)go.Size.X, (int)go.Size.Y))))
                    {
                        Location = new Vector2(Location.X, Location.Y - (Location.Y + Size.Y - go.Location.Y));
                        Speed = Vector2.Zero;
                        Freeze();
                    }
                }
            }
        }

        public override bool isPhysical()
        {
            return true;
        }
        
    }
}