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
        private int ToWin;
        private Q_Learning bot;
        private bool IsLearning;

        public TicTacControl()
        {
            
        }

        private void StartBot()
        {
            IsLearning = true;
            bot = new Q_Learning(this, Players.Player2);
            bot.Learn(2000000, 0.1f, 0.001f, 0.1f, 0.9f);
            IsLearning = false;
            Restart();
        }

        private Players[,] StateToTiles(State state)
        {
            int stateID = state.ID;
            int stateIDInBase3 = 0;
            int power = 0;
            int length = 0;
            while (stateID != 0)
            {
                stateIDInBase3 += stateID % 3 * (int)Math.Pow(10, power);
                stateID /= 3;
                length++;
                power++;
            }
            Players[,] tmpTiles = new Players[ColNum, RowNum];
            int x = 0;
            int y = 0;
            while(stateIDInBase3 != 0)
            {
                tmpTiles[x, y] = (Players)(stateIDInBase3 % 10);
                stateIDInBase3 /= 10;
                y++;
                if (y > 2)
                {
                    y = 0;
                    x++;
                }
            }
            while (x < 3)
            {
                tmpTiles[x, y] = Players.NoPlayer;
                y++;
                if (y > 2)
                {
                    y = 0;
                    x++;
                }
            }
            return tmpTiles;
        }

        public override void Start(GraphicsDevice gd, int[] args)
        {
            RowNum = args[0];
            ColNum = args[1];
            ToWin = args[2];
            ActionNum = RowNum * ColNum;
            StateNum = (int)Math.Pow(3, RowNum * ColNum);
            float deltaX = Game1.w_width / ColNum;
            float deltaY = Game1.w_height / RowNum;
            Board = CreateBoard(gd);
            Circle = ShapeCreator.CreateHollowCircle(gd, deltaX, deltaY);
            X = ShapeCreator.CreateX(gd, deltaX, deltaY);
            Tiles = new Players[ColNum, RowNum];
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

            //Draw background:
            tmp.Draw(ShapeCreator.rectangle(gd, Color.White, Game1.w_width, Game1.w_height), Vector2.Zero, Color.White);

            //Draw Collumns:
            float deltaX = Game1.w_width / ColNum;
            for (float i = deltaX; i < Game1.w_width; i += deltaX)
                tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, 1, Game1.w_height), new Vector2(i, 0), Color.White);

            //Draw rows:
            float deltaY = Game1.w_height / RowNum;
            for (float i = deltaY; i < Game1.w_height; i += deltaY)
                tmp.Draw(ShapeCreator.rectangle(gd, Color.Black, Game1.w_width, 1), new Vector2(0, i), Color.White);

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
            {
                Tiles[addX, addY] = CurrTurn;
                NextTurn();
            }

            Clean();
        }

        public override void HandleClick(Vector2 position)
        { 
            AddObject(position);
            if (IsTerminalState())
                Clean();
            if (CurrTurn == bot.BotTurn)
                bot.TakeAction(this, GetState());
            if (IsTerminalState())
                Clean();
        }

        public override Players CheckWin()
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
                        if (count == ToWin-1)
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
                        if (count == ToWin-1)
                            return curr;
                    }
                }
            }
            count = 0;
            // Check wins diagnoally
            curr = Players.NoPlayer;
            for (int x = 0; x < ColNum - ToWin + 1; x++)
            {
                curr = Players.NoPlayer;
                for (int y = 0; y < RowNum - ToWin + 1; y++)
                {
                    curr = Tiles[x, y];
                    count = 0;
                    while (curr == Tiles[x + count, y + count] && Tiles[x + count, y + count] != Players.NoPlayer)
                    {
                        if (count == ToWin-1)
                            return curr;
                        count++;
                    }

                }
            }
            count = 0;
            // Check wins diagnoally the other way
            curr = Players.NoPlayer;
            for (int x = 0; x < ColNum - ToWin + 1; x++)
            {
                curr = Players.NoPlayer;
                for (int y = RowNum - 1; y >= ToWin - 1; y--)
                {
                    curr = Tiles[x, y];
                    count = 0;
                    while (curr == Tiles[x + count, y - count] && Tiles[x + count, y - count] != Players.NoPlayer)
                    {
                        if (count == ToWin-1)
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

            if (!IsLearning && bot.BotTurn == Players.Player1)
                bot.TakeAction(this, GetState());
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

            int addY = action.ID / ColNum;
            int addX = action.ID % ColNum;

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


        public override int GetReward(Players forPlayer)
        {

            if (CheckWin() == forPlayer)
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
                addX = rand.Next(ColNum);
                addY = rand.Next(RowNum);
            }
            Tiles[addX, addY] = CurrTurn;

            NextTurn();

        }

        public override void Clean()
        {
            if (IsTerminalState())
            {
                Restart();
            }

        }

        public override bool IsLegalAction(Actione action)
        {
            int addY = action.ID / ColNum;
            int addX = action.ID % ColNum;

            if (Tiles[addX, addY] == Players.NoPlayer)
                return true;
            return false;
        }
    }
}