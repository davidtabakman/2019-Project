using Controller;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCenter;
using Helper;
using System.Collections.Generic;
using System;
using Learning;
using System.Threading;
using Microsoft.Xna.Framework.Input;

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
        private LearningBot bot;
        private LearningBot opponent;
        private Players BotPlayer;

        /// <summary>
        /// Create a Tic Tac Toe control class that will draw, update etc.
        /// </summary>
        /// <param name="ClickPriority">The lower this number is, the higher the priority is.</param>
        public TicTacControl(int ClickPriority) : base(ClickPriority)
        {
           
        }
        /// <summary>
        /// Attach the bot to the control, after this StartLearn can be called.
        /// </summary>
        public void AttachBot(LearningBot botAttach, Players botPlayer)
        {
            bot = botAttach;
            BotPlayer = botPlayer;
        }

        /// <summary>
        /// Start learning with the bot.
        /// </summary>
        private void StartBot()
        {
            if (!bot.IsSetup)        
                bot.Setup(this, BotPlayer);
            bot.Learn(200000, 0.05f, 0.0005f, 0.9f, 0.5f, opponent);
            Restart();
        }

        /// <summary>
        /// Trasfer the ID integer of a state into a board
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private Players[,] StateToTiles(State state)
        {
            int stateID = state.ID;
            int stateIDInBase3 = 0;
            int power = 0;
            int length = 0;
            // State from base 10 to base 3
            while (stateID != 0)
            {
                stateIDInBase3 += stateID % 3 * (int)Math.Pow(10, power);
                stateID /= 3;
                length++;
                power++;
            }
            // From base 3 to the board
            Players[,] tmpTiles = new Players[ColNum, RowNum];
            int x = 0;
            int y = 0;
            while(stateIDInBase3 != 0)
            {
                tmpTiles[x, y] = (Players)(stateIDInBase3 % 10);
                stateIDInBase3 /= 10;
                y++;
                if (y > RowNum)
                {
                    y = 0;
                    x++;
                }
            }
            while (x < ColNum)
            {
                tmpTiles[x, y] = Players.NoPlayer;
                y++;
                if (y > RowNum)
                {
                    y = 0;
                    x++;
                }
            }
            return tmpTiles;
        }

        /// <summary>
        /// Start the tic tac toe game. 
        /// </summary>
        /// <param name="args">Game parameters: Number of collumns, number of rows, how many in a row is needed to win</param>
        public override void Start(GraphicsDevice gd, int[] args)
        {
            opponent = null;
            RowNum = args[0];
            ColNum = args[1];
            ToWin = args[2];
            FeatureNum = ColNum * RowNum * 3; // Every tile has three features - isplayer1? isplayer2? isnoplayer?
            ActionNum = RowNum * ColNum;
            StateNum = (int)Math.Pow(3, RowNum * ColNum);
            float deltaX = Game1.w_width / ColNum;
            float deltaY = Game1.w_height / RowNum;
            // Create textures for drawing
            Board = CreateBoard(gd);
            Circle = ShapeCreator.CreateHollowCircle(gd, deltaX, deltaY, 5);
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
        }

        /// <summary>
        /// Create the board of the tic tac toe game
        /// </summary>
        /// <param name="gd"></param>
        /// <returns></returns>
        private Texture2D CreateBoard(GraphicsDevice gd)
        {
            RenderTarget2D background = new RenderTarget2D(gd, Game1.w_width, Game1.w_height);
            RenderTargetBinding[] last = gd.GetRenderTargets();
            gd.SetRenderTarget(background);
            SpriteBatch tmp = new SpriteBatch(gd);
            tmp.Begin();

            //Draw background:
            tmp.Draw(ShapeCreator.Rect(gd, Color.White, Game1.w_width, Game1.w_height), Vector2.Zero, Color.White);

            //Draw Collumns:
            float deltaX = Game1.w_width / ColNum;
            for (float i = deltaX; i < Game1.w_width; i += deltaX)
                tmp.Draw(ShapeCreator.Rect(gd, Color.Black, 1, Game1.w_height), new Vector2(i, 0), Color.White);

            //Draw rows:
            float deltaY = Game1.w_height / RowNum;
            for (float i = deltaY; i < Game1.w_height; i += deltaY)
                tmp.Draw(ShapeCreator.Rect(gd, Color.Black, Game1.w_width, 1), new Vector2(0, i), Color.White);

            tmp.End();
            gd.SetRenderTargets(last);
            return background;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Board, Vector2.Zero, Color.White);
            float deltaX = Game1.w_width / ColNum;
            float deltaY = Game1.w_height / RowNum;

            // Go over all tiles and draw either circles or Xs
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
            return;
        }

        /// <summary>
        /// Add a new object (either X or O) according to CurrTurn
        /// </summary>
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

        /// <summary>
        /// Function called on left click release
        /// </summary>
        public override bool HandleClick(Vector2 position)
        {
            // If a bot is loaded, play against it
            if (bot != null && bot.IsLearning)
                return false;
            AddObject(position);
            if (IsTerminalState())
                Clean();
            if (bot != null && CurrTurn == bot.BotTurn)
                bot.TakeAction(this, GetState());
            if (IsTerminalState())
                Clean();
            return true;
        }

        /// <summary>
        /// Return which player (player1, player2, no player) has won in the current game state.
        /// </summary>
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

        /// <summary>
        /// To be called when the exiting the game. Disposes all the textures
        /// </summary>
        public override void Clear()
        {
            Board.Dispose();
            Circle.Dispose();
            X.Dispose();
            if(bot != null)
                bot.Stop();
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
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

            if (bot != null && !bot.IsLearning && bot.BotTurn == Players.Player1)
                bot.TakeAction(this, GetState());
        }

        /// <summary>
        /// Get the state of the game. It is encoded in a way that every state has just one ID.
        /// </summary>
        public override State GetState()
        {
            int stateID = 0;
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum; y++)
                {
                    
                    stateID += (int)Tiles[x, y] * (int)Math.Pow(3, x * 3 + y );
                }
            }
            return new State(stateID, Tiles);
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

        /// <summary>
        /// Execute the action and pass to the next turn
        /// </summary>
        public override void DoAction(Actione action)
        {

            int addY = action.ID / ColNum;
            int addX = action.ID % ColNum;

            if (Tiles[addX, addY] == Players.NoPlayer)
                Tiles[addX, addY] = CurrTurn;

            NextTurn();
        }

        /// <summary>
        /// Change the current player to the other player
        /// </summary>
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


        public override double GetReward(Players forPlayer)
        {

            if (CheckWin() == forPlayer)
            {
                return 5;
            }
            else if (CheckWin() != Players.NoPlayer)
            {
                return -5;
            }
            else if (IsTerminalState())
            {
                return -2.5;
            }
            else
                return 0;
        }

        /// <summary>
        /// Makes the control ready for another move
        /// </summary>
        public override void Clean()
        {
            if (IsTerminalState())
            {
                Restart();
            }
        }

        /// <summary>
        /// Check if an action is legal in the current game state
        /// </summary>
        public override bool IsLegalAction(Actione action)
        {
            int addY = action.ID / ColNum;
            int addX = action.ID % ColNum;

            if (Tiles[addX, addY] == Players.NoPlayer)
                return true;
            return false;
        }

        public override bool IsTerminalState(State s)
        {
            Players[,] tempTiles = new Players[Tiles.GetLength(0), Tiles.GetLength(1)];
            for(int x = 0; x < Tiles.GetLength(0); x++)
            {
                for(int y = 0; y < Tiles.GetLength(1); y++)
                {
                    tempTiles[x, y] = Tiles[x, y];
                }
            }
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    Tiles[x, y] = (Players)s.Board[x, y];
                }
            }
            bool isTerminal = IsTerminalState();
            Tiles = tempTiles;
            return isTerminal;
        }

        /// <summary>
        /// Start learning with the bot in a different thread
        /// </summary>
        public override void StartLearn()
        {
            if (!bot.IsLearning)
            {
                Restart();
                Thread thread = new Thread(
                new ThreadStart(StartBot));
                thread.Start();
            }
        }

        /// <summary>
        /// Stop learning with the bot
        /// </summary>
        public override void StopLearn()
        {
            if (bot.IsLearning)
            {
                bot.Stop();
                Restart();
            }
        }

        public override void SetBot(LearningBot bot)
        {
            this.bot = bot;
        }

        public override LearningBot GetBot()
        {
            return bot;
        }

        /// <summary>
        /// Handle a keyboard key press
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool HandleKeyPress(Keys key)
        {
            return false;
        }

        public override void SetOpponent(LearningBot bot)
        {
            opponent = bot;
        }
    }
}