using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GameCenter;
using Controller;
using Learning;
using System.Threading;
using System.Linq;
using static Controller.State;

namespace Four_in_a_row
{
    public class FourInARowControl : GameControlBase
    {
        /// <summary>
        /// Create a four in a row control class that will draw, update etc.
        /// </summary>
        /// <param name="ClickPriority">The lower this number is, the higher the priority is.</param>
        public FourInARowControl(int ClickPriority) : base(ClickPriority)
        {
        }

        // Game modes
        public enum Modes
        {
            Quick = 1, // No gravity but still paints everything
            Graphical = 0, // Just for fun
            Learning = 2 // No UI at all
        }

        private Board board;
        private Dictionary<string, List<GameObject>> ObjList; // Objects to draw
        private GraphicsDevice graphicsDevice;
        private Players[,] circleList; // Array for use in win checking and learning etc. Doesn't have anything to do with graphics
        private Dictionary<Players, Color> PlayerColor; // Colors of the players
        private int RowNum;
        private int ColNum;
        private Modes Mode;
        private int CircleRadius;
        private Bot Enemy;


        /// <summary>
        /// Start the four in a row game. Initializes the board and other variables
        /// </summary>
        /// <param name="args">Game parameters: Number of collumns, number of rows, game mode (0 - Graphical, 1 - Quick, 2 - Learning). Has to be of length 3</param>
        public override void Start(GraphicsDevice gd, int[] args)
        {
            if (args[2] == 1)
                Mode = Modes.Quick;
            else if (args[2] == 0)
                Mode = Modes.Graphical;
            else if (args[2] == 2)
                Mode = Modes.Learning;
            else
                throw new Exception("Invalid arguments");

            MinimaxBot enemy = new MinimaxBot();
            enemy.SetMaxDepth(8);
            Enemy = enemy;
            PlayAgainst = Against.Bot;

            RowNum = args[1] + 1; // 1 is added for future convinience
            ColNum = args[0];
            ActionNum = ColNum;
            graphicsDevice = gd;
            Running = true;

            // First turn is of Player1
            CurrTurn = Players.Player1;

            // Initialize the logics board
            circleList = new Players[ColNum, RowNum-1];
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 0; y < RowNum - 1; y++)
                {
                    circleList[x, y] = Players.NoPlayer;
                }
            }
            FeatureNum = ColNum * (RowNum - 1) * 3;

            // Initialize the Game Objects lists (by string names)
            ObjList = new Dictionary<string, List<GameObject>>();
            ObjList.Add("Drawable", new List<GameObject>());
            ObjList.Add("NonDrawable", new List<GameObject>());

            // Add floor
            board = new Board(gd, Color.CadetBlue, Game1.w_width, Game1.w_height, ColNum, RowNum);
            CircleRadius = Math.Min(board.width / (ColNum * 2), board.height / (RowNum * 2));
            int fixY = 0;
            if (board.width / (ColNum * 2) < board.height / ((RowNum + 1) * 2))
            {
                fixY = (board.height / RowNum - CircleRadius * 2) / 2;
            }
            GameObject floor = new GameObject(new Rectangle(0, Game1.w_height + fixY, Game1.w_width, 10));
            floor.IsPhysical = true;
            ObjList["NonDrawable"].Add(floor);

            // Add all circle objects and set them as not visible
            float deltaX = board.width / ColNum;
            float deltaY = board.height / RowNum;
            for (int x = 0; x < ColNum; x++)
            {
                for (int y = 1; y < RowNum; y++)
                {
                    Vector2 location = new Vector2(x * deltaX, y * deltaY);
                    CircleObject newCircle = new CircleObject(location, Color.White, graphicsDevice, CircleRadius);
                    newCircle.IsVisible = false;
                    newCircle.Freeze();
                    newCircle.IsPhysical = false;
                    ObjList["Drawable"].Add(newCircle);
                }
            }
            
            PlayerToColorInit();
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
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

            foreach (GameObject gameObject in ObjList["Drawable"])
            {
                gameObject.IsVisible = false;
                gameObject.Speed = Vector2.Zero;
            }

            CurrTurn = Players.Player1;

