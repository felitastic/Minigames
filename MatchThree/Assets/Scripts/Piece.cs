/// <summary>
/// Data container, knows position in grid, piece type and holds reference to a tile gameobject
/// </summary>
public class Piece
{
    public IntPos2D GridPos { get; private set; }
    public int Type { get; private set; }
    public Tile TileObject { get; private set; }
    public int FallPosY { get; private set; }

    // Constructor
    public Piece(int xPos, int yPos, Tile tile, int type)
    {        
        this.TileObject = tile;
        this.Type = type;
        GridPos = new IntPos2D(xPos, yPos);
        SetNewType(type);
    }

    /// <summary>
    /// Changes type and sprite of the piece
    /// </summary>
    public void SetNewType(int type)
    {
        this.Type = type;
    }

    /// <summary>
    /// Changes the pieces position in the grid array
    /// </summary>
    public void SetGridPosition(int newX, int newY)
    {
        GridPos.x = newX;
        GridPos.y = newY;
        GameManager.Instance.SetPiecePosInArray(this, newX, newY);
    }

    public void SetFallPosition(int fallPosY)
    {
        FallPosY = fallPosY;
    }

    /// <summary>
    /// Returns true if the type of the other piece matches the type of this piece
    /// </summary>
    public bool MatchesWith(Piece otherPiece)
    {
        return this.Type == otherPiece.Type ? true : false;
    }

    /// <summary>
    /// Swaps this pieces position with the other one and vise versa
    /// </summary>
    public void Swap(Piece piece)
    {
        int newX = piece.GridPos.x;
        int newY = piece.GridPos.y;
        piece.SetGridPosition(GridPos.x, GridPos.y);
        SetGridPosition(newX, newY);
    }
}

/// <summary>
/// Takes two ints for pos x and pos x
/// </summary>
public class IntPos2D
{
    public int x;
    public int y;

    public IntPos2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
