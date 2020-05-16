using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Rendering;
using System.ComponentModel.Design;
using UnityEditorInternal;
using System.Linq.Expressions;

public class Board : MonoBehaviour
{
    [Header("Parent objects for the pieces and tiles for a cleaner inspector")]
    [SerializeField]
    private Transform pieceSpawn;
    [SerializeField]
    private Transform tileSpawn;
    [Header("Prefabs to spawn")]
    [SerializeField]
    private Piece pieceTile;
    [SerializeField]
    private GameObject bgTile;

    private Queue piecePool = new Queue();

    private GameManager GM;
    private int width { get { return GM.Width; } }
    private int height { get { return GM.Height; } }
    private Fruit[,] pieces { get { return GM.Pieces; } }

    private void Awake()
    {
        GameManager.OnStart += SetGM;        
    }

    private void Start()
    {
        SetBoard();
        PlayerInput.OnPieceMove += MovePiece;
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
        GM.SetGameState(eGameState.running);
    }
    /// <summary>
    /// Spawns the board tiles according to height and width 
    /// </summary>
    private void SpawnBoardTile(int x, int y)
    {
        //Instantiating the tile, renaming for better overview in inspector
        GameObject backgroundTile = Instantiate(bgTile, new Vector2(x, y), Quaternion.identity) as GameObject;
        backgroundTile.transform.SetParent(tileSpawn);
        int ID = y * width + x;
        backgroundTile.name = ID.ToString()+": "+x+","+y;
    }
    /// <summary>
    /// Spawns the pieces on the board while making sure there are no matches at the start
    /// </summary>
    private void SpawnPieces(int x, int y)
    {
        int fruitToUse = Random.Range(0, GM.PieceSprites.Length);
        Piece piece = Instantiate(pieceTile, new Vector2(x, y), Quaternion.identity); //TODO: y+height to offset them for the start?
        piece.transform.SetParent(pieceSpawn);
        GM.SetPiecePosInArray(new Fruit(x, y, piece, fruitToUse), x, y);
        while (PreventMatch(pieces[x,y]))
        {
            fruitToUse = Random.Range(0, GM.PieceSprites.Length);
            pieces[x, y].SetType(fruitToUse);
        }
        pieces[x, y].piece.SetSprite(GM.PieceSprites[fruitToUse]);     
        int ID = y * width + x;
        piece.name = ID.ToString();
    }
     /// <summary>
     /// Returns true if the fruit has a match
     /// </summary>
    private bool PreventMatch(Fruit fruit)
    {
        //3rd spawned piece (no check needed before)
        if (fruit.row >= 2 && fruit.column == 0)
        {
            if (fruit.IsEqual(pieces[fruit.row - 1, fruit.column]) && fruit.IsEqual(pieces[fruit.row - 2, fruit.column]))
                return true;
        }
        //first piece in second row
        else if (fruit.row == 0 && fruit.column == 1)
        {
            if (fruit.IsEqual(pieces[fruit.row, fruit.column-1]) && fruit.IsEqual(pieces[fruit.row + 1, fruit.column-1]))
                return true;
        }
        //pieces spawned after this
        else if (fruit.column > 0)
        {                         
            //get all neighbours of the spawned piece
            Fruit[] neighbours = FruitToCheck(fruit, true);             
                                                                        
            if (fruit.IsEqual(neighbours[1]))                          
            {                                                                  
                    if (fruit.row != width - 1 && fruit.IsEqual(neighbours[5]))
                        return true;
                    else if (fruit.column > 1 && fruit.IsEqual(neighbours[3]))
                        return true;
                    else if (fruit.row != 0 && fruit.IsEqual(neighbours[4]))
                        return true;
                    else if (fruit.row != 0 && fruit.IsEqual(neighbours[0]))
                        return true;
                }
                else if (fruit.row != 0 && fruit.IsEqual(neighbours[0]))
                {
                    if (fruit.IsEqual(neighbours[4]))
                        return true;
                    else if (fruit.row != 1 && fruit.IsEqual(neighbours[2]))
                        return true;
                }            
        }   
        return false;
    }
    /// <summary>
    /// Reacts to the player input, checks if there are matches and if positions need to be swapped
    /// </summary>
    private void MovePiece(Piece piece, eSwipeDir swipeDir)
    {
        Fruit fruit = GM.GetFruit(Mathf.RoundToInt(piece.transform.position.x), Mathf.RoundToInt(piece.transform.position.y));
        Fruit otherfruit = fruit;

        switch (swipeDir)
        {
            case eSwipeDir.none:
                return;
            case eSwipeDir.up:
                otherfruit = GM.GetFruit(fruit.row, fruit.column + 1);
                break;
            case eSwipeDir.down:
                otherfruit = GM.GetFruit(fruit.row, fruit.column - 1);
                break;
            case eSwipeDir.left:
                otherfruit = GM.GetFruit(fruit.row -1, fruit.column);
                break;
            case eSwipeDir.right:
                otherfruit = GM.GetFruit(fruit.row +1, fruit.column);
                break;
            default:
                Debug.Log("Couldn't find the swipeDirection "+swipeDir.ToString()+", canceling movement & match check");
                return;
        }

        if (fruit.IsEqual(otherfruit))
            return;

        fruit.Swap(otherfruit);

        //check if any fruit has a match
        if (CalculateMatches(FruitToCheck(fruit)) | CalculateMatches(FruitToCheck(otherfruit)))
        {             
            StartCoroutine(fruit.piece.SwapPosition(new Vector3(fruit.row, fruit.column,0)));
            StartCoroutine(otherfruit.piece.SwapPosition(new Vector3(otherfruit.row, otherfruit.column,0)));
            return;
        }
        //no matches found, reversing positions again
        StartCoroutine(fruit.piece.SwapPosition(new Vector3(fruit.row, fruit.column, 0), true));
        StartCoroutine(otherfruit.piece.SwapPosition(new Vector3(otherfruit.row, otherfruit.column, 0), true));
        otherfruit.Swap(fruit);
    }
    /*
     * According to the game rules, only the following "neighbours" have to be checked on Move/Spawn
     * 
     * Positions in Grid relative to "movedFruit" (x,y):
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
    private Fruit[] FruitToCheck(Fruit movedFruit, bool startUp = false)
    {
        int row = movedFruit.row;
        int column = movedFruit.column;
        Fruit[] neighbouringFruit = new Fruit[13];

        if (startUp)
        {
            neighbouringFruit[0] = GM.GetFruit(row-1, column);
            neighbouringFruit[1] = GM.GetFruit(row, column-1);
            neighbouringFruit[2] = GM.GetFruit(row-2, column);
            neighbouringFruit[3] = GM.GetFruit(row, column-2);
            neighbouringFruit[4] = GM.GetFruit(row-1, column-1);
            neighbouringFruit[5] = GM.GetFruit(row+1, column-1);
        }
        else
        {
            neighbouringFruit[0] = GM.GetFruit(row, column);
            neighbouringFruit[1] = GM.GetFruit(row, column + 1);
            neighbouringFruit[2] = GM.GetFruit(row, column + 2);
            neighbouringFruit[3] = GM.GetFruit(row - 1, column + 1);
            neighbouringFruit[4] = GM.GetFruit(row + 1, column + 1);
            neighbouringFruit[5] = GM.GetFruit(row, column - 1);
            neighbouringFruit[6] = GM.GetFruit(row, column - 2);
            neighbouringFruit[7] = GM.GetFruit(row + 1, column - 1);
            neighbouringFruit[8] = GM.GetFruit(row - 1, column - 1);
            neighbouringFruit[9] = GM.GetFruit(row - 1, column);
            neighbouringFruit[10] = GM.GetFruit(row - 2, column);
            neighbouringFruit[11] = GM.GetFruit(row + 1, column);
            neighbouringFruit[12] = GM.GetFruit(row + 2, column);
        }
        return neighbouringFruit;
    }
     /// <summary>
     /// Checks for possible matches, returns true if there are enough and calls the destroy method
     /// </summary>
    private bool CalculateMatches(Fruit[] fruitToCheck)
    {
        List<Fruit> AllMatches = Matches(fruitToCheck);

        if (AllMatches.Count < 3)
        {
            return false;
        }

        DestroyFruits(AllMatches);
        return true;
    }
     /// <summary>
     /// Returns a List of the matches for the piece in pos 0 of the given array
     /// For further ref on the array, lookup FruitToCheck method
     /// </summary>
    private List<Fruit> Matches(Fruit[] fruitToCheck)
    {
        Fruit fruit = fruitToCheck[0];
        List<Fruit> matches = new List<Fruit>();
        matches.Add(fruit);

        //upper pieces
        if (fruit.column < height-1 && fruit.IsEqual(fruitToCheck[1]))
        {
            matches.Add(fruitToCheck[1]);
            if (fruit.column < height - 2 && fruit.IsEqual(fruitToCheck[2]))
            {
                matches.Add(fruitToCheck[2]);
            }
            else if (fruit.row > 0 && fruit.IsEqual(fruitToCheck[3]))
            {
                matches.Add(fruitToCheck[3]);
            }
            else if (fruit.row < width-1 && fruit.IsEqual(fruitToCheck[4]))
            {
                matches.Add(fruitToCheck[4]);
            }
        }
        //lower pieces
        if (fruit.column > 0 && fruit.IsEqual(fruitToCheck[5]))
        {
            matches.Add(fruitToCheck[5]);
            if (fruit.column > 1 && fruit.IsEqual(fruitToCheck[6]))
            {
                matches.Add(fruitToCheck[6]);
            }
            else if (fruit.row < width - 1 && fruit.IsEqual(fruitToCheck[7]))
            {
                matches.Add(fruitToCheck[7]);
            }
            else if (fruit.row > 0 && fruit.IsEqual(fruitToCheck[8]))
            {
                matches.Add(fruitToCheck[8]);
            }
        }
        //left pieces
        if (fruit.row > 0 && fruit.IsEqual(fruitToCheck[9]))
        {
            matches.Add(fruitToCheck[9]);
            if (fruit.row > 1 && fruit.IsEqual(fruitToCheck[10]))
            {
                matches.Add(fruitToCheck[10]);
            }
            else if (fruit.column < height-1 && fruit.IsEqual(fruitToCheck[3]))
            {
                if (!matches.Contains(fruitToCheck[3]))
                    matches.Add(fruitToCheck[3]);
            }          
            else if (fruit.column > 0 && fruit.IsEqual(fruitToCheck[8]))
            {
                if (!matches.Contains(fruitToCheck[8]))
                    matches.Add(fruitToCheck[8]);
            }
        }
        //right pieces
        if (fruit.row < width - 1 && fruit.IsEqual(fruitToCheck[11]))
        {
            matches.Add(fruitToCheck[11]);
            if (fruit.row < width - 2 && fruit.IsEqual(fruitToCheck[12]))
            {
                matches.Add(fruitToCheck[12]);
            }
            else if (fruit.column > 0 && fruit.IsEqual(fruitToCheck[7]))
            {
                if (!matches.Contains(fruitToCheck[7]))
                    matches.Add(fruitToCheck[7]);
            }            
            else if (fruit.column < height-1 && fruit.IsEqual(fruitToCheck[4]))
            {
                if (!matches.Contains(fruitToCheck[4]))
                    matches.Add(fruitToCheck[4]);
            }
        }

        return matches;
    }
    /// <summary>
    /// Iterates through the given list and calls the destroy method for each item
    /// </summary>
    private void DestroyFruits(List<Fruit> fruitToDestroy)
    {
        foreach(Fruit fruit in fruitToDestroy)
        {
            piecePool.Enqueue(fruit);
            StartCoroutine(fruit.piece.Deactivate());
        }
    }
}