            if (bot != null && !bot.IsLearning && bot.BotTurn == Players.Player1)
                bot.TakeAction(this, GetState());
        }
        /// <summary>
        /// Initialize the player-to-color dictionary.
        /// </summary>
        private void PlayerToColorInit()
        {
            PlayerColor = new Dictionary<Players, Color>
            {
                [Players.Player1] = Color.Yellow,
                [Players.Player2] = Color.Red
            };
        }

        /// <summary>
        /// Function called on left click release
        /// </summary>
        public override bool HandleClick(Vector2 position)
        {
            // If a bot is loaded, play against it
            if (bot != null && bot.IsLearning)
                return false;
            if (PlayAgainst != Against.Noone && CurrTurn == BotPlayer)
                return true;
            AddCircle(position);
            if (IsTerminalState())
                Clean();
            
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            // Apply physics on all the objects in the game
            List<GameObject> obj = new List<GameObject>();
            foreach(var objList in ObjList.Values)
            {
                obj.AddRange(objList);
            }
            foreach (GameObject c in obj)
            {
                c.Update(obj);
            }

            if (CurrTurn == BotPlayer)
            {
                bool ActionTaken = false;
                if (bot != null && bot.IsLearning)
                    return;
                else if (PlayAgainst == Against.Minimax && Enemy != null)
                {
                    Enemy.TakeAction(this, GetState());
                    ActionTaken = true;
                }
                else if (PlayAgainst == Against.Bot && bot.IsSetup)
                {
                    bot.TakeAction(this, GetState());
                    ActionTaken = true;
                }
                if (IsTerminalState() && ActionTaken)
                    Clean();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Mode != Modes.Learning)
            {
                // Draw all the drawable objects
                for (int i = 0; i < ObjList["Drawable"].Count; i++)
                {
                    GameObject c = ObjList["Drawable"][i];
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
                    circleList[collumn, y] = CurrTurn;
                    break;
                }
            }
        }

        public override void RegisterAction(State s, Actione a, Players player)
        {

            // Add the circle to the two dimensional array for checking the wins
            for (int y = 0; y < RowNum - 1; y++)
            {
                if ((Players)s.Board[a.ID, y] == Players.NoPlayer)
                {
                    s.Board[a.ID, y] = (double)player;
                    break;
                }
            }
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

        /// <summary>
        /// Add a circle to its final place and freeze it immidiently
        /// </summary>
        /// <param name="addX">The collumn in which it will be added</param>
        private void QuickAdd(int addX)
        {
            if (circleList[addX, RowNum - 2] != Players.NoPlayer)
            {
                NextTurn();
                return;
            }

            int fixX = (board.width / ColNum - CircleRadius * 2) / 2;
            int fixY = (board.height / RowNum - CircleRadius * 2) / 2;

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
            //CircleObject toAdd = new CircleObject(pos, PlayerColor[CurrTurn], graphicsDevice, CircleRadius);
            //toAdd.IsVisible = true;
            //ObjList["Drawable"].Add(toAdd);
            ObjList["Drawable"][addX * (RowNum - 1) + addY - 1].IsVisible = true;
            ObjList["Drawable"][addX * (RowNum - 1) + addY - 1].Color = PlayerColor[CurrTurn];
            ObjList["Drawable"][ObjList.Count - 1].Freeze();

            NextTurn();
        }
        
        /// <summary>
        /// Add a circle at the top of a collumn, if it will collide with another circle cancel the adding.
        /// </summary>
        /// <param name="addX">The collumn it will be added at</param>
        /// <param name="pos">The position of the click</param>
        private void GraphicalAdd(int addX, Vector2 pos)
        {
            int fixX = (board.width / ColNum - CircleRadius * 2) / 2;

            bool add = true;
            foreach (GameObject c in ObjList["Drawable"])
            {
                if (c.Collides(new Vector2((int)pos.X + 1, (int)pos.Y + 1), new Vector2(board.width / ColNum - 2, (board.height / RowNum) - 2)))
                    add = false;
            }
            if (add)
            {
                int y = 0;
                for (; y < RowNum - 1; y++)
                {
                    if (circleList[addX, y] == Players.NoPlayer)
                    {
                        break;
                    }
                }

                RegisterCircle(addX);

                // Adding the circle object for graphics
                pos.X += fixX;
                ObjList["Drawable"][addX * (RowNum - 1) + y].Location = pos;
                ObjList["Drawable"][addX * (RowNum - 1) + y].Color = PlayerColor[CurrTurn];
                ObjList["Drawable"][addX * (RowNum - 1) + y].IsVisible = true;
                ObjList["Drawable"][addX * (RowNum - 1) + y].Frozen = false;
                ObjList["Drawable"][addX * (RowNum - 1) + y].IsPhysical = true;
                ObjList["Drawable"][addX * (RowNum - 1) + y].EditHitbox(new Vector2(board.width / ColNum - fixX, board.height / RowNum));
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

            // Add the circle according to the current mode
            if (Mode == Modes.Quick)
            {
                QuickAdd(addX);
            }
            else
            {
                GraphicalAdd(addX, pos);
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
        public Vector2 ClampCircle(Vector2 pos)
        {
            int deltaX = board.width / ColNum;
            int seperation = 0;
            int newX = ((int)pos.X / deltaX) * deltaX + seperation;
            int deltaY = board.height / RowNum;
            int newY = ((int)pos.Y / deltaY) * deltaY + seperation;
            return new Vector2(newX, newY);
        }

        /// <summary>
        /// Return which player (player1, player2, no player) has won in the current game state.
        /// </summary>
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

        public override Players CheckWin(State gameState)
        {
            Players[,] temp = (Players[,])circleList.Clone();
            for(int x = 0; x < circleList.GetLength(0); x++)
            {
                for(int y = 0; y < circleList.GetLength(1); y++)
                {
                    circleList[x, y] = (Players)gameState.Board[x, y];
                }
            }
            Players won = CheckWin();
            circleList = temp;
            return won;
        }

        /// <summary>
        /// To be called when the exiting the game. Disposes all the textures
        /// </summary>
        public override void Clear()
        {
            board.Dispose();
            foreach (var obj in ObjList["Drawable"])
                obj.Dispose();
            if (bot != null)
                bot.Stop();
        }

        /// <summary>
        /// Get the state of the game.
        /// </summary>
        public override State GetState()
        {
            return new State(circleList);
        }

        public override double GetReward(Players forPlayer)
        {
            if (CheckWin() == forPlayer)
                return 5;
            else if (CheckWin() == Players.NoPlayer)
                return 0;
            else
                return -5;
        }

        public override double GetReward(Players forPlayer, State gameState)
        {
            if (CheckWin(gameState) == forPlayer)
                return 5;
            else if (CheckWin(gameState) == Players.NoPlayer)
                return 0;
            else
                return -5;
        }
        /// <summary>
        /// Check if an action is legal in the current game state
        /// </summary>
        public override bool IsLegalAction(Actione action)
        {
            if (circleList[action.ID, RowNum - 2] != Players.NoPlayer)
                return false;
            return true;
        }

        public override bool IsLegalAction(Actione action, State s)
        {
            if ((Players)s.Board[action.ID, RowNum - 2] != Players.NoPlayer)
                return false;
            return true;
        }

        /// <summary>
        /// Execute the action and pass to the next turn
        /// </summary>
        public override void DoAction(Actione action)
        {
            QuickAdd(action.ID);
        }

        protected override bool NoMovesLeft()
        {
            for(int x = 0; x < ColNum; x++)
            {
                if (circleList[x, RowNum - 2] == Players.NoPlayer)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Makes the control ready for another move
        /// </summary>
        public override void Clean()
        {
            if (IsTerminalState())
                Restart(Mode);
        }

        /// <summary>
        /// Start learning with the bot in a different thread
        /// </summary>
        public override void StartLearn()
        {
            if (!bot.IsLearning)
            {
                Mode = Modes.Quick;
                Restart(Mode);
                Thread thread = new Thread(
                new ThreadStart(StartBot));
                thread.Start();
            }
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
            bot.Learn(200000, 0.05f, 0.0005f, 0.9f, 0.5f, Enemy);
            Restart(Mode);
        }

        /// <summary>
        /// Stop learning with the bot
        /// </summary>
        public override void StopLearn()
        {
            if (bot.IsLearning)
            {
                bot.Stop();
                Restart(Mode);
            }
        }

        public override void SetBot(LearningBot bot)
        {
            base.SetBot(bot);
            Restart(Mode);
        }

        public override bool HandleKeyPress(Keys key)
        {
            return false;
        }


    }
}