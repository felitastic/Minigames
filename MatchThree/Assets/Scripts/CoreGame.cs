using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Controls the basic logic of the game, such as Matching, swapping and destroying pieces
/// </summary>
public class CoreGame : MonoBehaviour
{
    private int tempScore;
    private List<Piece> curMatches = new List<Piece>();
    [SerializeField]
    private GameManager GM;

    private void Awake()
    {
        GameManager.OnStart += SetGM;
        PlayerInput.OnPieceMove += SwapPieces;
    }

    /// <summary>
    /// On Startup, GM makes a call to tell other scripts it is ready
    /// </summary>
    private void SetGM(GameManager GM)
    {
        this.GM = GM;
    }

    /// <summary>
    /// Reacts to the player input, checks if there are matches and if positions need to be swapped
    /// </summary>
    private void SwapPieces(Tile tile, eSwipeDir swipeDir)
    {
        Piece piece = GM.GetPieceAt(Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.y));
        Piece otherPiece = piece;

        switch (swipeDir)
        {
            case eSwipeDir.none:
                return;
            case eSwipeDir.up:
                otherPiece = GM.GetPieceAt(piece.GridPos.x, piece.GridPos.y + 1);
                break;
            case eSwipeDir.down:
                otherPiece = GM.GetPieceAt(piece.GridPos.x, piece.GridPos.y - 1);
                break;
            case eSwipeDir.left:
                otherPiece = GM.GetPieceAt(piece.GridPos.x - 1, piece.GridPos.y);
                break;
            case eSwipeDir.right:
                otherPiece = GM.GetPieceAt(piece.GridPos.x + 1, piece.GridPos.y);
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
        if (CalculateMatches(GM.FruitToCheck(piece)) && CalculateMatches(GM.FruitToCheck(otherPiece)))
        {
            //print(piece.tile.name + " swap Pos: " + piece.GridPos.x + "," + piece.GridPos.y);
            //print(otherPiece.tile.name + " swap Pos: " + otherPiece.GridPos.x + "," + otherPiece.GridPos.y);
            tempScore = ScoreMultipliedBy(2);
            SuccessfulSwap(piece, otherPiece);
            return;
        }
        else if (CalculateMatches(GM.FruitToCheck(piece)) | CalculateMatches(GM.FruitToCheck(otherPiece)))
        {
            //print(piece.tile.name + " swap Pos: " + piece.GridPos.x + "," + piece.GridPos.y);
            //print(otherPiece.tile.name + " swap Pos: " + otherPiece.GridPos.x + "," + otherPiece.GridPos.y);
            SuccessfulSwap(piece, otherPiece);
            return;
        }
        //no matches found, play "could not swap" animation and put them in their original positions
        StartCoroutine(piece.TileObject.SwapPosition(new Vector3(piece.GridPos.x, piece.GridPos.y, 0), true));
        StartCoroutine(otherPiece.TileObject.SwapPosition(new Vector3(otherPiece.GridPos.x, otherPiece.GridPos.y, 0), true));
        otherPiece.Swap(piece);
        GM.SetGameState(eGameState.running);
    }

    private void SuccessfulSwap(Piece piece, Piece otherPiece)
    {
        GM.SubstractOneMove();
        StartCoroutine(piece.TileObject.SwapPosition(new Vector3(piece.GridPos.x, piece.GridPos.y, 0)));
        StartCoroutine(otherPiece.TileObject.SwapPosition(new Vector3(otherPiece.GridPos.x, otherPiece.GridPos.y, 0)));
        RearrangePiecePosInGrid(curMatches);
    }


