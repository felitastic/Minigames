using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes care of spawning and destroying the board for every new game
/// </summary>

//destroy whole board
//destroy all pieces
public class BoardSpawn : MonoBehaviour
{
    [Header("Prefabs to spawn")]
    [SerializeField]
    public Tile TileObject;
    [SerializeField]
    public GameObject BGTile;

    [Header("Spawnpoints to parent the instantiated prefabs to")]
    [SerializeField]
    private Transform TileSpawn;
    [SerializeField]
    private Transform BGTileSpawn;

    [Header("Mask to hide sprites outside of the board range")]
    [SerializeField]
    private GameObject mask;

    [SerializeField]
    private GameManager GM;
    private List<GameObject> allBGTiles = new List<GameObject>();

    private void Awake()
    {
        GameManager.OnStart += SetGM;
        GameManager.OnSetup += SetBoard;
        GameManager.OnGameEnd += ResetBoard;
        //print("Boardspawn set all delegates");
    }

    /// <summary>
    /// On Startup, GM makes a call to tell other scripts it is ready
    /// </summary>
    private void SetGM(GameManager GM)
    {
        this.GM = GM;
        //print("Boardspawn got GM");
    }

    /// <summary>
    /// Starts the setup of the board and tiles
    /// </summary>
    private void SetBoard()
    {
        if (ResetNeeded())
            ResetBoard();
        else
            GM.SetGridArray();

        for (int column = 0; column < GM.Height; column++)
        {
            for (int row = 0; row < GM.Width; row++)
            {
                //if (allBGTiles.Count < GM.Height * GM.Width)
                //    SpawnBoardTile(row, column);
                SpawnPiece(row, column);
            }
        }
        SetMask();
        GM.SortGemsByColor();
        GM.SetGameState(eGameState.running);
        StartCoroutine(GM.GemShimmer());
    }

    /// <summary>
    /// Sets mask size and position to cover the whole board
    /// </summary>
    private void SetMask()
    {
        mask.transform.position = new Vector3((float)(GM.Width - 1) / 2, (float)(GM.Height - 1) / 2, 0);
        mask.transform.localScale = new Vector3(GM.Width, GM.Height, 1);
    }

    /// <summary>
    /// Spawns the board tiles according to height and width 
    /// </summary>
    private void SpawnBoardTile(int x, int y)
    {
        //Instantiating the tile, renaming for better overview in inspector
        GameObject backgroundTile = Instantiate(BGTile, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        backgroundTile.transform.SetParent(BGTileSpawn);
        int ID = y * GM.Width + x;
        backgroundTile.name = ID.ToString() + ": " + x + "," + y;
        allBGTiles.Add(backgroundTile);
    }

    /// <summary>
    /// Spawns the pieces on the board while making sure there are no matches at the start
    /// </summary>
    private void SpawnPiece(int x, int y)
    {
        Tile tile = Instantiate(TileObject, new Vector3(x, y, 0), Quaternion.identity); //TODO: y+height to offset them for the start?
        tile.transform.SetParent(TileSpawn);
        int spriteToUse = GM.RandomType();
        GM.SetPiecePosInArray(new Piece(x, y, tile, spriteToUse), x, y);

        while (PreventMatch(GM.GetPieceAt(x, y)))
        {
            spriteToUse = GM.RandomType();
            GM.Pieces[x, y].SetNewType(spriteToUse);
        }

        GM.Pieces[x, y].TileObject.SetGemSprite();
        int ID = y * GM.Width + x;
        tile.name = ID.ToString();
    }

    /// <summary>
    /// Returns true if the fruit has a match
    /// </summary>
    private bool PreventMatch(Piece piece)
    {
        //3rd spawned piece (no check needed before)
        if (piece.GridPos.x >= 2 && piece.GridPos.y == 0)
        {
            if (piece.MatchesWith(GM.Pieces[piece.GridPos.x - 1, piece.GridPos.y]) && piece.MatchesWith(GM.Pieces[piece.GridPos.x - 2, piece.GridPos.y]))
                return true;
        }
        //first piece in second row
        else if (piece.GridPos.x == 0 && piece.GridPos.y == 1)
        {
            if (piece.MatchesWith(GM.Pieces[piece.GridPos.x, piece.GridPos.y - 1]) && piece.MatchesWith(GM.Pieces[piece.GridPos.x + 1, piece.GridPos.y - 1]))
                return true;
        }
        //pieces spawned after this
        else if (piece.GridPos.y > 0)
        {
            //get all neighbours of the spawned piece
            Piece[] neighbours = GM.FruitToCheck(piece, true);

            if (piece.MatchesWith(neighbours[1]))
            {
                if (piece.GridPos.x != GM.Width - 1 && piece.MatchesWith(neighbours[5]))
                    return true;
                else if (piece.GridPos.y > 1 && piece.MatchesWith(neighbours[3]))
                    return true;
                else if (piece.GridPos.x != 0 && piece.MatchesWith(neighbours[4]))
                    return true;
                else if (piece.GridPos.x != 0 && piece.MatchesWith(neighbours[0]))
                    return true;
            }
            else if (piece.GridPos.x != 0 && piece.MatchesWith(neighbours[0]))
            {
                if (piece.MatchesWith(neighbours[4]))
                    return true;
                else if (piece.GridPos.x != 1 && piece.MatchesWith(neighbours[2]))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if there are already tiles in the game
    /// </summary>
    private bool ResetNeeded()
    {
        if (GM.Pieces.Length == 0)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Destroys pieces and bg tiles if needed
    /// </summary>
    private void ResetBoard()
    {
        //destroy all pieces
        foreach (Piece piece in GM.Pieces)
        {
            Destroy(piece.TileObject.gameObject);
        }

        GM.SetGridArray();

        //if the size of the board changed, destroy all bg tiles
        if (allBGTiles.Count != GM.Height*GM.Width)
        {
            for (int i = 0; i < allBGTiles.Count; i++)
            {
                Destroy(allBGTiles[i]);
            }
            allBGTiles.Clear();
        }
    }
}
