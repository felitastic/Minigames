﻿public enum eGameState
{
    setup,
    running,
    matching,
    falling,
    paused,
    end
}

public enum eSwipeDir
{
    none = 0,
    up = 1,
    down = -1,
    left = 2,
    right = -2
}

public enum eMenuScene 
{ 
    Start, 
    GameMode, 
    Ingame, 
    Help, 
    Tutorial,
    NormalResult, 
    ScoringResult,
    Highscore, 
    Credits 
}
