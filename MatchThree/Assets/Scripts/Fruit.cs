using Boo.Lang;
using System.Diagnostics;
/// <summary>
/// Knows position and type of fruit, contains a reference to the piece tile
/// </summary>
public class Fruit
{
    public int row { get; private set; }
    public int column { get; private set; }
    public int type { get; private set; }
    public Piece piece { get; private set; }

    // Constructor
    public Fruit(int row, int column, Piece tile, int type)
    {        
        this.piece = tile;
        this.type = type;
        SetType(type);
        ChangePosition(row, column);
    }

    /// <summary>
    /// Changes type and sprite of the piece
    /// </summary>
    public void SetType(int type)
    {
        this.type = type;
    }

    /// <summary>
    /// Changes the pieces position in the grid array
    /// </summary>
    public void ChangePosition(int row, int column)
    {
        this.row = row;
        this.column = column;
        GameManager.Instance.SetPiecePosInArray(this, row, column);
    }

    /// <summary>
    /// Returns true if the type of the other fruit matches the type of this fruit
    /// </summary>
    public bool IsEqual(Fruit otherFruit)
    {
        return this.type == otherFruit.type ? true : false;
    }

    /// <summary>
    /// Swaps this pieces position with the other one and vise versa
    /// </summary>
    /// <param name="fruit"></param>
    public void Swap(Fruit fruit)
    {
        int newX = fruit.row;
        int newY = fruit.column;
        fruit.ChangePosition(row, column);
        ChangePosition(newX, newY);
    }
}
