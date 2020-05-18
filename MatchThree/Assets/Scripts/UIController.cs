using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public KeyCode QuitButton = KeyCode.Escape;
    public KeyCode RestartButton = KeyCode.F5;
    public Text Score;
    public Text MovesLeft;
    public CanvasGroup ResultCanvas;

    private void Awake()
    {
        GameManager.OnScoreUpdate += SetScore;
        GameManager.OnMoveChange += SetMovesCount;
        GameManager.OnRestart += HideResult;
        GameManager.OnGameEnd += GameEnded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(QuitButton))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(RestartButton))
        {
            GameManager.Instance.Restart();    
        }
    }

    private void GameEnded()
    {
        StartCoroutine(ShowResultScreen());
    }

    private void SetScore(int newScore)
    {
        Score.text = "Score: " + newScore;
    }

    private void SetMovesCount(int newMoves)
    {
        MovesLeft.text = "Moves left: " + newMoves;
    }

    private void HideResult()
    {
        ResultCanvas.alpha = 0.0f;
    }

    private IEnumerator ShowResultScreen()
    {
        yield return new WaitForSeconds(0.5f);

        while (ResultCanvas.alpha < 1.0f)
        {
            ResultCanvas.alpha += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
