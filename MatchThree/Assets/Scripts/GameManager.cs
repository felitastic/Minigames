using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected GameManager() { }

    public int Width = 10;
    public int Height = 5;
    [Tooltip("Order: Orange, Apple, Coconut, Melon, Passionfruit")]
    public Sprite[] PieceSprites;
    public Piece[,] Pieces { get; protected set; }
    public eGameState GameState { get; private set; }

    public static event System.Action<GameManager> OnStart = delegate { };
    public static event System.Action OnRestart = delegate { };

    private eGameState prePauseState = eGameState.running;

    private void Awake()
    {
        SetGameState(eGameState.setup);
    }

    private void Start()
    {
        SetBoardSize();
        OnStart(this);
    }
    /// <summary>
    /// Changes the GameState
    /// </summary>
    public void SetGameState(eGameState newState)
    {
        GameState = newState;
    }
    /// <summary>
    /// Initiates the 2D piece array with width and height (+1 for upper spawn when falling down)
    /// </summary>
    public void SetBoardSize()
    {
        Pieces = new Piece[Width, Height];
    }
    /// <summary>
    /// Sets the pieces position in the piece array to row, column
    /// </summary>
    public void SetPiecePosInArray(Piece piece, int xPos, int yPos)
    {
        Pieces[xPos, yPos] = piece;
    }
    /// <summary>
    /// Returns a piece if there is one at the given position in the array
    /// </summary>
    public Piece GetFruit(int xPos, int yPos)
    {
        if (xPos < Width && xPos >= 0 && yPos < Height && yPos >= 0)
            return Pieces[xPos, yPos];

        return null;
    }
    /// <summary>
    /// Destroys all pieces and calls board to respawn them
    /// </summary>
    public void Restart()
    {
        SetGameState(eGameState.setup);
        foreach(Piece piece in Pieces)
        {
            Destroy(piece.TileObject.gameObject);
        }
        OnRestart();
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
}
