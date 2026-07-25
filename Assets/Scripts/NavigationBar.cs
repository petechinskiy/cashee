using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBar : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void DisableAnimator()
    {
        anim.enabled = false;
    }
}
