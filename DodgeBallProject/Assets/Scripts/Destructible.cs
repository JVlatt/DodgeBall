using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int hp = 2;
    public GameObject hitVFX;
    private Animator _anim;
    private bool playSound = false;
    private void Start()
    {
        _anim = GetComponent<Animator>();
        GameManager.Instance.destructibles.Add(this);
    }

    private void Update()
    {
        if (!playSound && hp == 0)
        {
            SoundManager.instance.WallFall();
            SoundManager.instance.wallFall.pitch = Random.Range(0.8f, 1.2f);
            playSound = true;
        }
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
        playSound = false;
        StartCoroutine(ResetCoroutine());
    }

    public IEnumerator ResetCoroutine()
    {
        _anim.SetBool("Reset", true);
        yield return new WaitForSeconds(1.0f);
        _anim.SetBool("Reset", false);
    }
}
