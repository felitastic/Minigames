using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject rogue, menuBackground, gameBackground, title, startScreen, gamemodeScreen, ingameScreen, highscoreScreen, resultScreen;
    [SerializeField]
    private SpriteRenderer boardBackground;

    [Header("Keys")]
    [SerializeField]
    private KeyCode quitButton = KeyCode.Escape;
    [SerializeField]
    private KeyCode restartButton = KeyCode.F5;

    [Header("Rogue Fields")]
    [SerializeField]
    private Text speechBubble;

    [Header("Ingame Fields")]
    [SerializeField]
    private Text curScore;
    [SerializeField]
    private Text movesLeft;

    [Header("Game Mode fields")]
    [SerializeField]
    private Dropdown boardSize;
    [SerializeField]
    private Dropdown sessionLength;

    [Header("Result fields")]
    [SerializeField]
    private InputField nameForScoreboard;
    [SerializeField]
    private Text resultScore;


    [Header("---")]
    [SerializeField]
    private CanvasGroup ResultScreen;
    [SerializeField]
    private CanvasGroup StartScreen;
    [SerializeField]
    private CanvasGroup GameModeScreen;
    [SerializeField]
    private CanvasGroup HighScoreScreen;

    [SerializeField]
    private RectTransform[] ScoreBoards = new RectTransform[9];
    private int curBoard = 0;

    private void Awake()
    {
        title.SetActive(true);
        startScreen.SetActive(true);
        menuBackground.SetActive(true);
        rogue.SetActive(true);
        speechBubble.text = "Today's a good day for\n a HEIST!";
        //ShowCanvasGroup(StartScreen);
        //HideCanvasGroup(GameModeScreen);
        //HideCanvasGroup(ResultScreen);
        //HideCanvasGroup(HighScoreScreen);
    }

    private void Start()
    {
        GameManager.OnScoreUpdate += SetScore;
        GameManager.OnMoveChange += SetMovesCount;
        GameManager.OnGameEnd += GameEnded;
        //GameManager.OnSetup += HideResult;
    }

    private void Update()
    {
        if (Input.GetKeyDown(quitButton))
        {
            QuitGame();
        }

        if (Input.GetKeyDown(restartButton))
        {
            GameManager.Instance.ResetGame();
            //blubb restart
        }
    }

    public void PlayButtonPressed()
    {
        gamemodeScreen.SetActive(true);
        speechBubble.text = "The bigger the treasure\nthe easier the heist!";
        startScreen.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.StartNewGame(boardSize.value, sessionLength.value);
        title.SetActive(false);
        rogue.SetActive(false);
        gamemodeScreen.SetActive(false);
        ingameScreen.SetActive(true);
        menuBackground.SetActive(false);
        SetBoardBackgrounds();
    }

    private void SetBoardBackgrounds()
    {
        gameBackground.transform.position = new Vector3(
                            (float)(GameManager.Instance.Width - 1) / 2,
                            (float)(GameManager.Instance.Height - 1) / 2, 0);

        boardBackground.color = new Color(Color.white.a, Color.white.r, Color.white.g, 255);
        boardBackground.transform.position = new Vector3(
                            (float)(GameManager.Instance.Width - 1) / 2,
                            (float)(GameManager.Instance.Height - 1) / 2, 0);
        boardBackground.size = new Vector2(
                            (float)(GameManager.Instance.Width + 1) / 2,
                            (float)(GameManager.Instance.Height + 1) / 2);
    }

    private void HideBoardBackground()
    {
        boardBackground.color = new Color(Color.white.a, Color.white.r, Color.white.g, 0);
    }

    private void SetScore(int newScore)
    {
        curScore.text = "Gems: " + newScore;
    }

    private void SetMovesCount(int newMoves)
    {
        movesLeft.text = "Moves: " + newMoves;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void GameEnded()
    {
        HideBoardBackground();
        ingameScreen.SetActive(false);
        resultScreen.SetActive(true);
        rogue.SetActive(true);
        speechBubble.text = "That was a good haul!";
        resultScore.text = GameManager.Instance.FullScore + " ";
    }

    public void HoverOverButton(int pButtonNo)
    {
        string dialog = speechBubble.text;
        switch (pButtonNo)
        {
            case 1:
                dialog = "I can explain the base rules for stealing!";
                break;
            case 2:
                dialog = "Let's get back to the heist!";
                break;
            case 3:
                dialog = "Wanna regroup in the hideout?";
                break;
            case 4:
                dialog = "One never truly quits!";
                break;
            default:
                LeavingButton();
                break;
        }
        speechBubble.text = dialog;
    }

    public void LeavingButton()
    {
        speechBubble.text = "Need a hand?";
    }

    public void ChangeDialog(int pDialog)
    {
        string dialog = speechBubble.text;
        switch (pDialog)
        {
            case 1:
                dialog = "It's simpel:\nMatch 3 or more gems!";
                break;
            //case 2:
            //    //dialog = "Want me to explain the basics of stealing?";
            //    break;
            //case 3:
            //    dialog = "Wanna regroup in the hideout?";
            //    break;
            //case 4:
            //    dialog = "One never truly quits!";
                //break;
            default:
                LeavingButton();
                break;
        }
        speechBubble.text = dialog;
    }

    //-------------------------

    //private void GameEnded()
    //{
    //    StartCoroutine(ShowResultScreen());
    //}

    //private void HideResult()
    //{
    //    HideCanvasGroup(ResultScreen);
    //}

    //private IEnumerator ShowResultScreen()
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    while (ResultScreen.alpha < 1.0f)
    //    {
    //        ResultScreen.alpha += 0.1f;
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    ShowCanvasGroup(ResultScreen);
    //}

    //private void ShowCanvasGroup(CanvasGroup group)
    //{
    //    group.alpha = 1;
    //    group.blocksRaycasts = true;
    //}

    //private void HideCanvasGroup(CanvasGroup group)
    //{
    //    group.alpha = 0;
    //    group.blocksRaycasts = false;
    //}

    //#region Buttons


    //public void ShowHighscores()
    //{
    //    HideCanvasGroup(StartScreen);
    //    ShowCanvasGroup(HighScoreScreen);
    //}

    //public void CloseHighscores()
    //{
    //    HideCanvasGroup(HighScoreScreen);
    //    ShowCanvasGroup(StartScreen);
    //}

    //public void PlayGame()
    //{
    //    HideCanvasGroup(StartScreen);
    //    ShowCanvasGroup(GameModeScreen);
    //}

    //public void PreviousScoreboard()
    //{
    //    int newBoard = curBoard == 0 ? ScoreBoards.Length - 1 : curBoard - 1;

    //    Vector2 newBoardnewPos = newBoardPos(ScoreBoards[curBoard]);
    //    Vector2 curBoardnewPos = newBoardPos(ScoreBoards[curBoard], 700);

    //    ScoreBoards[newBoard].anchoredPosition = new Vector2(newBoardnewPos.x - 700, newBoardnewPos.y);
    //    StartCoroutine(MoveScoreboard(ScoreBoards[curBoard], curBoardnewPos));
    //    StartCoroutine(MoveScoreboard(ScoreBoards[newBoard], newBoardnewPos));
    //    curBoard = newBoard;
    //}

    //public void NextScoreboard()
    //{
    //    int newBoard = curBoard == ScoreBoards.Length - 1 ? 0 : curBoard + 1;

    //    Vector2 newBoardnewPos = newBoardPos(ScoreBoards[curBoard]);
    //    Vector2 curBoardnewPos = newBoardPos(ScoreBoards[curBoard], -700);

    //    ScoreBoards[newBoard].anchoredPosition = new Vector2(newBoardnewPos.x + 700, newBoardnewPos.y);
    //    StartCoroutine(MoveScoreboard(ScoreBoards[curBoard], curBoardnewPos));
    //    StartCoroutine(MoveScoreboard(ScoreBoards[newBoard], newBoardnewPos));
    //    curBoard = newBoard;
    //}

    //public void ConfirmName()
    //{
    //    string name = nameForScoreboard.text;

    //}

    //#endregion

    //private Vector2 newBoardPos(RectTransform transform, float offset = 0)
    //{
    //    return new Vector2(transform.anchoredPosition.x + offset, transform.anchoredPosition.y);
    //}

    //private IEnumerator MoveScoreboard(RectTransform board, Vector2 newPos, float lerpTime = 0.3f)
    //{
    //    HighScoreScreen.interactable = false;
    //    bool moving = true;
    //    float curLerpTime = 0.0f;
    //    Vector2 startPos = board.anchoredPosition;

    //    while (moving)
    //    {
    //        curLerpTime += Time.deltaTime;
    //        float percentage = curLerpTime / lerpTime;

    //        board.anchoredPosition = Vector2.Lerp(startPos, newPos, percentage);
    //        yield return null;

    //        if (curLerpTime >= lerpTime)
    //        {
    //            moving = false;
    //            yield return null;
    //            HighScoreScreen.interactable = true;
    //            yield break;
    //        }
    //    }
    //}
}
