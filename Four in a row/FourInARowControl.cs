using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GameCenter;
using Controller;
using Learning;

namespace Four_in_a_row
{
    public class FourInARowControl : GameControlBase
    {

        public FourInARowControl(int ClickPriority) : base(ClickPriority)
        {
        }

        // Game modes
        enum Modes
        {
            Quick, // No gravity but still paints everything
            Graphical, // Just for fun
            Learning // No UI at all
        }

        private Board board;
        private LinkedList<GameObject> ObjList; // Objects to draw
        private GraphicsDevice graphicsDevice { get; set; }
        private Players currTurn { get; set; }
        private Players[,] circleList; // Array for use in win checking and learning etc. Doesn't have anything to do with graphics
        private Dictionary<Players, Color> PlayerColor;
        private int RowNum { get; set; }
        private int ColNum { get; set; }
        private Modes Mode { get; set; }

        /// <summary>
        /// Start the four in a row game. Initializes the board and other variables
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="args">Game parameters: Number of collumns, number of rows, game mode (0 - Graphical, 1 - Quick). Has to be of length 3</param>
        public override void Start(GraphicsDevice gd, int[] args)
        {
            if (args[2] == 1)
                Mode = Modes.Quick;
            else if (args[2] == 0)
                Mode = Modes.Graphical;
            else
                Mode = Modes.Learning;

            RowNum = args[1] + 1; // 1 is added for future convinience
            ColNum = args[0];
            graphicsDevice = gd;
            Running = true;

            // First turn is of Player1
            currTurn = Players.Player1;

            // Initialize the logic board
            circleList = new Players[ColNum, RowNum-1];
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum - 1; y++)
                {
                    circleList[x, y] = Players.NoPlayer;
                }
            }

            // Initialize the objects-for-drawing/physics list
            ObjList = new LinkedList<GameObject>();

            // Add floor
            board = new Board(gd, Color.CadetBlue, Game1.w_width, Game1.w_height, ColNum, RowNum);
            int circleRadius = Math.Min(board.width / (ColNum * 2), board.height / (RowNum * 2));
            int fixY = 0;
            if (board.width / (ColNum * 2) < board.height / ((RowNum + 1) * 2))
            {
                fixY = (board.height / RowNum - circleRadius * 2) / 2;
            }
            ObjList.AddFirst(new GameObject(new Rectangle(0, Game1.w_height + fixY, Game1.w_width, 10)));

