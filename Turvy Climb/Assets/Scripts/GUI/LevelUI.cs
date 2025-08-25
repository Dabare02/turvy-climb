using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    private Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void OnPointerEnter()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            if (!player.IsABodyPartMoving()) _anim.SetTrigger("Hover");
        }
    }

    public void OnPointerExit()
    {
        _anim.SetTrigger("NoHover");
    }
}
