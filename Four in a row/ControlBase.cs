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
    public abstract class ControlBase
    {
        public bool Running;
        public int StateNum { get; protected set; }
        public int ActionNum { get; protected set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Start(GraphicsDevice gd, int[] args);
        public abstract void HandleClick(Vector2 position);
        public abstract void Clear();
    }
}