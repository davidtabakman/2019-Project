using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Learning;
using GameCenter;
using Controller;

public abstract class GameControlBase : ControlBase
{
    public enum Players
    {
        Player1 = 1,
        Player2 = 2,
        NoPlayer = 0
    }
    public Players CurrTurn { get; protected set; }
    public abstract State GetState();
    public abstract int GetReward(Players forPlayer);
    public abstract bool IsLegalAction(Actione action);
    public abstract void DoAction(Actione action);
    public abstract bool IsTerminalState();
    public abstract void Clean();
    public abstract Players CheckWin();
}