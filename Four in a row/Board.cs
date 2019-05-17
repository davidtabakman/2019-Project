using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Helper;

namespace Four_in_a_row
{
    public class Board
    {
        protected Color background { get; } // The board color
        public int width { get; }
        public int height { get; }
        public RenderTarget2D boardTexture { get; } // Created in the constructor function
        public int circleRadius { get; } // Radius of a circle on the board
        private int RowNum { get; set; }
        private int ColNum { get; set; }

        /// <summary>
        /// Creates a four in a row board object
        /// </summary>
        /// <param name="gd">Graphics device to use</param>
        /// <param name="background">Color of the board</param>
        /// <param name="width">Board width</param>
        /// <param name="height">Board height</param>
        public Board(GraphicsDevice gd, Color background, int width, int height, int colNum, int rowNum)
        {
            RowNum = rowNum;
            ColNum = colNum;
            this.width = width;
            this.height = height;
            // The length of each tile, on the X and Y axis
            float deltaX = width / ColNum;
            float deltaY = height / RowNum;
            // Calculate the raius of circles on the board, it is set to be 4/10 of the length of the tile (on the shorter axis)
            circleRadius = (int)((4 * MathHelper.Min(deltaX, deltaY)) / 10);

            // Create the board texture manually (not a loaded image to allow various sizes of the window)
            boardTexture = new RenderTarget2D(gd, width, height);
            // Save the current target of the GraphicsDevice (Probably the window screen)
            RenderTargetBinding[] last = gd.GetRenderTargets();
            // Start drawing on the created board texture
            gd.SetRenderTarget(boardTexture);
            SpriteBatch sb = new SpriteBatch(gd);
            sb.Begin();
            // Fill the background with a white color
            sb.Draw(ShapeCreator.Rect(gd, Color.White, width, height), Vector2.Zero, Color.White);
            // Draw the to-be board
            sb.Draw(ShapeCreator.Rect(gd, background, width, height), new Vector2(0, deltaY), Color.White);
            DrawHoles(gd, sb);
            sb.End();
            // Make the white pixels transperent in order to create layering
            ShapeCreator.Transper(boardTexture, Color.White);

            // Return the graphics device to its previous state
            gd.SetRenderTargets(last);


        }

        /// <summary>
        /// Create holes on the board texture
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="sb"></param>
        private void DrawHoles(GraphicsDevice gd, SpriteBatch sb)
        {
            // The length of each tile, on the X and Y axis
            float deltaX = width / ColNum;
            float deltaY = height / RowNum;
            int radius = circleRadius;
            // Calculate the distance between the leftmost point on the circle to the left wall of the tile
            float seperationX = (deltaX - 2 * circleRadius) / 2;
            float seperationY = (deltaY - 2 * circleRadius) / 2;
            Texture2D circle = ShapeCreator.Circle(gd, Color.White, radius);
            for (float x = seperationX; x < width; x += deltaX)
            {
                for (float y = deltaY + seperationY; y < height; y += deltaY)
                {
                    sb.Draw(circle, new Vector2(x, y), Color.White);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(boardTexture, new Vector2(0, 0), Color.White);
        }

        public void Dispose()
        {
            boardTexture.Dispose();
        }
    }
}