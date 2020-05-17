using System.Collections;
using UnityEngine;

/// <summary>
/// Attached to the tile prefab; controls movement and animation of the "piece" tiles
/// Is instantiated at game start via prefab and referenced by the piece class
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Tile : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRend;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private BoxCollider2D col;
    [SerializeField]
    private float swapSpeed = 4.5f;
    [SerializeField]
    private float fallSpeed = 5.0f;
    private float destroyAnimTime;
    public bool Moving { get; private set; }

    private void Awake()
    {
        Moving = false;
        spriteRend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        GetAnimClipTime();
    }

    public void Matched(bool isMatched = true)
    {
        if (isMatched)
            spriteRend.color = Color.grey;
        else
            spriteRend.color = Color.white;
    }

    /// <summary>
    /// Checks the animation clips for one called "destroy" and gets its clip time
    /// </summary>
    private void GetAnimClipTime()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "destroy":
                    destroyAnimTime = clip.length;
                    return;
                default:
                    destroyAnimTime = 0.5f;
                    break;
            }
        }
    }

    /// <summary>
    /// Called by the fruit class when the sprite/type changes
    /// </summary>
    /// <param name="sprite"></param>
    public void SetSprite(Sprite sprite)
    {
        spriteRend.sprite = sprite;
    }

    /// <summary>
    /// Moves piece to the new location; if !noMatch it will move back to the old position afterwards
    /// </summary>
    public IEnumerator SwapPosition(Vector3 endPos, bool noMatch = false)
    {
        if (Moving)
            yield return null;

        Moving = true;
        bool stay = noMatch;
        Vector3 origPos = transform.position;

        while(Moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, swapSpeed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, endPos) < 0.001f)
            {
                if (stay)
                {
                    endPos = origPos;
                    stay = false;
                }
                else
                {
                    Moving = false;
                }
            }
            yield return null;
        }
    }

    private bool IsInPosition(Vector3 newPos)
    {
        return Vector3.Distance(transform.position, newPos) < 0.001f ? true : false;
    }

    /// <summary>
    /// Coroutine for making the pieces fall into their new spot after matches have been destroyed
    /// </summary>
    public IEnumerator FallToNewPos(Vector3 newPos)
    {
        Moving = true;

        if (IsInPosition(newPos))
        {
            Moving = false;
            yield break;
        }

        //Vector3 startPos = transform.position;
        //float lerpTime = 0.7f;
        //float curLerpTime = 0.0f;

        //wait until all pieces are ready to fall
        while (GameManager.Instance.GameState != eGameState.falling)
            yield return null;

        while (Moving)
        {
            //curLerpTime += Time.deltaTime;
            //float percentage = curLerpTime / lerpTime;
            //transform.position = Vector3.Lerp(transform.position, newPos, percentage);

            ////if (curLerpTime >= lerpTime)
            //if (Vector3.Distance(transform.position, newPos) < 0.001f)
            //{
            //    //print(gameObject.name + " has reached final pos: "+newPos.x+","+newPos.y);
            //    moving = false;
            //}
            transform.position = Vector3.MoveTowards(transform.position, newPos, fallSpeed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, newPos) < 0.001f)
            {
                Moving = false;
            }
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// Plays destroy animation, moves tile to its new position and changes the sprite
    /// </summary>
    public IEnumerator Deactivate(int fallPosY, Sprite newSprite)
    {
        anim.SetBool("destroy", true);
        col.enabled = false;
        //play animation for destruction
        yield return new WaitForSeconds(destroyAnimTime);
        MoveToFallPosition(fallPosY, newSprite);
    }

    private void MoveToFallPosition(int fallPosY, Sprite newSprite)
    {
        transform.position = new Vector3(transform.position.x, fallPosY, 0);
        Matched(false);
        SetSprite(newSprite);
        col.enabled = true;
        anim.SetBool("destroy", false);
    }
}