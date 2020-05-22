using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all tetris shapes
/// </summary>
public class TetrisMovement : MonoBehaviour
{
    [SerializeField]
    protected ePlayerShape playerShape = ePlayerShape.TBlock;
    [SerializeField]
    protected eMoveDir moveDir = eMoveDir.down;
    [SerializeField]
    protected eRotation curRotation = eRotation.up;

    protected enum eMoveDir { down, left, right };
    protected enum eRotation { up = 0, right = 1, down = 2, left = 3 };

    [SerializeField]
    protected float defaultdropTime = 1.0f;
    [SerializeField]
    protected float fastdropTime = 0.5f;
    protected float dropTimer;
    public int startPos { get; protected set; }
    public RectTransform rectTrans { get; protected set; }

    [SerializeField]
    [Tooltip("Wait between sideway moves while button is pressed")]
    protected float movePause = 0.25f;
    protected bool fastDrop;

    protected int[] curPos;

    protected Board board;

    public static event System.Action GameEnded = delegate { };

    protected virtual void Awake()
    {
        startPos = 15;
        rectTrans = GetComponent<RectTransform>();
        fastDrop = false;
        dropTimer = defaultdropTime;
    }

    protected virtual void Start()
    {
        StartCoroutine(Setup());
    }

    protected virtual IEnumerator Setup()
    {
        curRotation = eRotation.down;
        curPos = PosOnGrid(startPos, curRotation);
        yield return new WaitForSeconds(0.05f);
        board.ChangeGameState(eMinigameState.ready);
    }

    public void SetBoard(Board board)
    {
        this.board = board;
    }

    /// <summary>
    /// Returns an array of the positions in grid for current shape with given rotation and pivot (basePos)
    /// </summary>
    protected virtual int[] PosOnGrid(int basePos, eRotation rotation)
    {
        int[] newPlayerPos = new int[4];
        newPlayerPos[0] = basePos;      //0 is always the center pos and pivot of the shape

        switch (rotation)
        {
            case eRotation.up:
                newPlayerPos[1] = basePos - 1;
                newPlayerPos[2] = basePos + 1;
                newPlayerPos[3] = basePos - board.Width;

                break;
            case eRotation.right:
                newPlayerPos[1] = basePos - board.Width;
                newPlayerPos[2] = basePos + board.Width;
                newPlayerPos[3] = basePos + 1;

                break;
            case eRotation.down:
                newPlayerPos[1] = basePos - 1;
                newPlayerPos[2] = basePos + 1;
                newPlayerPos[3] = basePos + board.Width;

                break;
            case eRotation.left:
                newPlayerPos[1] = basePos - board.Width;
                newPlayerPos[2] = basePos + board.Width;
                newPlayerPos[3] = basePos - 1;

                break;
            default:
                break;
        }
        return newPlayerPos;
    }

    protected virtual void Update()
    {
        if (board.State == eMinigameState.running)
        {
            if (Input.GetButton("Down"))
            {
                fastDrop = true;
            }
            else
            {
                fastDrop = false;
            }

            if (Input.GetButton("Right") && moveDir == eMoveDir.down)
            {
                StartCoroutine(MoveRight());
            }
            else if (Input.GetButton("Left") && moveDir == eMoveDir.down)
            {
                StartCoroutine(MoveLeft());
            }

            if (Input.GetButtonDown("RotateRight"))
            {
                Rotate(false);
            }
            else if (Input.GetButtonDown("RotateLeft"))
            {
                Rotate(true);
            }
        }
    }

    protected virtual void Rotate(bool left)
    {
        int rotate;
        int angle;

        angle = left ? -90 : +90;
        rotate = left ? +1 : -1;

        if (!((int)curRotation + rotate < 4 && (int)curRotation + rotate >= 0))
        {
            rotate = left ? -3 : +3;
        }

        eRotation newRotation = curRotation + rotate;
        int[] newPos = PosOnGrid(curPos[0], newRotation);

        if (!IsPosFree(newPos))
        {
            print("cannot rotate");
            return;
        }

        this.transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, angle));
        curRotation = newRotation;
        curPos = newPos;
    }

    protected virtual bool IsInGoal(int[] posToCheck)
    {
        for (int i = 0; i < posToCheck.Length; i++)
        {
            if (board.tiles[posToCheck[i]].state != ePosState.goal)
                return false;
        }
        return true;
    }     

    protected virtual bool IsPosFree(int[] posToCheck)
    {
        for (int i = 0; i < posToCheck.Length; i++)
        {
            if (posToCheck[i] < 0)
                continue;

            if (board.tiles[posToCheck[i]].state == ePosState.border || board.tiles[posToCheck[i]].state == ePosState.obstacle)
                return false;
        }
        return true;
    }

    protected virtual IEnumerator MoveLeft()
    {
        moveDir = eMoveDir.left;

        while (moveDir == eMoveDir.left)
        {
            int[] newPos = PosOnGrid(curPos[0] - 1, curRotation);

            if (!IsPosFree(newPos))
            {
                moveDir = eMoveDir.down;
                yield break;
            }

            rectTrans.anchoredPosition = new Vector3(
                                rectTrans.anchoredPosition.x - board.TileSize,
                                rectTrans.anchoredPosition.y,
                                0);

            curPos = newPos;
            yield return new WaitForSeconds(movePause);

            if (moveDir != eMoveDir.down && !Input.GetButton("Left"))
                moveDir = eMoveDir.down;
        }
    }

    protected virtual IEnumerator MoveRight()
    {
        moveDir = eMoveDir.right;

        while (moveDir == eMoveDir.right)
        {
            int[] newPos = PosOnGrid(curPos[0] + 1, curRotation);

            //0 is always the center pos of the shape
            if (!IsPosFree(newPos))
            {
                moveDir = eMoveDir.down;
                yield break;
            }

            rectTrans.anchoredPosition = new Vector3(
                                rectTrans.anchoredPosition.x + board.TileSize,
                                rectTrans.anchoredPosition.y,
                                0);

            curPos = newPos;
            yield return new WaitForSeconds(movePause);

            if (moveDir != eMoveDir.down && !Input.GetButton("Right"))
                moveDir = eMoveDir.down;
        }
    }

    public virtual IEnumerator AutomaticDropping()
    {
        while (board.State == eMinigameState.running)
        {
            if (fastDrop && dropTimer > fastdropTime)
            {
                dropTimer = fastdropTime;
            }

            dropTimer -= Time.deltaTime;

            if (dropTimer <= 0.0f)
            {
                if (IsInGoal(curPos))
                {
                    print("you win :)");
                    board.GameResult(true);
                    yield break;
                }

                int[] newPos = PosOnGrid(curPos[0] + board.Width, curRotation);

                if (!IsPosFree(newPos))
                {
                    print("you lose");
                    board.GameResult(false);
                    yield break;
                }

                rectTrans.anchoredPosition = new Vector3(
                    rectTrans.anchoredPosition.x,
                    rectTrans.anchoredPosition.y - board.TileSize,
                    0);

                curPos = newPos;
                dropTimer = fastDrop ? fastdropTime : defaultdropTime;                  
            }
            yield return null;

            while (board.State == eMinigameState.paused)
                yield return null;
        }
    }
}