    /// <summary>
    /// Checks for possible matches, returns true if there are enough and calls the destroy method
    /// </summary>
    private bool CalculateMatches(Piece[] piecesToCheck)
    {
        List<Piece> allMatches = Matches(piecesToCheck);
        int tempScore = 0;

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
            {
                curMatches.Add(piece);
                tempScore += 1;
            }
            this.tempScore += CalculateMatchScore(tempScore);
        }
        return true;
    }

    private int CalculateMatchScore(int oneMatchScore)
    {
        int bonus = 1;

        switch (oneMatchScore)
        {
            case 1:
                bonus = 1;
                break;
            case 2:
                bonus = 1;
                break;
            case 3:
                bonus = 1;
                break;
            case 4:
                bonus = 1;
                break;
            case 5:
                bonus = 2;
                break;
            case 6:
                bonus = 3;
                break;
            case 7:
                bonus = 4;
                break;
            default:
                break;
        }

        return oneMatchScore * bonus;
    }

    private int ScoreMultipliedBy(int matchCount)
    {
        return tempScore *= matchCount;
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
        if (piece.GridPos.y < GM.Height - 1 && piece.MatchesWith(piecesToCheck[1]))
        {
            matches.Add(piecesToCheck[1]);
            if (piece.GridPos.y < GM.Height - 2 && piece.MatchesWith(piecesToCheck[2]))
            {
                matches.Add(piecesToCheck[2]);
            }
            else if (piece.GridPos.x > 0 && piece.MatchesWith(piecesToCheck[3]))
            {
                matches.Add(piecesToCheck[3]);
            }
            else if (piece.GridPos.x < GM.Width - 1 && piece.MatchesWith(piecesToCheck[4]))
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
            else if (piece.GridPos.x < GM.Width - 1 && piece.MatchesWith(piecesToCheck[7]))
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
            else if (piece.GridPos.y < GM.Height - 1 && piece.MatchesWith(piecesToCheck[3]))
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
        if (piece.GridPos.x < GM.Width - 1 && piece.MatchesWith(piecesToCheck[11]))
        {
            matches.Add(piecesToCheck[11]);
            if (piece.GridPos.x < GM.Width - 2 && piece.MatchesWith(piecesToCheck[12]))
            {
                matches.Add(piecesToCheck[12]);
            }
            else if (piece.GridPos.y > 0 && piece.MatchesWith(piecesToCheck[7]))
            {
                if (!matches.Contains(piecesToCheck[7]))
                    matches.Add(piecesToCheck[7]);
            }
            else if (piece.GridPos.y < GM.Height - 1 && piece.MatchesWith(piecesToCheck[4]))
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
        int[] freeTileCount = new int[GM.Width];

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
            for (int y = start; y < GM.Height; y++)
            {
                Piece upperPiece = GM.GetPieceAt(posX, y);
                matchedP.Swap(upperPiece);
            }
        }

        //set fall pos and new type for each matched piece
        foreach (Piece matchedP in matchedPieces)
        {
            matchedP.SetFallPosition(matchedP.GridPos.y + freeTileCount[matchedP.GridPos.x]);
            int newType = GM.RandomType();
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
        ResetScore();
        //wait for destruction to finish
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(CollapseColumns());
    }

    /// <summary>
    /// Adds temporary score to the final one and sets temp score to zero
    /// </summary>
    private void ResetScore()
    {
        GM.AddToScore(tempScore);
        tempScore = 0;
    }

    private bool AnyPieceMoving()
    {
        foreach (Piece p in GM.Pieces)
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
        for (int x = 0; x < GM.Width; x++)
        {
            for (int y = 0; y < GM.Height; y++)
            {
                Piece piece = GM.GetPieceAt(x, y);
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

        if (!CheckWholeGridForMatches() && !GM.GameEnd())
            GM.SetGameState(eGameState.running);
    }

    /// <summary>
    /// Returns true if there are any matches on the whole board
    /// </summary>
    private bool CheckWholeGridForMatches()
    {
        int matchCount = 1;

        foreach (Piece piece in GM.Pieces)
        {
            if (CalculateMatches(GM.FruitToCheck(piece)) && !curMatches.Contains(piece))
                matchCount += 1;
        }
        if (curMatches.Count > 0)
        {
            tempScore = ScoreMultipliedBy(matchCount);
            RearrangePiecePosInGrid(curMatches);
            return true;
        }
        return false;
    }
}
