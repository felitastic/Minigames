using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates UI text fields
/// </summary>
public class UIUpdates : MonoBehaviour
{
    [SerializeField]
    private Text curScore;
    [SerializeField]
    private Text movesLeft;
    [SerializeField]
    private Text resultScore;


    private void Awake()
    {
        GameManager.OnScoreUpdate += SetScore;
        GameManager.OnMoveChange += SetMovesCount;
        GameManager.OnGameEnd += SetGameresult;
    }

    private void SetScore(int newScore)
    {
        curScore.text = "Gems: " + newScore;
    }

    private void SetMovesCount(int newMoves)
    {
        movesLeft.text = "Moves: " + newMoves;
    }

    private void SetGameresult()
    {
        resultScore.text = GameManager.Instance.FullScore + " ";
    }

}
