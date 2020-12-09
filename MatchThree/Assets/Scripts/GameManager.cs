using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Knows the hard facts such as game state, board size, max moves and all the Tile objects on the grid
/// </summary>
public class GameManager : Singleton<GameManager>
{
    protected GameManager() { }
    private SaveSystem saveSystem;
    public List<ScoreEntry> allScores { get; private set; }

    //[Range(4,18)]
    public int Width { get; protected set; }
    //[Range(4,10)]
    public int Height { get; protected set; }
    //[Range(10, 100)]
    public int MaxMoves { get; protected set; }
    public Sprite[] PieceSprites;

    private eGameState prePauseState = eGameState.running;
    public int FullScore { get; private set; }
    public int MovesLeft { get; private set; }

    public Piece[,] Pieces { get; private set; }
    public eGameState GameState { get; private set; }

    public static event System.Action<GameManager> OnStart = delegate { };
    public static event System.Action OnSetup = delegate { };
    public static event System.Action<int> OnScoreUpdate = delegate { };
    public static event System.Action<int> OnMoveChange = delegate { };
    public static event System.Action OnGameEnd = delegate { };

    private void Awake()
    {
        allScores = new List<ScoreEntry>();
        Pieces = new Piece[0, 0];
        //print("piece count: " + Pieces.Length);
        SetGameState(eGameState.setup);
    }

    private void Start()
    {
        //print("GM calls Startup");
        saveSystem = GetComponent<SaveSystem>();
        //BaseScore();
        saveSystem.Load();
        OnStart(this);

    }

