using UnityEngine;

/// <summary>
/// Manages the players input via mouse on the board
/// </summary>
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float minSwipeDistance = 0.4f;
    private Vector3 clickStartPos;
    private Vector3 clickEndPos;
    private Piece clickedPiece;

    public static event System.Action<Piece, eSwipeDir> OnPieceMove = delegate { };

    private void Update()
    {
        if (GameManager.Instance.GameState == eGameState.running)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickStartPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                clickedPiece = ClickedPiece(clickStartPos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (clickedPiece == null)
                    return;

                clickEndPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
                float distX = Mathf.Abs(clickEndPos.x - clickStartPos.x);
                float distY = Mathf.Abs(clickEndPos.y - clickStartPos.y);
                //Only count as move if swipe movement was "strong" enough
                if (distX > minSwipeDistance || distY > minSwipeDistance)
                    OnPieceMove(clickedPiece, SwipeDirection(SwipeAngle(clickStartPos, clickEndPos)));
                //clear clickedPiece just to be sure
                clickedPiece = null;
            }
        }
    }
    /// <summary>
    /// Returns Piece if one was hit with raycast at the given position 
    /// </summary>
    private Piece ClickedPiece(Vector3 hitPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(hitPos, Vector2.zero);
        return hit.collider != null ? hit.collider.GetComponent<Piece>() : null;
    }
    /// <summary>
    /// Returns the angle between 2 Vectors
    /// </summary>
    private float SwipeAngle(Vector3 start, Vector3 end)
    {
        Vector3 swipe = start - end;
        float swipeAngle = Mathf.Atan2(swipe.x, swipe.y)  * (180 / Mathf.PI);
        return swipeAngle;
    }
    /// <summary>
    /// Returns the Direction enum calculated with the swipe angle
    /// </summary>
    private eSwipeDir SwipeDirection(float angle)
    {
        if (angle > 140 || angle <-140)
            return eSwipeDir.up;
        else if (angle > 50 && angle < 130)
            return eSwipeDir.left;
        else if (angle > -40 && angle < 40)
            return eSwipeDir.down;
        else if (angle > -130 && angle < -50)
            return eSwipeDir.right;
        else
            return eSwipeDir.none;
    }
}
