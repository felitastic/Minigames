using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Board : MonoBehaviour
{
    [Header("Mask object")]
    [SerializeField]
    private GameObject mask;
    [Header("Parent objects for the pieces and tiles for a cleaner inspector")]
    [SerializeField]
    private Transform pieceSpawn;
    [SerializeField]
    private Transform BGSpawn;
    [Header("Prefabs to spawn")]
    [SerializeField]
    private Tile pieceTile;
    [SerializeField]
    private GameObject bgTile;

    private List<Piece> curMatches = new List<Piece>();

    private GameManager GM;
    private int width { get { return GM.Width; } }
    private int height { get { return GM.Height; } }
    private Piece[,] pieces { get { return GM.Pieces; } }

    private void Awake()
    {
        GameManager.OnStart += SetGM;
    }

    private void Start()
    {
        SetBoard();
        PlayerInput.OnPieceMove += SwapPieces;
        GameManager.OnRestart += SetBoard;
    }

    /// <summary>
    /// On Startup, GM makes a call to tell other scripts it is ready
    /// </summary>
    private void SetGM(GameManager GM)
    {
        this.GM = GM;
    }

    /// <summary>
    /// Starts the setup of the board and tiles
    /// </summary>
    private void SetBoard()
    {
        for (int column = 0; column < height; column++)
        {
            for (int row = 0; row < width; row++)
            {
                SpawnBoardTile(row, column);
                SpawnPieces(row, column);
            }
        }
        SetMask();
        GM.SetGameState(eGameState.running);
    }

    private void SetMask()
    {
        mask.transform.position = new Vector3((float)(width - 1) / 2, (float)(height - 1) / 2, 0);
        mask.transform.localScale = new Vector3(width, height, 1);
    }

    /// <summary>
    /// Spawns the board tiles according to height and width 
    /// </summary>
    private void SpawnBoardTile(int x, int y)
    {
        //Instantiating the tile, renaming for better overview in inspector
        GameObject backgroundTile = Instantiate(bgTile, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        backgroundTile.transform.SetParent(BGSpawn);
        int ID = y * width + x;
        backgroundTile.name = ID.ToString() + ": " + x + "," + y;
    }

    /// <summary>
    /// Spawns the pieces on the board while making sure there are no matches at the start
    /// </summary>
    private void SpawnPieces(int x, int y)
    {
        int spriteToUse = RandomType();
        Tile tile = Instantiate(pieceTile, new Vector3(x, y, 0), Quaternion.identity); //TODO: y+height to offset them for the start?
        tile.transform.SetParent(pieceSpawn);
        GM.SetPiecePosInArray(new Piece(x, y, tile, spriteToUse), x, y);
        while (PreventMatch(pieces[x, y]))
        {
            spriteToUse = RandomType();
            pieces[x, y].SetNewType(spriteToUse);
        }
        pieces[x, y].TileObject.SetSprite(GM.PieceSprites[spriteToUse]);
        int ID = y * width + x;
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
            if (piece.MatchesWith(pieces[piece.GridPos.x - 1, piece.GridPos.y]) && piece.MatchesWith(pieces[piece.GridPos.x - 2, piece.GridPos.y]))
                return true;
        }
        //first piece in second row
        else if (piece.GridPos.x == 0 && piece.GridPos.y == 1)
        {
            if (piece.MatchesWith(pieces[piece.GridPos.x, piece.GridPos.y - 1]) && piece.MatchesWith(pieces[piece.GridPos.x + 1, piece.GridPos.y - 1]))
                return true;
        }
        //pieces spawned after this
        else if (piece.GridPos.y > 0)
        {
            //get all neighbours of the spawned piece
            Piece[] neighbours = FruitToCheck(piece, true);

            if (piece.MatchesWith(neighbours[1]))
            {
                if (piece.GridPos.x != width - 1 && piece.MatchesWith(neighbours[5]))
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

    private int RandomType()
    {
        return Random.Range(0, GM.PieceSprites.Length);
    }

    /// <summary>
    /// Reacts to the player input, checks if there are matches and if positions need to be swapped
    /// </summary>
    private void SwapPieces(Tile tile, eSwipeDir swipeDir)
    {
        Piece piece = GM.GetFruit(Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.y));
        Piece otherPiece = piece;

        switch (swipeDir)
        {
            case eSwipeDir.none:
                return;
            case eSwipeDir.up:
                otherPiece = GM.GetFruit(piece.GridPos.x, piece.GridPos.y + 1);
                break;
            case eSwipeDir.down:
                otherPiece = GM.GetFruit(piece.GridPos.x, piece.GridPos.y - 1);
                break;
            case eSwipeDir.left:
                otherPiece = GM.GetFruit(piece.GridPos.x - 1, piece.GridPos.y);
                break;
            case eSwipeDir.right:
                otherPiece = GM.GetFruit(piece.GridPos.x + 1, piece.GridPos.y);
                break;
            default:
                Debug.Log("Couldn't find the swipeDirection " + swipeDir.ToString() + ", canceling movement & match check");
                break;
        }

        //if they are of the same type, play the "could not swap" animation
        if (piece.MatchesWith(otherPiece))
        {
            StartCoroutine(piece.TileObject.SwapPosition(new Vector3(otherPiece.GridPos.x, otherPiece.GridPos.y, 0), true));
            StartCoroutine(otherPiece.TileObject.SwapPosition(new Vector3(piece.GridPos.x, piece.GridPos.y, 0), true));
            return;
        }

        piece.Swap(otherPiece);

        //check if any piece has a match
        if (CalculateMatches(FruitToCheck(piece)) | CalculateMatches(FruitToCheck(otherPiece)))
        {
            //print(piece.tile.name + " swap Pos: " + piece.GridPos.x + "," + piece.GridPos.y);
            //print(otherPiece.tile.name + " swap Pos: " + otherPiece.GridPos.x + "," + otherPiece.GridPos.y);

            StartCoroutine(piece.TileObject.SwapPosition(new Vector3(piece.GridPos.x, piece.GridPos.y, 0)));
            StartCoroutine(otherPiece.TileObject.SwapPosition(new Vector3(otherPiece.GridPos.x, otherPiece.GridPos.y, 0)));
            RearrangePiecePosInGrid(curMatches);
            return;
        }
        //no matches found, play "could not swap" animation and put them in their original positions
        StartCoroutine(piece.TileObject.SwapPosition(new Vector3(piece.GridPos.x, piece.GridPos.y, 0), true));
        StartCoroutine(otherPiece.TileObject.SwapPosition(new Vector3(otherPiece.GridPos.x, otherPiece.GridPos.y, 0), true));
        otherPiece.Swap(piece);
        GM.SetGameState(eGameState.running);
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
    private Piece[] FruitToCheck(Piece clickedPiece, bool startUp = false)
    {
        int xPos = clickedPiece.GridPos.x;
        int yPos = clickedPiece.GridPos.y;
        Piece[] neighbouringPiece = new Piece[13];

        if (startUp)
        {
            neighbouringPiece[0] = GM.GetFruit(xPos - 1, yPos);
            neighbouringPiece[1] = GM.GetFruit(xPos, yPos - 1);
            neighbouringPiece[2] = GM.GetFruit(xPos - 2, yPos);
            neighbouringPiece[3] = GM.GetFruit(xPos, yPos - 2);
            neighbouringPiece[4] = GM.GetFruit(xPos - 1, yPos - 1);
            neighbouringPiece[5] = GM.GetFruit(xPos + 1, yPos - 1);
        }
        else
        {
            neighbouringPiece[0] = GM.GetFruit(xPos, yPos);
            neighbouringPiece[1] = GM.GetFruit(xPos, yPos + 1);
            neighbouringPiece[2] = GM.GetFruit(xPos, yPos + 2);
            neighbouringPiece[3] = GM.GetFruit(xPos - 1, yPos + 1);
            neighbouringPiece[4] = GM.GetFruit(xPos + 1, yPos + 1);
            neighbouringPiece[5] = GM.GetFruit(xPos, yPos - 1);
            neighbouringPiece[6] = GM.GetFruit(xPos, yPos - 2);
            neighbouringPiece[7] = GM.GetFruit(xPos + 1, yPos - 1);
            neighbouringPiece[8] = GM.GetFruit(xPos - 1, yPos - 1);
            neighbouringPiece[9] = GM.GetFruit(xPos - 1, yPos);
            neighbouringPiece[10] = GM.GetFruit(xPos - 2, yPos);
            neighbouringPiece[11] = GM.GetFruit(xPos + 1, yPos);
            neighbouringPiece[12] = GM.GetFruit(xPos + 2, yPos);
        }
        return neighbouringPiece;
    }

    /// <summary>
    /// Checks for possible matches, returns true if there are enough and calls the destroy method
    /// </summary>
    private bool CalculateMatches(Piece[] piecesToCheck)
    {
        List<Piece> allMatches = Matches(piecesToCheck);

        if (allMatches.Count < 3)
        {
            return false;
        }

        foreach (Piece p in allMatches)
        {
            p.TileObject.Matched();
        }

        //make sure we have no doubles in the list
        foreach (Piece piece in allMatches)
        {
            if (!curMatches.Contains(piece))
                curMatches.Add(piece);
        }
        return true;
    }

    /// <summary>
    /// Returns a List of the matches for the piece in pos 0 of the given array
    /// For further ref on the array, lookup FruitToCheck method
    /// </summary>
    private List<Piece> Matches(Piece[] piecesToCheck)
    {
        Piece piece = piecesToCheck[0];
        List<Piece> matches = new List<Piece>();
        matches.Add(piece);

        //upper pieces
        if (piece.GridPos.y < height - 1 && piece.MatchesWith(piecesToCheck[1]))
        {
            matches.Add(piecesToCheck[1]);
            if (piece.GridPos.y < height - 2 && piece.MatchesWith(piecesToCheck[2]))
            {
                matches.Add(piecesToCheck[2]);
            }
            else if (piece.GridPos.x > 0 && piece.MatchesWith(piecesToCheck[3]))
            {
                matches.Add(piecesToCheck[3]);
            }
            else if (piece.GridPos.x < width - 1 && piece.MatchesWith(piecesToCheck[4]))
            {
                matches.Add(piecesToCheck[4]);
            }
        }
        //lower pieces
        if (piece.GridPos.y > 0 && piece.MatchesWith(piecesToCheck[5]))
        {
            matches.Add(piecesToCheck[5]);
            if (piece.GridPos.y > 1 && piece.MatchesWith(piecesToCheck[6]))
            {
                matches.Add(piecesToCheck[6]);
            }
            else if (piece.GridPos.x < width - 1 && piece.MatchesWith(piecesToCheck[7]))
            {
                matches.Add(piecesToCheck[7]);
            }
            else if (piece.GridPos.x > 0 && piece.MatchesWith(piecesToCheck[8]))
            {
                matches.Add(piecesToCheck[8]);
            }
        }
        //left pieces
        if (piece.GridPos.x > 0 && piece.MatchesWith(piecesToCheck[9]))
        {
            matches.Add(piecesToCheck[9]);
            if (piece.GridPos.x > 1 && piece.MatchesWith(piecesToCheck[10]))
            {
                matches.Add(piecesToCheck[10]);
            }
            else if (piece.GridPos.y < height - 1 && piece.MatchesWith(piecesToCheck[3]))
            {
                if (!matches.Contains(piecesToCheck[3]))
                    matches.Add(piecesToCheck[3]);
            }
            else if (piece.GridPos.y > 0 && piece.MatchesWith(piecesToCheck[8]))
            {
                if (!matches.Contains(piecesToCheck[8]))
                    matches.Add(piecesToCheck[8]);
            }
        }
        //right pieces
        if (piece.GridPos.x < width - 1 && piece.MatchesWith(piecesToCheck[11]))
        {
            matches.Add(piecesToCheck[11]);
            if (piece.GridPos.x < width - 2 && piece.MatchesWith(piecesToCheck[12]))
            {
                matches.Add(piecesToCheck[12]);
            }
            else if (piece.GridPos.y > 0 && piece.MatchesWith(piecesToCheck[7]))
            {
                if (!matches.Contains(piecesToCheck[7]))
                    matches.Add(piecesToCheck[7]);
            }
            else if (piece.GridPos.y < height - 1 && piece.MatchesWith(piecesToCheck[4]))
            {
                if (!matches.Contains(piecesToCheck[4]))
                    matches.Add(piecesToCheck[4]);
            }
        }
        return matches;
    }

    /// <summary>
    /// Destroys all pieces in given list, rearranges positions of all pieces on the board
    /// </summary>
    private void RearrangePiecePosInGrid(List<Piece> matchedPieces)
    {
        //How many tiles are going to be empty due to matches
        int[] freeTileCount = new int[width];

        for (int i = 0; i < matchedPieces.Count; i++)
        {
            int xPos = matchedPieces[i].GridPos.x;
            freeTileCount[xPos] += 1;
        }

        //swap each matched piece with pieces above it until it reached top position
        foreach (Piece matchedP in matchedPieces)
        {
            int start = matchedP.GridPos.y + 1;
            int posX = matchedP.GridPos.x;
            for (int y = start; y < height; y++)
            {
                Piece upperPiece = pieces[posX, y];
                matchedP.Swap(upperPiece);
            }
        }

        //set fall pos and new type for each matched piece
        foreach (Piece matchedP in matchedPieces)
        {
            matchedP.SetFallPosition(matchedP.GridPos.y + freeTileCount[matchedP.GridPos.x]);
            int newType = RandomType();
            matchedP.SetNewType(newType);
        }
        StartCoroutine(DestroyMatches());
    }

    private IEnumerator DestroyMatches()
    {
        foreach (Piece match in curMatches)
        {
            StartCoroutine(match.TileObject.Deactivate(match.FallPosY, GM.PieceSprites[match.Type]));
        }
        curMatches.Clear();
        //wait for destruction to finish
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(CollapseColumns());
    }

    private bool AnyPieceMoving()
    {
        foreach (Piece p in pieces)
        {
            if (p.TileObject.Moving)
                return true;
        }
        return false;
    }

    private IEnumerator CollapseColumns()
    {
        yield return new WaitForSeconds(0.25f);
        //make everything fall
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Piece piece = pieces[x, y];
                //if (piece == null)
                //    print("nothing in grid at " + x + "," + y);
                //else
                StartCoroutine(piece.TileObject.FallToNewPos(
                    new Vector3(piece.GridPos.x, piece.GridPos.y, 0)));
            }
        }
        yield return null;
        //keep falling until all pieces have reached their destination
        GM.SetGameState(eGameState.falling);
        while (AnyPieceMoving())
            yield return new WaitForSeconds(0.1f);

        GM.SetGameState(eGameState.matching);
        yield return new WaitForSeconds(0.25f);

        if (!CheckWholeGridForMatches())
            GM.SetGameState(eGameState.running);
    }

    /// <summary>
    /// Returns true if there are any matches on the whole board
    /// </summary>
    private bool CheckWholeGridForMatches()
    {
        foreach (Piece piece in pieces)
        {
            CalculateMatches(FruitToCheck(piece));
        }
        if (curMatches.Count > 0)
        {
            RearrangePiecePosInGrid(curMatches);
            return true;
        }
        return false;
    }
}
