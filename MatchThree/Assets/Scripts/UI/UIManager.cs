
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages screen and scene changes
/// </summary>
public class UIManager : Singleton<UIManager>
{
    protected UIManager() { }
    public eMenuScene Scene { get; private set; }

    [SerializeField]
    private GameObject rogue, menuBackground, gameBackground, boardpieceParent, title, startScreen, gamemodeScreen, ingameScreen, helpScreen, tutorialscreen, highscoreScreen, normalResultScreen, scoringResultScreen;
    [SerializeField]
    private SpriteRenderer boardBackground;

    public int HighscorePage { get; private set; }


    [Header("Game Mode fields")]
    [SerializeField]
    private Dropdown boardSize;
    [SerializeField]
    private Dropdown sessionLength;

    [Header("Result fields")]
    [SerializeField]
    private InputField nameForScoreboard;



    //[Header("---")]
    //[SerializeField]
    //private CanvasGroup ResultScreen;
    //[SerializeField]
    //private CanvasGroup StartScreen;
    //[SerializeField]
    //private CanvasGroup GameModeScreen;
    //[SerializeField]
    //private CanvasGroup HighScoreScreen;

    //[SerializeField]
    //private RectTransform[] ScoreBoards = new RectTransform[9];
    //private int curBoard = 0;

    private void Awake()
    {
        SwitchScene(eMenuScene.Start);
        HighscorePage = 0;
        //ShowCanvasGroup(StartScreen);
        //HideCanvasGroup(GameModeScreen);
        //HideCanvasGroup(ResultScreen);
        //HideCanvasGroup(HighScoreScreen);
    }

    private void Start()
    {

        GameManager.OnGameEnd += GameEnded;
        //GameManager.OnSetup += HideResult;
    }


    /// <summary>
    /// Returns the gamemode values entered by the player: x = board size, y = session length
    /// </summary>
    public Vector2Int GameModeValues()
    {
        return new Vector2Int(boardSize.value, sessionLength.value);
    }

    /// <summary>
    /// Controls which visual elements are visible
    /// </summary>
    /// <param name="newScene"></param>
    public void SwitchScene(eMenuScene newScene)
    {
        Scene = newScene;
        switch (Scene)
        {
            case eMenuScene.Start:
                title.SetActive(true);
                rogue.SetActive(true);
                menuBackground.SetActive(true);
                startScreen.SetActive(true);

                break;
            case eMenuScene.GameMode:
                title.SetActive(false);
                rogue.SetActive(true);
                startScreen.SetActive(false);
                gamemodeScreen.SetActive(true);

                break;
            case eMenuScene.Ingame:
                BoardPieceVisibility(true);
                rogue.SetActive(false);
                gamemodeScreen.SetActive(false);
                menuBackground.SetActive(false);
                ingameScreen.SetActive(true);
                PositionGameBackgrounds();
                BoardBGStatus(true);

                break;
            case eMenuScene.Help:
                rogue.SetActive(true);
                helpScreen.SetActive(true);
                ingameScreen.SetActive(false);
                tutorialscreen.SetActive(false);

                break;
            case eMenuScene.Tutorial:
                helpScreen.SetActive(false);
                tutorialscreen.SetActive(true);

                break;
            case eMenuScene.NormalResult:
                BoardPieceVisibility(false);
                BoardBGStatus(false);
                ingameScreen.SetActive(false);
                normalResultScreen.SetActive(true);
                scoringResultScreen.SetActive(false);
                rogue.SetActive(true);

                break;
            case eMenuScene.ScoringResult:
                BoardPieceVisibility(false);
                BoardBGStatus(false);
                ingameScreen.SetActive(false);
                normalResultScreen.SetActive(true);
                scoringResultScreen.SetActive(true);
                rogue.SetActive(true);
                break;
            case eMenuScene.Highscore:

                break;
            case eMenuScene.Credits:

                break;
            default:
                Debug.LogWarning("Could not find enum " + Scene.ToString());
                break;
        }
    }

    /// <summary>
    /// Centers the game background image for the game
    /// </summary>
    private void PositionGameBackgrounds()
    {
        gameBackground.transform.position = new Vector3(
                            (float)(GameManager.Instance.Width - 1) / 2,
                            (float)(GameManager.Instance.Height - 1) / 2, 0);
    }

    /// <summary>
    /// True: Updates the board behind the gems to fit the board size and sets image alpha to 255
    /// False: Sets board image's alpha to 0
    /// </summary>
    private void BoardBGStatus(bool show)
    {
        if (show)
        {
            boardBackground.color = new Color(Color.white.a, Color.white.r, Color.white.g, 255);
            boardBackground.transform.position = new Vector3(
                                (float)(GameManager.Instance.Width - 1) / 2,
                                (float)(GameManager.Instance.Height - 1) / 2, 0);
            boardBackground.size = new Vector2(
                                (float)(GameManager.Instance.Width + 1) / 2,
                                (float)(GameManager.Instance.Height + 1) / 2);
        }
        else
        {
            boardBackground.color = new Color(Color.white.a, Color.white.r, Color.white.g, 0);
        }
    }

    private void BoardPieceVisibility(bool show)
    {
        boardpieceParent.SetActive(show);
    }

    private void GameEnded()
    {
        SwitchScene(eMenuScene.NormalResult);
    }

    public void UpdateHighscorePage(bool plus)
    {
        if (plus)
        {
            HighscorePage = HighscorePage < 2 ? HighscorePage + 1 : 0; 
        }
        else
        {
            HighscorePage = HighscorePage > 0 ? HighscorePage - 1 : 2; 
        }
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
