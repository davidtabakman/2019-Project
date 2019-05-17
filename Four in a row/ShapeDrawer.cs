using GameCenter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Helper
{
    public class ShapeCreator
    {

        /// <summary>
        /// Return a texture2D of a rectangle
        /// </summary>
        public static Texture2D Rect(GraphicsDevice gd, Color color, int width, int height)
        {
            // Create a new texture
            RenderTarget2D boardTexture = new RenderTarget2D(gd, width, height);
            Color[] pixarr = new Color[width * height];
            for (int x = 0; x < width * height; x++)
            {
                pixarr[x] = color;
            }
            boardTexture.SetData(pixarr);
            return boardTexture;
        }

        /// <summary>
        /// Return a texture2D of a circle
        /// </summary>
        public static Texture2D Circle(GraphicsDevice gd, Color color, int radius)
        {
            // Create a new texture
            RenderTarget2D boardTexture = new RenderTarget2D(gd, radius * 2, radius * 2);
            Color[] pixarr = new Color[radius * radius * 4];

            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
                    // If the distance from the center is smaller than the radius, fill the pixel
                    float len = new Vector2(x - radius, y - radius).Length();
                    if (len <= radius)
                    {
                        pixarr[x * radius * 2 + y] = color;
                    }
                    else
                    {
                        pixarr[x * radius * 2 + y] = Color.Transparent;
                    }
                }
            }

            boardTexture.SetData(pixarr);
            return boardTexture;
        }

        /// <summary>
        /// Make a color on an image transparent
        /// </summary>
        public static void Transper(Texture2D text, Color toTranper)
        {
            // Just go over all the colors and make the fitting ones transparent
            Color[] pixarr = new Color[text.Width * text.Height];
            text.GetData(pixarr);
            for (int i = 0; i < text.Width * text.Height; i++)
            {
                if (pixarr[i] == toTranper)
                {
                    pixarr[i] = Color.Transparent;
                }
            }
            text.SetData(pixarr);
        }

        /// <summary>
        /// Create an X shape (like in tic tac toe)
        /// </summary>
        /// <param name="deltaX">The width</param>
        /// <param name="deltaY">The height</param>
        public static Texture2D CreateX(GraphicsDevice gd, float deltaX, float deltaY)
        {
            RenderTarget2D background = new RenderTarget2D(gd, (int)deltaX, (int)deltaY);
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(background);
            SpriteBatch tmp = new SpriteBatch(gd);
            tmp.Begin();
            tmp.Draw(Rect(gd, Color.White, (int)deltaX, (int)deltaY), Vector2.Zero, Color.White);

            int lineLen = (int)Math.Min(deltaX, deltaY) / 2;

            // Simple linear algebra to calculate the coordinates of the lines
            int toX = (int)(deltaX / 2 - Math.Cos(Math.PI / 4) * (lineLen / 2));
            int toY = (int)(deltaX / 2 - Math.Sin(Math.PI / 4) * (lineLen / 2));

            tmp.Draw(Rect(gd, Color.Black, 1, 1),
                new Rectangle(toX, toY, lineLen, 1),
                null,
                Color.Black,
                (float)(Math.PI / 4),
                new Vector2(0, 0),
                SpriteEffects.None,
                0);

            toX += (int)(Math.Cos(Math.PI / 4) * (lineLen / 2) * 2);

            tmp.Draw(Rect(gd, Color.Black, 1, 1),
                new Rectangle(toX, toY, lineLen, 1),
                null,
                Color.Black,
                (float)(3 * Math.PI / 4),
                new Vector2(0, 0),
                SpriteEffects.None,
                0);

            tmp.End();

            Transper(background, Color.White);

            gd.SetRenderTargets(last);
            return background;
        }

        /// <summary>
        /// Create a hollow circle
        /// </summary>
        /// <param name="deltaX">Width of the whole texture</param>
        /// <param name="deltaXbyWidth">deltaX divided by radius * 2</param>
        /// <param name="deltaY">Height</param>
        public static Texture2D CreateHollowCircle(GraphicsDevice gd, float deltaX, float deltaY, int deltaXbyWidth)
        {
            // Create a texture
            RenderTarget2D background = new RenderTarget2D(gd, (int)deltaX, (int)deltaY);
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(background);
            SpriteBatch tmp = new SpriteBatch(gd);
            tmp.Begin();
            tmp.Draw(Rect(gd, Color.White, (int)deltaX, (int)deltaY), Vector2.Zero, Color.White);

            int CircleRadius = (int)Math.Min(deltaX, deltaY) / deltaXbyWidth; 

            // Move the circle to the center of the texture
            int fixX = (int)deltaX / 2 - CircleRadius;
            int fixY = (int)deltaY / 2 - CircleRadius;

            // The size of the line
            int OutsideSize = 2;

            // Create a big circle and a smaller circle at the center of it
            tmp.Draw(Circle(gd, Color.Black, CircleRadius), new Vector2(fixX, fixY), Color.White);
            tmp.Draw(Circle(gd, Color.White, CircleRadius - OutsideSize / 2), new Vector2(fixX + OutsideSize / 2, fixY + OutsideSize / 2), Color.White);

            tmp.End();

            Transper(background, Color.White);

            gd.SetRenderTargets(last);
            return background;
        }
    }
}