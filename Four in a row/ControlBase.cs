using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Learning;
using GameCenter;
using Controller;

namespace Controller
{
    /// <summary>
    /// Base class of all controls, can be updated, drew, started and handle click events
    /// </summary>
    public abstract class ControlBase
    {
        public bool Running;
        public bool IsLearnable { get; protected set; }
        public int StateNum { get; protected set; }
        public int ActionNum { get; protected set; }
        public int ClickPriority { get; set; } // The higher this is, the lower the priority

        /// <summary>
        /// Create a <c>ControlBase</c> class
        /// </summary>
        /// <param name="ClickPriority">// The higher this is, the lower the priority</param>
        public ControlBase(int ClickPriority)
        {
            IsLearnable = false;
            this.ClickPriority = ClickPriority;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Start(GraphicsDevice gd, int[] args);
        public abstract bool HandleClick(Vector2 position);
        public abstract bool HandleKeyPress(Keys key);
        public abstract void Clear();

    }
}