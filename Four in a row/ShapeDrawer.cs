using GameCenter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Helper
{
    public class ShapeCreator
    {

        // Return a Texture2D to a rectangle
        public static Texture2D rectangle(GraphicsDevice gd, Color color, int width, int height)
        {
            RenderTarget2D boardTexture = new RenderTarget2D(gd, width, height);
            Color[] pixarr = new Color[width * height];
            for (int x = 0; x < width * height; x++)
            {
                pixarr[x] = color;
            }
            boardTexture.SetData(pixarr);
            return boardTexture;
        }

        //Return a Texture2D to a circle
        public static Texture2D circle(GraphicsDevice gd, Color color, int radius)
        {
            RenderTarget2D boardTexture = new RenderTarget2D(gd, radius * 2, radius * 2);
            Color[] pixarr = new Color[radius * radius * 4];

            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
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

        // Make a color on an image transperent
        public static void Transper(Texture2D text, Color toTranper)
        {
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

        public static Texture2D CreateX(GraphicsDevice gd, float deltaX, float deltaY)
        {
            RenderTarget2D background = new RenderTarget2D(gd, (int)deltaX, (int)deltaY);
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(background);
            SpriteBatch tmp = new SpriteBatch(gd);
            tmp.Begin();
            tmp.Draw(ShapeCreator.rectangle(gd, Color.White, (int)deltaX, (int)deltaY), Vector2.Zero, Color.White);

            int lineLen = (int)Math.Min(deltaX, deltaY) / 2;

            int toX = (int)(deltaX / 2 - Math.Cos(Math.PI / 4) * (lineLen / 2));
            int toY = (int)(deltaX / 2 - Math.Sin(Math.PI / 4) * (lineLen / 2));

            tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, 1, 1),
                new Rectangle(toX, toY, lineLen, 1),
                null,
                Color.Black,
                (float)(Math.PI / 4),
                new Vector2(0, 0),
                SpriteEffects.None,
                0);

            toX += (int)(Math.Cos(Math.PI / 4) * (lineLen / 2) * 2);

            tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, 1, 1),
                new Rectangle(toX, toY, lineLen, 1),
                null,
                Color.Black,
                (float)(3 * Math.PI / 4),
                new Vector2(0, 0),
                SpriteEffects.None,
                0);

            tmp.End();

            ShapeCreator.Transper(background, Color.White);

            gd.SetRenderTargets(last);
            return background;
        }

        public static Texture2D CreateHollowCircle(GraphicsDevice gd, float deltaX, float deltaY)
        {
            RenderTarget2D background = new RenderTarget2D(gd, (int)deltaX, (int)deltaY);
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(background);
            SpriteBatch tmp = new SpriteBatch(gd);
            tmp.Begin();
            tmp.Draw(ShapeCreator.rectangle(gd, Color.White, (int)deltaX, (int)deltaY), Vector2.Zero, Color.White);

            int CircleRadius = (int)Math.Min(deltaX, deltaY) / 5;

            int fixX = (int)deltaX / 2 - CircleRadius;
            int fixY = (int)deltaY / 2 - CircleRadius;

            int OutsideSize = 2;

            tmp.Draw(ShapeCreator.circle(gd, Color.Black, CircleRadius), new Vector2(fixX, fixY), Color.White);
            tmp.Draw(ShapeCreator.circle(gd, Color.White, CircleRadius - OutsideSize / 2), new Vector2(fixX + OutsideSize / 2, fixY + OutsideSize / 2), Color.White);

            tmp.End();

            ShapeCreator.Transper(background, Color.White);

            gd.SetRenderTargets(last);
            return background;
        }
    }
}