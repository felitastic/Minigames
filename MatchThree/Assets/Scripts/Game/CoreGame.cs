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
        PlayerInput.OnPieceMove += CheckForMatchesOnInput;
    }

    /// <summary>
    /// On Startup, GM makes a call to tell other scripts it is ready
    /// </summary>
    private void SetGM(GameManager GM)
    {
        this.GM = GM;
    }

    /// <summary>
    /// Reacts to the players input
    /// </summary>
    private void CheckForMatchesOnInput(Tile tile, eSwipeDir swipeDir)
    {
        Piece _piece = GM.GetPieceAt(Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.y));
        Piece _otherPiece = _piece;

        switch (swipeDir)
        {
            case eSwipeDir.none:
                return;
            case eSwipeDir.up:
                _otherPiece = GM.GetPieceAt(_piece.GridPos.x, _piece.GridPos.y + 1);
                break;
            case eSwipeDir.down:
                _otherPiece = GM.GetPieceAt(_piece.GridPos.x, _piece.GridPos.y - 1);
                break;
            case eSwipeDir.left:
                _otherPiece = GM.GetPieceAt(_piece.GridPos.x - 1, _piece.GridPos.y);
                break;
            case eSwipeDir.right:
                _otherPiece = GM.GetPieceAt(_piece.GridPos.x + 1, _piece.GridPos.y);
                break;
            default:
                Debug.Log("Couldn't find the swipeDirection " + swipeDir.ToString() + ", canceling movement & match check");
                break;
        }

        // if the pieces are the same, show attempted swapping
        if (_piece.MatchesWith(_otherPiece))
        {
            StartCoroutine(_piece.TileObject.SwapPosition(new Vector3(_otherPiece.GridPos.x, _otherPiece.GridPos.y, 0), true));
            StartCoroutine(_otherPiece.TileObject.SwapPosition(new Vector3(_piece.GridPos.x, _piece.GridPos.y, 0), true));
            GM.SetGameState(eGameState.running);
            return;
        }
        // if they are not the same, swap, then check for matches
        else
        {
            _piece.Swap(_otherPiece);
            if (CalculateMatches(GM.FruitToCheck(_piece)) | CalculateMatches(GM.FruitToCheck(_otherPiece)))
            {
                SwapPieces(_piece, _otherPiece);
                RearrangePiecePositions(curMatches);                
                return;
            }
            else
            {
                _piece.Swap(_otherPiece);
                StartCoroutine(_piece.TileObject.SwapPosition(new Vector3(_otherPiece.GridPos.x, _otherPiece.GridPos.y, 0), true));
                StartCoroutine(_otherPiece.TileObject.SwapPosition(new Vector3(_piece.GridPos.x, _piece.GridPos.y, 0), true));
                GM.SetGameState(eGameState.running);
                return;
            }
        }




        //if they are of the same type, play the "could not swap" animation
        if (_piece.MatchesWith(_otherPiece))
        {
            StartCoroutine(_piece.TileObject.SwapPosition(new Vector3(_otherPiece.GridPos.x, _otherPiece.GridPos.y, 0), true));
            StartCoroutine(_otherPiece.TileObject.SwapPosition(new Vector3(_piece.GridPos.x, _piece.GridPos.y, 0), true));
            GM.SetGameState(eGameState.running);
            return;
        }

        _piece.Swap(_otherPiece);

        //check if any piece has a match
        //both match = double score
        if (CalculateMatches(GM.FruitToCheck(_piece)) && CalculateMatches(GM.FruitToCheck(_otherPiece)))
        {
            //print(piece.tile.name + " swap Pos: " + piece.GridPos.x + "," + piece.GridPos.y);
            //print(otherPiece.tile.name + " swap Pos: " + otherPiece.GridPos.x + "," + otherPiece.GridPos.y);
            tempScore = ScoreMultipliedBy(2);
            SwapPieces(_piece, _otherPiece);
            return;
        }
        else if (CalculateMatches(GM.FruitToCheck(_piece)) | CalculateMatches(GM.FruitToCheck(_otherPiece)))
        {
            //print(piece.tile.name + " swap Pos: " + piece.GridPos.x + "," + piece.GridPos.y);
            //print(otherPiece.tile.name + " swap Pos: " + otherPiece.GridPos.x + "," + otherPiece.GridPos.y);
            SwapPieces(_piece, _otherPiece);
            return;
        }
        //no matches found, play "could not swap" animation and put them in their original positions
        StartCoroutine(_piece.TileObject.SwapPosition(new Vector3(_piece.GridPos.x, _piece.GridPos.y, 0), true));
        StartCoroutine(_otherPiece.TileObject.SwapPosition(new Vector3(_otherPiece.GridPos.x, _otherPiece.GridPos.y, 0), true));
        _otherPiece.Swap(_piece);
        GM.SetGameState(eGameState.running);
    }

    /// <summary>
    /// Swaps the two pieces and subtracts a move
    /// </summary>
    private void SwapPieces(Piece piece, Piece otherPiece)
    {
        GM.SubstractOneMove();
        StartCoroutine(piece.TileObject.SwapPosition(new Vector3(piece.GridPos.x, piece.GridPos.y, 0)));
        StartCoroutine(otherPiece.TileObject.SwapPosition(new Vector3(otherPiece.GridPos.x, otherPiece.GridPos.y, 0)));        
    }

    /// <summary>
    /// Clears all pieces in the given list, gives the empty pieces a new type and makes all pieces fall down 
    /// </summary>
    private void RearrangePiecePositions(List<Piece> matchedPieces)
    {
        //how many tiles are going to be empty due to destroyed matches
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
            StartCoroutine(match.TileObject.Deactivate(match.FallPosY, match.Type));
            GameManager.Instance.AddToScore(1);
        }
        curMatches.Clear();
        ResetScore();
        //wait for destruction to finish
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(CollapseColumns());
    }

    /// <summary>
    /// Starts falling animation for all pieces, waits until every piece has stopped moving
    /// </summary>
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
    /// True if any piece on the board still has Moving = true
    /// </summary>
    private bool AnyPieceMoving()
    {
        foreach (Piece p in GM.Pieces)
        {
            if (p.TileObject.Moving)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns true if there are matches
    /// </summary>
    private bool CalculateMatches(Piece[] piecesToCheck)
    {
        List<Piece> allMatches = Matches(piecesToCheck);
        if (allMatches.Count < 3)       
            return false;

        int _tempScore = 0;
        
        //foreach (Piece p in allMatches)
        //{
        //    p.TileObject.Matched();
        //}
        //make sure we have no doubles in the list
        foreach (Piece piece in allMatches)
        {
            if (!curMatches.Contains(piece))
            {
                curMatches.Add(piece);
                //_tempScore += 1;
            }
            //this.tempScore += CalculateMatchScore(_tempScore);
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
    /// Adds temporary score to the final one and sets temp score to zero
    /// </summary>
    private void ResetScore()
    {
        GM.AddToScore(tempScore);
        tempScore = 0;
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
            RearrangePiecePositions(curMatches);
            return true;
        }
        return false;
    }
}
