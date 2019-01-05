using Controller;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCenter;
using Helper;
using System.Collections.Generic;
using System;
using Learning;
using System.Threading;

namespace TicTacToe
{
    public class TicTacControl : GameControlBase
    {
        

        private Texture2D Board { get; set; }
        private Texture2D Circle { get; set; }
        private Texture2D X { get; set; }
        private Players[,] Tiles;
        private int RowNum;
        private int ColNum;
        public Players CurrTurn { get; private set; }
        private Q_Leaning bot;

        public TicTacControl()
        {
            
        }

        private void StartBot()
        {
            bot = new Q_Leaning(this, 10000);
        }

        public override void Start(GraphicsDevice gd, int[] args)
        {
            RowNum = 3;
            ColNum = 3;
            ActionNum = RowNum * ColNum;
            StateNum = (int)Math.Pow(3, RowNum * ColNum);
            float deltaX = Game1.w_width / ColNum;
            float deltaY = Game1.w_height / RowNum;
            Board = CreateBoard(gd);
            Circle = ShapeCreator.CreateHollowCircle(gd, deltaX, deltaY);
            X = ShapeCreator.CreateX(gd, deltaX, deltaY);
            Tiles = new Players[3, 3];
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum; y++)
                {
                    Tiles[x, y] = Players.NoPlayer;
                }
            }
            
            Running = true;
            CurrTurn = Players.Player1;