    private void BaseScore()
    {
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                allScores.Add(new ScoreEntry(j, "Granny", 111 * i));
            }
        }
        saveSystem.Save(allScores);
    }

    public void SetScores(List<ScoreEntry> scores)
    {
        allScores = scores;
    }

    public void StartNewGame(int boardSize, int sessionLength)
    {
        SetGameState(eGameState.setup);
        switch (boardSize)
        {
            case 0:
                Width = 7;
                Height = 5;
                break;
            case 1:
                Width = 10;
                Height = 8;
                break;
            case 2:
                Width = 12;
                Height = 9;
                break;
            default:
                break;
        }
        switch (boardSize)
        {
            case 0:
                MaxMoves = 10;
                break;
            case 1:
                MaxMoves = 20;
                break;
            case 2:
                MaxMoves = 40;
                break;
            default:
                break;
        }

        ResetGame();
    }

    /// <summary>
    /// Sets all parameters to their initial value, sets board size and camera position
    /// </summary>
    public void ResetGame()
    {
        MovesLeft = MaxMoves;
        FullScore = 0;
        UpdateUI();
        SetCameraPosition();
        OnSetup();
    }

    /// <summary>
    /// Sends actions with all current UI values out
    /// </summary>
    private void UpdateUI()
    {
        OnScoreUpdate(FullScore);
        OnMoveChange(MovesLeft);
    }

    /// <summary>
    /// Changes the GameState
    /// </summary>
    public void SetGameState(eGameState newState)
    {
        GameState = newState;
        print(GameState.ToString());
    }
    /// <summary>
    /// Initiates the 2D piece array with width and height 
    /// </summary>
    public void SetGridArray()
    {
        Pieces = new Piece[Width, Height];
    }
    /// <summary>
    /// Sets the camera to center the board
    /// </summary>
    public void SetCameraPosition()
    {
        Camera.main.transform.position = new Vector3((float)(Width - 1) / 2, (float)(Height - 1) / 2, -10);
    } 

    /// <summary>
    /// Sets the pieces position in the piece array to x, y
    /// </summary>
    public void SetPiecePosInArray(Piece piece, int xPos, int yPos)
    {
        Pieces[xPos, yPos] = piece;
    }
    /// <summary>
    /// Adds gain to the final score
    /// </summary>
    public void AddToScore(int gain)
    {
        //print(fullScore + " + " + gain);
        FullScore += gain;
        OnScoreUpdate(FullScore);
    }
    /// <summary>
    /// Subtracts one move from the moves the player has left
    /// </summary>
    public void SubstractOneMove()
    {
        MovesLeft -= 1;
        OnMoveChange(MovesLeft);
    }

    /// <summary>
    /// Returns a piece if there is one at the given position in the array
    /// </summary>
    public Piece GetPieceAt(int xPos, int yPos)
    {
        if (xPos < Width && xPos >= 0 && yPos < Height && yPos >= 0)
            return Pieces[xPos, yPos];

        return null;
    }

    /// <summary>
    /// Returns true if the condition for the end of the game is given (no moves left)
    /// </summary>
    public bool GameEnd()
    {
        if (MovesLeft <= 0)
        {
            SetGameState(eGameState.end);
            //TODO: save score
            allScores.Add(new ScoreEntry(1, "Blubbi", FullScore));
            saveSystem.Save(allScores);
            OnGameEnd();
            return true;
        }
        return false;
    }

    public void Pause()
    {
        prePauseState = GameState;
        SetGameState(eGameState.paused);
    }

    public void UnPause()
    {
        SetGameState(prePauseState);
    }

    /// <summary>
    /// Returns a random sprite/type within the sprite array
    /// </summary>
    public int RandomType()
    {
        return Random.Range(0, PieceSprites.Length);
    }

    /*
     * According to the game rules, only the following "neighbours" have to be checked on Move/Spawn
     * 
     * Positions in Grid relative to "clickedPiece" (x,y):
     *             | 0,+2|
     *       |-1,+1| 0,+1|+1,+1|
     * |-2, 0|-1, 0| x, y|+1, 0|+2, 0|
     *       |-1,-1| 0,-1|+1,-1|
     *             | 0,-2|
     *             
     *   given back if startUp = false:       given back if startUp = true:
     *               | 2 |                (x = movedFruit not included in array)
     *           | 3 | 1 | 4 |          
     *      | 10 | 9 | 0 | 11| 12 |             | 2 | 0 | x |
     *           | 8 | 5 | 7 |                      | 5 | 1 | 4 |    
     *               | 6 |                              | 3 |
     */
    /// <summary>
    /// Returns a list of all neighbouring pieces for the given piece
    /// If startUp is true, it will return the neighbours of a freshly spawned piece
    /// </summary>
    public Piece[] FruitToCheck(Piece clickedPiece, bool startUp = false)
    {
        int xPos = clickedPiece.GridPos.x;
        int yPos = clickedPiece.GridPos.y;
        Piece[] neighbouringPiece = new Piece[13];

        if (startUp)
        {
            neighbouringPiece[0] = GetPieceAt(xPos - 1, yPos);
            neighbouringPiece[1] = GetPieceAt(xPos, yPos - 1);
            neighbouringPiece[2] = GetPieceAt(xPos - 2, yPos);
            neighbouringPiece[3] = GetPieceAt(xPos, yPos - 2);
            neighbouringPiece[4] = GetPieceAt(xPos - 1, yPos - 1);
            neighbouringPiece[5] = GetPieceAt(xPos + 1, yPos - 1);
        }
        else
        {
            neighbouringPiece[0] = GetPieceAt(xPos, yPos);
            neighbouringPiece[1] = GetPieceAt(xPos, yPos + 1);
            neighbouringPiece[2] = GetPieceAt(xPos, yPos + 2);
            neighbouringPiece[3] = GetPieceAt(xPos - 1, yPos + 1);
            neighbouringPiece[4] = GetPieceAt(xPos + 1, yPos + 1);
            neighbouringPiece[5] = GetPieceAt(xPos, yPos - 1);
            neighbouringPiece[6] = GetPieceAt(xPos, yPos - 2);
            neighbouringPiece[7] = GetPieceAt(xPos + 1, yPos - 1);
            neighbouringPiece[8] = GetPieceAt(xPos - 1, yPos - 1);
            neighbouringPiece[9] = GetPieceAt(xPos - 1, yPos);
            neighbouringPiece[10] = GetPieceAt(xPos - 2, yPos);
            neighbouringPiece[11] = GetPieceAt(xPos + 1, yPos);
            neighbouringPiece[12] = GetPieceAt(xPos + 2, yPos);
        }
        return neighbouringPiece;
    }

    public ScoreEntry[] CurrentTop5Of(int boardID)
    {
        ScoreEntry[] scores = new ScoreEntry[5];

        scores = (from score in allScores
                  where score.boardID == boardID
                  orderby score.score
                  select score).ToArray();
        print("top score " + scores[0] + ", lowest score " + scores[4]);
        return scores;
    }

    public bool AddToScoreList(ScoreEntry newEntry)
    {
        ScoreEntry[] top5 = CurrentTop5Of(newEntry.boardID);
        //TODO: add new score to list if possible, send it to UI
        //check if score is higher than one of the scores in the list
        //move all others down and delete last
        //somehow save which one is the new one for highlighting
        //profit
        return true;
    }
}
