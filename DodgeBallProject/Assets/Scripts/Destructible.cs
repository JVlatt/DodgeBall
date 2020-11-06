using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int hp = 2;
    private Animator _anim;
    private void Start()
    {
        _anim = GetComponent<Animator>();
        GameManager.Instance.destructibles.Add(this);
    }

    public void Hurt()
    {
        hp--;
        _anim.SetInteger("State", hp);
    }

    public void Reset()
    {
        hp = 2;
        _anim.SetInteger("State", hp);
        StartCoroutine(ResetCoroutine());
    }

    public IEnumerator ResetCoroutine()
    {
        _anim.SetBool("Reset", true);
        yield return new WaitForSeconds(1.0f);
        _anim.SetBool("Reset", false);
    }
}