            Thread thread = new Thread(
            new ThreadStart(StartBot));
            thread.Start();
        }

        private Texture2D CreateBoard(GraphicsDevice gd)
        {
            RenderTarget2D background = new RenderTarget2D(gd, Game1.w_width, Game1.w_height);
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(background);
            SpriteBatch tmp = new SpriteBatch(gd);
            tmp.Begin();

            tmp.Draw(ShapeCreator.rectangle(gd, Color.White, Game1.w_width, Game1.w_height), Vector2.Zero, Color.White);
            tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, 1, Game1.w_height), new Vector2(Game1.w_width / 3, 0), Color.White);
            tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, 1, Game1.w_height), new Vector2(2 * Game1.w_width / 3, 0), Color.White);
            tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, Game1.w_width, 1), new Vector2(0, Game1.w_height / 3), Color.White);
            tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, Game1.w_width, 1), new Vector2(0, 2 * Game1.w_height / 3), Color.White);

            tmp.End();
            gd.SetRenderTargets(last);
            return background;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Board, Vector2.Zero, Color.White);
            float deltaX = Game1.w_width / ColNum;
            float deltaY = Game1.w_height / RowNum;

            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum; y++)
                {
                    if (Tiles[x, y] == Players.Player1)
                        spriteBatch.Draw(X, new Vector2(x * deltaX, y * deltaY), Color.White);
                    else if (Tiles[x, y] == Players.Player2)
                        spriteBatch.Draw(Circle, new Vector2(x * deltaX, y * deltaY), Color.White);

                }
            }
        }


        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        private void AddObject(Vector2 position)
        {
            float deltaX = Game1.w_width / ColNum;
            float deltaY = Game1.w_height / RowNum;

            int addX = (int)(position.X / deltaX);
            int addY = (int)(position.Y / deltaY);

            if (Tiles[addX, addY] == Players.NoPlayer)
                Tiles[addX, addY] = CurrTurn;

            NextTurn();

            Clean();
        }

        public override void HandleClick(Vector2 position)
        {
            AddObject(position);
            bot.TakeAciton(this, GetState());
        }

        private Players CheckWin()
        {
            // Check wins horizontal
            Players curr = Players.NoPlayer;
            int count = 0;
            for (int y = 0; y < RowNum; y++)
            {
                curr = Players.NoPlayer;

                for (int x = 0; x < ColNum; x++)
                {
                    if (curr != Tiles[x, y] || Tiles[x, y] == Players.NoPlayer)
                    {
                        curr = Tiles[x, y];
                        count = 0;
                    }
                    else
                    {
                        count++;
                        if (count == 2)
                            return curr;
                    }
                }
            }
            count = 0;
            // Check wins vertical
            curr = Players.NoPlayer;
            for (int x = 0; x < ColNum; x++)
            {
                curr = Players.NoPlayer;
                for (int y = 0; y < RowNum; y++)
                {
                    if (curr != Tiles[x, y] || Tiles[x, y] == Players.NoPlayer)
                    {
                        curr = Tiles[x, y];
                        count = 0;
                    }
                    else
                    {
                        count++;
                        if (count == 2)
                            return curr;
                    }
                }
            }
            count = 0;
            // Check wins diagnoally
            curr = Players.NoPlayer;
            for (int x = 0; x < ColNum - 2; x++)
            {
                curr = Players.NoPlayer;
                for (int y = 0; y < RowNum - 2; y++)
                {
                    curr = Tiles[x, y];
                    count = 0;
                    while (curr == Tiles[x + count, y + count] && Tiles[x + count, y + count] != Players.NoPlayer)
                    {
                        if (count == 2)
                            return curr;
                        count++;
                    }

                }
            }
            count = 0;
            // Check wins diagnoally the other way
            curr = Players.NoPlayer;
            for (int x = 0; x < ColNum - 2; x++)
            {
                curr = Players.NoPlayer;
                for (int y = RowNum - 1; y > 1; y--)
                {
                    curr = Tiles[x, y];
                    count = 0;
                    while (curr == Tiles[x + count, y - count] && Tiles[x + count, y - count] != Players.NoPlayer)
                    {
                        if (count == 2)
                            return curr;
                        count++;
                    }

                }
            }
            return Players.NoPlayer;
        }

        public override void Clear()
        {
            Board.Dispose();
            Circle.Dispose();
            X.Dispose();
        }

        public void Restart()
        {
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum; y++)
                {
                    Tiles[x, y] = Players.NoPlayer;
                }
            }
            CurrTurn = Players.Player1;
        }

        public override State GetState()
        {
            int stateID = 0;
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum; y++)
                {
                    stateID += (int)Tiles[x, y] * (int)Math.Pow(3, (x * 3 + y ));
                }
            }
            return new State(stateID);
        }

        public bool NoMovesLeft()
        {
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum; y++)
                {
                    if (Tiles[x, y] == Players.NoPlayer)
                        return false;
                }
            }
            return true;
        }

        public override void DoAction(Actione action)
        {

            int addX = action.ID / ColNum;
            int addY = action.ID % RowNum;

            if (Tiles[addX, addY] == Players.NoPlayer)
                Tiles[addX, addY] = CurrTurn;

            NextTurn();
        }

        private void NextTurn()
        {
            if (CurrTurn == Players.Player1)
                CurrTurn = Players.Player2;
            else
                CurrTurn = Players.Player1;
        }

        public override bool IsTerminalState()
        {
           
            if (CheckWin() == Players.Player1 || NoMovesLeft())
            {
                return true;
            }
            else if (CheckWin() == Players.NoPlayer)
                return false ;
            else
                return true;
        }


        public override int GetReward()
        {
            if (CheckWin() == CurrTurn)
            {
                return 10;
            }
            else if (CheckWin() == Players.NoPlayer)
                return 0;
            else
                return -10;
        }

        public void RandomAction()
        {
            Random rand = new Random();

            int addX = 0;
            int addY = 0;

            if (NoMovesLeft())
            {
                return;
            }

            while (Tiles[addX, addY] != Players.NoPlayer)
            {
                addX = rand.Next(3);
                addY = rand.Next(3);
            }
            Tiles[addX, addY] = CurrTurn;

            NextTurn();

        }

        public override void Clean()
        {
            Restart();
        }

        public override bool IsLegalAction(Actione action)
        {
            int addX = action.ID / ColNum;
            int addY = action.ID % RowNum;

            if (Tiles[addX, addY] == Players.NoPlayer)
                return true;
            return false;
        }
    }
}