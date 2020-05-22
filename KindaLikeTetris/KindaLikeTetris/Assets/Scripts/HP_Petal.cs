using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_Petal : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Set the "falling" animation when this health point is lost
    /// </summary>
    public void Fall()
    {
        anim.SetBool("destroy", true);
    }

    /// <summary>
    /// Set the "return" animation when this health point is regained
    /// </summary>
    public void Regain()
    {
        anim.SetBool("destroy", false);
    }
}