            PlayerToColorInit();
        }

        private void Restart(Modes mode)
        {
            for (int x = 0; x < circleList.GetLength(0); x++)
            {
                for (int y = 0; y < circleList.GetLength(1); y++)
                {
                    circleList[x, y] = Players.NoPlayer;
                }
            }
            Mode = mode;
            if (mode != Modes.Learning)
            {
                ObjList.Clear();
                int circleRadius = Math.Min(board.width / (ColNum * 2), board.height / (RowNum * 2));
                int fixY = 0;
                if (board.width / (ColNum * 2) < board.height / ((RowNum + 1) * 2))
                {
                    fixY = (board.height / RowNum - circleRadius * 2) / 2;
                }
                ObjList.AddFirst(new GameObject(new Rectangle(0, Game1.w_height + fixY, Game1.w_width, 10)));
            }
        }
        //Initialize the player-to-color dictionary
        private void PlayerToColorInit()
        {
            PlayerColor = new Dictionary<Players, Color>
            {
                [Players.Player1] = Color.Yellow,
                [Players.Player2] = Color.Red
            };
        }

        // Function called on left click release
        public override bool HandleClick(Vector2 position)
        {
            if (Mode != Modes.Learning)
            {
                AddCircle(position);
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            // Checks if a player has won the game in its current state
            foreach (GameObject c in ObjList)
            {
                c.Update(ObjList);
            }


        }

        public override void Draw(SpriteBatch sb)
        {
            if (Mode != Modes.Learning)
            {
                // Draw all the circles
                foreach (GameObject c in ObjList)
                {
                    c.Draw(sb);
                }
                // Draw the board
                board.Draw(sb);
                
            }

        }

        /// <summary>
        /// Function adds the circle to the logics matrix circleList
        /// </summary>
        /// <param name="collumn">The collumn to which the circle is appended</param>
        private void RegisterCircle(int collumn)
        {
            // Add the circle to the two dimensional array for checking the wins
            for (int y = 0; y < RowNum - 1; y++)
            {
                if (circleList[collumn, y] == Players.NoPlayer)
                {
                    circleList[collumn, y] = currTurn;
                    break;
                }
            }
        }

        /// <summary>
        /// Change the current player to the other player
        /// </summary>
        private void NextTurn()
        {
            if (currTurn == Players.Player1)
                currTurn = Players.Player2;
            else
                currTurn = Players.Player1;
        }

        private void QuickAdd(int addX, int circleRadius)
        {
            int fixX = (board.width / ColNum - circleRadius * 2) / 2;
            int fixY = (board.height / RowNum - circleRadius * 2) / 2;

            int deltaY = board.height / RowNum;
            int deltaX = board.width / ColNum;
            int addY = RowNum - 1;
            for (; addY > 0; addY--)
            {
                if (circleList[addX, RowNum - 1 - addY] == Players.NoPlayer)
                {
                    break;
                }
            }
            RegisterCircle(addX);

            int newY = deltaY * addY;
            int newX = deltaX * addX;
            Vector2 pos = Vector2.Zero;
            pos.X = newX + fixX;
            pos.Y = newY + fixY;
            ObjList.AddLast(new CircleObject(pos, PlayerColor[currTurn], graphicsDevice, circleRadius));
            ObjList.Last.Value.Freeze();
            NextTurn();
            
        }

        private void GraphicalAdd(int addX, Vector2 pos, int circleRadius)
        {
            int fixX = (board.width / ColNum - circleRadius * 2) / 2;

            bool add = true;
            foreach (GameObject c in ObjList)
            {
                if (c.Collides(new Rectangle((int)pos.X, (int)pos.Y, board.width / ColNum, (board.height / RowNum) + 1)))
                    add = false;
            }
            if (add)
            {
                RegisterCircle(addX);

                // Adding the circle object for graphics
                pos.X += fixX;
                ObjList.AddLast(new CircleObject(pos, PlayerColor[currTurn], graphicsDevice, circleRadius));
                ObjList.Last.Value.EditHitbox(new Vector2(board.width / ColNum - fixX, board.height / RowNum));
                NextTurn();

            }
        }

        /// <summary>
        /// Handle an addition of a circle at a specific position according to the board
        /// </summary>
        /// <param name="pos"></param>
        public void AddCircle(Vector2 pos)
        {
            // Line the circle up with the appropriate collumn
            pos = ClampCircle(pos);
            pos = new Vector2(pos.X, 0);
            
            // The collumn of the new circle
            int addX = (int)(pos.X / (board.width / ColNum));

            // Check if the collumn is full
            if (circleList[addX, RowNum-2] != Players.NoPlayer)
                return;

            int circleRadius = Math.Min(board.width / (ColNum * 2), board.height / ((RowNum + 1) * 2));

            // Add the circle according to the current mode
            if (Mode == Modes.Quick)
            {
                QuickAdd(addX, circleRadius);
            }
            else
            {
                GraphicalAdd(addX, pos, circleRadius);
            }

            // Check if someone won
            if (CheckWin() != Players.NoPlayer)
            {
                Console.WriteLine(CheckWin());
                Restart(Mode);
            }

        }

        /// <summary>
        /// Give the appropriate top left coordinates of a specific collumn that contains pos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 ClampCircle(Vector2 pos)
        {
            int deltaX = board.width / ColNum;
            int seperation = 0;
            int newX = ((int)pos.X / deltaX) * deltaX + seperation;
            int deltaY = board.height / RowNum;
            int newY = ((int)pos.Y / deltaY) * deltaY + seperation;
            return new Vector2(newX, newY);
        }

        public override Players CheckWin()
        {
            // Check wins horizontal
            Players curr = Players.NoPlayer;
            int count = 0;
            for (int y = 0; y < RowNum - 1; y++)
            {
                curr = Players.NoPlayer;
                for (int x = 0; x < ColNum; x++)
                {
                    if (curr != circleList[x, y] || circleList[x, y] == Players.NoPlayer)
                    {
                        curr = circleList[x, y];
                        count = 0;
                    }
                    else
                    {
                        count++;
                        if (count == 3)
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
                for (int y = 0; y < RowNum - 1; y++)
                {
                    if (curr != circleList[x, y] || circleList[x, y] == Players.NoPlayer)
                    {
                        curr = circleList[x, y];
                        count = 0;
                    }
                    else
                    {
                        count++;
                        if (count == 3)
                            return curr;
                    }
                }
            }
            count = 0;
            // Check wins diagnoally
            curr = Players.NoPlayer;
            for (int x = 0; x < ColNum - 3; x++)
            {
                curr = Players.NoPlayer;
                for (int y = 0; y < RowNum - 4; y++)
                {
                    curr = circleList[x, y];
                    count = 0;
                    while (curr == circleList[x + count, y + count] && circleList[x + count, y + count] != Players.NoPlayer)
                    {
                        if (count == 3)
                            return curr;
                        count++;
                    }
                    
                }
            }
            count = 0;
            // Check wins diagnoally the other way
            curr = Players.NoPlayer;
            for (int x = 0; x < ColNum - 3; x++)
            {
                curr = Players.NoPlayer;
                for (int y = RowNum-2; y > 2; y--)
                {
                    curr = circleList[x, y];
                    count = 0;
                    while (curr == circleList[x + count, y - count] && circleList[x + count, y - count] != Players.NoPlayer)
                    {
                        if (count == 3)
                            return curr;
                        count++;
                    }

                }
            }
            return Players.NoPlayer;
        }

        public override void Clear()
        {
            board.Dispose();
        }

        public override State GetState()
        {
            return new State(circleList);
        }

        public override double GetReward(Players forPlayer)
        {
            if (CheckWin() == forPlayer)
                return 1;
            else if (CheckWin() == Players.NoPlayer)
                return 0;
            else
                return -1;
        }

        public override bool IsLegalAction(Actione action)
        {
            if (circleList[action.ID, RowNum - 2] != Players.NoPlayer)
                return false;
            return true;
        }

        public override void DoAction(Actione action)
        {
            RegisterCircle(action.ID);
        }

        public override bool IsTerminalState()
        {
            if (NoMovesLeft())
                return true;
            else if (CheckWin() != Players.NoPlayer)
                return true;
            return false;
        }

        private bool NoMovesLeft()
        {
            for(int x = 0; x < ColNum; x++)
            {
                if (circleList[x, RowNum - 2] == Players.NoPlayer)
                    return false;
            }
            return true;
        }

        public override void Clean()
        {
            if (IsTerminalState())
                Restart(Mode);
        }

        public override bool IsTerminalState(State s)
        {
            throw new NotImplementedException();
        }

        public override void StartLearn()
        {
            throw new NotImplementedException();
        }

        public override void StopLearn()
        {
            throw new NotImplementedException();
        }

        public override void SetBot(LearningBot bot)
        {
            throw new NotImplementedException();
        }

        public override LearningBot GetBot()
        {
            throw new NotImplementedException();
        }
    }
}