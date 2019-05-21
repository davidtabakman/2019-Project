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

    public class Actione
    {
        public int ID { get; private set; }

        public Actione(int ID)
        {
            this.ID = ID;
        }
    }

    public class State
    {
        public int ID { get; private set; }
        public double[,] Board { get; private set; }

        public State(int ID)
        {
            this.ID = ID;
            Board = null;
        }

        /// <summary>
        /// Create a state using a board of players.
        /// </summary>
        public State (GameControlBase.Players[,] board)
        {
            Board = new double[board.GetLength(0), board.GetLength(1)];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Board[x, y] = (int)board[x, y];
                }
            }
            ID = -1;
        }
        /// <summary>
        /// Create a state using an ID and a board of players.
        /// </summary>
        public State(int ID, GameControlBase.Players[,] board)
        {
            Board = new double[board.GetLength(0), board.GetLength(1)];
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Board[x, y] = (int)board[x, y];
                }
            }
            this.ID = ID;
        }
    }

    /// <summary>
    /// The base for all game related controls.
    /// </summary>
    public abstract class GameControlBase : ControlBase
    {
        // Player ID's
        public enum Players
        {
            Player1 = 1,
            Player2 = 2,
            NoPlayer = 0
        }

        public Players CurrTurn { get; protected set; }
        public bool IsDrawing { get; protected set; }
        public int FeatureNum;

        public GameControlBase(int ClickPriority) : base(ClickPriority)
        {
            IsLearnable = true; // Games are learnable
        }

        // All the required functions for machine learning
        public abstract void StartLearn();
        public abstract void StopLearn();
        public abstract State GetState();
        public abstract double GetReward(Players forPlayer);
        public abstract bool IsLegalAction(Actione action);
        public abstract void DoAction(Actione action);
        public abstract bool IsTerminalState();
        public abstract bool IsTerminalState(State s);
        public abstract void Clean();
        public abstract Players CheckWin();
        public abstract void SetBot(LearningBot bot);
        public abstract LearningBot GetBot();
        public abstract void SetOpponent(LearningBot bot);
    }
}