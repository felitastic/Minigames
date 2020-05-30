using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private KeyCode QuitButton = KeyCode.Escape;
    [SerializeField]
    private KeyCode RestartButton = KeyCode.F5;
    [SerializeField]
    private Text Score;
    [SerializeField]
    private Text MovesLeft;
    [SerializeField]
    private CanvasGroup ResultScreen;
    [SerializeField]
    private CanvasGroup StartScreen;
    [SerializeField]
    private CanvasGroup GameModeScreen;
    [SerializeField]
    private CanvasGroup HighScoreScreen;
    [SerializeField]
    private Dropdown BoardSize;
    [SerializeField]
    private Dropdown SessionLength;
    [SerializeField]
    private InputField nameForScoreboard;
    [SerializeField]
    private RectTransform[] ScoreBoards = new RectTransform[9];
    private int curBoard = 0;

    private void Awake()
    {
        ShowCanvasGroup(StartScreen);
        HideCanvasGroup(GameModeScreen);
        HideCanvasGroup(ResultScreen);
        HideCanvasGroup(HighScoreScreen);
    }

    private void Start()
    {
        GameManager.OnScoreUpdate += SetScore;
        GameManager.OnMoveChange += SetMovesCount;
        GameManager.OnSetup += HideResult;
        GameManager.OnGameEnd += GameEnded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(QuitButton))
        {
            QuitGame();
        }

        if (Input.GetKeyDown(RestartButton))
        {
            GameManager.Instance.ResetGame();
            //blubb restart
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
        HideCanvasGroup(ResultScreen);
    }

    private IEnumerator ShowResultScreen()
    {
        yield return new WaitForSeconds(0.5f);

        while (ResultScreen.alpha < 1.0f)
        {
            ResultScreen.alpha += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        ShowCanvasGroup(ResultScreen);
    }

    private void ShowCanvasGroup(CanvasGroup group)
    {
        group.alpha = 1;
        group.blocksRaycasts = true;
    }

    private void HideCanvasGroup(CanvasGroup group)
    {
        group.alpha = 0;
        group.blocksRaycasts = false;
    }

    #region Buttons
    public void StartGame()
    {
        GameManager.Instance.StartNewGame(BoardSize.value, SessionLength.value);
        StartScreen.alpha = 0;
        GameModeScreen.alpha = 0;
    }

    public void ShowHighscores()
    {
        HideCanvasGroup(StartScreen);
        ShowCanvasGroup(HighScoreScreen);
    }

    public void CloseHighscores()
    {
        HideCanvasGroup(HighScoreScreen);
        ShowCanvasGroup(StartScreen);
    }

    public void PlayGame()
    {
        HideCanvasGroup(StartScreen);
        ShowCanvasGroup(GameModeScreen);
    }

    public void QuitGame()
    {
        Application.Quit();
    } 

    public void PreviousScoreboard()
    {
        int newBoard = curBoard == 0 ? ScoreBoards.Length - 1 : curBoard - 1;

        Vector2 newBoardnewPos = newBoardPos(ScoreBoards[curBoard]);
        Vector2 curBoardnewPos = newBoardPos(ScoreBoards[curBoard], 700);

        ScoreBoards[newBoard].anchoredPosition = new Vector2(newBoardnewPos.x - 700, newBoardnewPos.y);
        StartCoroutine(MoveScoreboard(ScoreBoards[curBoard], curBoardnewPos));
        StartCoroutine(MoveScoreboard(ScoreBoards[newBoard], newBoardnewPos));
        curBoard = newBoard;
    }

    public void NextScoreboard()
    {
        int newBoard = curBoard == ScoreBoards.Length - 1 ? 0 : curBoard + 1;

        Vector2 newBoardnewPos = newBoardPos(ScoreBoards[curBoard]);
        Vector2 curBoardnewPos = newBoardPos(ScoreBoards[curBoard], -700);

        ScoreBoards[newBoard].anchoredPosition = new Vector2(newBoardnewPos.x + 700, newBoardnewPos.y);
        StartCoroutine(MoveScoreboard(ScoreBoards[curBoard], curBoardnewPos));
        StartCoroutine(MoveScoreboard(ScoreBoards[newBoard], newBoardnewPos));
        curBoard = newBoard;
    }

    public void ConfirmName()
    {
        string name = nameForScoreboard.text;

    }

    #endregion

    private Vector2 newBoardPos(RectTransform transform, float offset = 0)
    {
        return new Vector2(transform.anchoredPosition.x + offset, transform.anchoredPosition.y);
    }

    private IEnumerator MoveScoreboard(RectTransform board, Vector2 newPos, float lerpTime = 0.3f)
    {
        HighScoreScreen.interactable = false;
        bool moving = true;
        float curLerpTime = 0.0f;
        Vector2 startPos = board.anchoredPosition;

        while (moving)
        {
            curLerpTime += Time.deltaTime;
            float percentage = curLerpTime / lerpTime;

            board.anchoredPosition = Vector2.Lerp(startPos, newPos, percentage);
            yield return null;

            if (curLerpTime >= lerpTime)
            {
                moving = false;
                yield return null;
                HighScoreScreen.interactable = true;
                yield break;
            }
        }
    }
}
