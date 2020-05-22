public enum eGameState
{
    running,
    paused,
    loading,
    minigame,
    cutscene
}

public enum eAbility
{
    wings = 0,
    horn = 1,
    water = 2,
    claw = 3
}

public enum ePlayerShape 
{ 
    TBlock = 0, 
    LBlock = 1, 
    IBlock = 2, 
    OBlock = 3, 
    SBlock = 4, 
    ZBlock = 5, 
    JBlock = 6 
}

public enum ePosState
{
    free,
    obstacle,
    border,
    goal        
}

public enum eMinigameState 
{ 
    ready,
    paused,     
    running, 
    Lose, 
    Win 
}
