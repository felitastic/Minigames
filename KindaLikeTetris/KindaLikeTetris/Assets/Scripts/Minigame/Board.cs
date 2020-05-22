using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private LevelSetting[] Levels;
    private LevelSetting curLevel;


    public eMinigameState State { get; private set; }

    private TetrisMovement player;

    [SerializeField]
    private Tile tilePrefab;

    //HACK: test ui
    //public GameObject ReplayButton;
    //public TMPro.TextMeshProUGUI Result;

    //TODO: get these from the SO
    //[SerializeField]
    //private int[] goalPos;
    //[SerializeField]
    //private int[] obstaclePos;

    //[SerializeField]
    //private int width = 10;
    //[SerializeField]
    //private int height = 19;

    public int Width { get { return curLevel.BoardWidth; } }
    public int Height { get { return curLevel.BoardHeight; } }
    public TileInfo[] tiles { get; private set; }
    public int TileSize { get { return curLevel.TileSize; } } 

    private void Awake()
    {        
        ChangeGameState(eMinigameState.paused);
        //HACK temporary, make random or menu later
        curLevel = Levels[0];
        StartCoroutine(BoardSetup());
    }

    private void OnDisable()
    {
        State = eMinigameState.paused;
    }

    private IEnumerator BoardSetup()
    {
        SpawnBoard();
        SpawnPlayerTile(ePlayerShape.TBlock);
        SetGoal();
        SetObstacles();
        yield return new WaitForSeconds(0.5f);
        ChangeGameState(eMinigameState.ready);
        StartCoroutine(CheckForStartInput());
    }

    private IEnumerator CheckForStartInput()
    {
        while (State == eMinigameState.ready)
        {
            if (Input.anyKeyDown)
            {
                ChangeGameState(eMinigameState.running);
                StartCoroutine(player.AutomaticDropping());
                yield break;
            }
            yield return null;
        }
    }

    private void SpawnBoard()
    {
        tiles = new TileInfo[Height * Width];
        int tileCount = 0;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile newTile = Instantiate(tilePrefab);
                newTile.transform.SetParent(this.gameObject.transform);                
                newTile.rectTrans.anchoredPosition = 
                    new Vector3(x * TileSize - curLevel.XOffset, -y * TileSize + curLevel.YOffset, 0);
                newTile.name = "tile("+tileCount+")";
                newTile.PosInArray = tileCount;
                newTile.rectTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                if (x == 0 || x == Width-1 || y == Height-1 || y == 0)
                { 
                    tiles[tileCount] = new TileInfo(newTile, ePosState.border);
                    newTile.SetColor(Color.blue);
                }
                else
                {
                    tiles[tileCount] = new TileInfo(newTile, ePosState.free);
                    newTile.SetColor(Color.cyan);
                }
                tileCount++;
            }
        }
    }

    private void SetObstacles()
    {
        for (int i = 0; i < curLevel.ObstaclePositions.Length; i++)
        {
            tiles[curLevel.ObstaclePositions[i]].Update(ePosState.obstacle);
            tiles[curLevel.ObstaclePositions[i]].tile.SetColor(Color.gray);            
        }
    }

    private void SetGoal()
    {
        for (int i = 0; i < curLevel.GoalPositions.Length; i++)
        {
            tiles[curLevel.GoalPositions[i]].Update(ePosState.goal);
            tiles[curLevel.GoalPositions[i]].tile.SetColor(Color.magenta);
        }
    }

    // LBlock = 0, TBlock = 1, IBlock = 2, OBlock = 3, SBlock = 4, ZBlock = 5, JBlock = 6 
    private void SpawnPlayerTile(ePlayerShape playerShape)
    {
        player = Instantiate(curLevel.PlayerShape);
        player.transform.SetParent(this.gameObject.transform);

        int startPos = player.startPos - Width;

        player.rectTrans.anchoredPosition = 
            new Vector3(startPos*TileSize - curLevel.XOffset, -1 * TileSize + curLevel.YOffset, 0);
            //new Vector3(player.startPos * Size - curLevel.XOffset, -1 * Size + curLevel.YOffset, 0);

        player.rectTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        player.SetBoard(this);
    }

    public void ChangeGameState(eMinigameState newState)
    {
        State = newState;
    }

    public void GameResult(bool win)
    {
        if (win)
        {
            ChangeGameState(eMinigameState.Win);
        }
        else
        {
            ChangeGameState(eMinigameState.Lose);
        }
        print(State.ToString());

        //StartCoroutine(EndGame());
    }

    //private IEnumerator EndGame()
    //{
    //    while (BlackScreen.alpha < 1.0f)
    //    {
    //        BlackScreen.alpha += 0.1f;
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    OnGameEnd(tempAbility, true);
    //    this.gameObject.SetActive(false);
    //}

        //HACK temporary button call to reset the minigame
        public void ReplayGame()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
    }
}


