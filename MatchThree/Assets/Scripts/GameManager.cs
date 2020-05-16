using core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected GameManager() { }

    public int Width = 10;
    public int Height = 5;
    [Tooltip("Order: Orange, Apple, Coconut, Melon, Passionfruit")]
    public Sprite[] PieceSprites;
    public Fruit[,] Pieces { get; protected set; }
    public eGameState GameState { get; private set; }

    public static event System.Action<GameManager> OnStart = delegate { };
    public static event System.Action OnRestart = delegate { };

    private void Awake()
    {
        SetGameState(eGameState.loading);
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
    /// Initiates the 2D piece array with width and height
    /// </summary>
    public void SetBoardSize()
    {
        Pieces = new Fruit[Width, Height];
    }
    /// <summary>
    /// Sets the pieces position in the piece array to row, column
    /// </summary>
    public void SetPiecePosInArray(Fruit fruit, int row, int column)
    {
        Pieces[row, column] = fruit;
    }
    /// <summary>
    /// Returns a piece if there is one at the given position in the array
    /// </summary>
    public Fruit GetFruit(int row, int column)
    {
        if (row < Width && row >= 0 && column < Height && column >= 0)
            return Pieces[row, column];

        return null;
    }
    /// <summary>
    /// Destroys all pieces and calls board to respawn them
    /// </summary>
    public void Restart()
    {
        SetGameState(eGameState.loading);
        foreach(Fruit fruit in Pieces)
        {
            Destroy(fruit.piece.gameObject);
        }
        OnRestart();
    }
}
