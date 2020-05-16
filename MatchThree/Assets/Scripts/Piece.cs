using System.Collections;
using UnityEngine;

/// <summary>
/// Controls movement and animation of the "fruit" tiles
/// Is instantiated at game start via prefab and referenced by the fruit class
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Piece : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRend;
    [SerializeField]
    private Animator anim;
    private float destroyAnimTime;
    private bool moving;

    private void Awake()
    {
        moving = false;
        spriteRend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        GetAnimClipTime();
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
    public IEnumerator SwapPosition(Vector3 newPos, bool noMatch = false)
    {
        moving = true;
        Vector3 oldPos = transform.position;

        while(moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, 0.05f);
            if (newPos.Equals(transform.position))
            {
                if (noMatch)
                {
                    StartCoroutine(SwapPosition(oldPos));
                    yield break;
                }
                else
                {
                    moving = false;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine for making the pieces fall into their new spot after a match has been destroyed
    /// </summary>
    private IEnumerator FallToNewPos()
    {
        Vector2 oldPos;
        Vector2 goalPos;
        //fall down until newPos reached
        yield return null;
    }

    /// <summary>
    /// Reactives the piece at the top to make to fall into the board again
    /// </summary>
    public void Reactivate()
    {
        anim.SetBool("destroy", false);
        //StartCoroutine(FallToNewPos(this,transform.position, ))
    }

    /// <summary>
    /// Calley by the board to "destroy" this piece when matched
    /// </summary>
    public IEnumerator Deactivate()
    {
        while (moving)
            yield return null;

        anim.SetBool("destroy", true);
        //play animation for destruction
        yield return new WaitForSeconds(destroyAnimTime);
    }
}