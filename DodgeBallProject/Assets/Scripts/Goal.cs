using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class Goal : MonoBehaviour
{
    public int maxHp = 5;
    private int hp;

    public GameObject hitVFX;
    public GameObject changeStateVFX;
    public GameObject destroyVFX;
    public GameObject explosionCristalVFX;

    private Slider healthBar;
    private Animation animBackground;
    private Animation animFill;

    private Animator _anim;
    public enum GoalState
    {
        Full = 0,
        ThreeQuarter,
        Half,
        OneQuarter
    }

    public GoalState curState;

    void Start()
    {
        hp = maxHp;
        curState = GoalState.Full;
        GameManager.Instance.goals.Add(this);
        healthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        animBackground = healthBar.transform.GetChild(0).GetComponent<Animation>();
        animFill = healthBar.transform.GetChild(1).GetChild(0).GetComponent<Animation>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        healthBar.value = hp;
    }

    public void Reset()
    {
        hp = maxHp;
        curState = GoalState.Full;
        destroyVFX.SetActive(false);
        for (int i = 0; i < destroyVFX.transform.childCount; i++)
        {
            destroyVFX.transform.GetChild(i).GetComponent<ParticleSystem>().Clear();
        }
        _anim.SetInteger("State", 0);
        StartCoroutine(ResetCoroutine());
    }

    public IEnumerator ResetCoroutine()
    {
        _anim.SetBool("Reset",true);
        yield return new WaitForSeconds(0.5f);
        _anim.SetBool("Reset", false);
    }

    public void Hurt(int damageIncrease)
    {
        hp -= damageIncrease;

        if (animBackground.isPlaying)
        {
            animBackground.Rewind();
            animFill.Rewind();
        }

        animBackground.Play();
        animFill.Play();

        if (hp <= maxHp && curState == GoalState.Full)
        {
            Instantiate(changeStateVFX, this.transform);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            SoundManager.instance.LastCrystalState();
            curState = GoalState.ThreeQuarter;
            _anim.SetInteger("State", 1);
        }

        if (hp <= (3 * maxHp) / 4 && curState == GoalState.ThreeQuarter)
        {
            Instantiate(changeStateVFX, this.transform);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            SoundManager.instance.LastCrystalState();
            curState = GoalState.Half;
            _anim.SetInteger("State", 2);
        }

        if (hp <= (2 * maxHp) / 4 && curState == GoalState.Half)
        {
            Instantiate(changeStateVFX, this.transform);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            SoundManager.instance.LastCrystalState();
            curState = GoalState.OneQuarter;
            _anim.SetInteger("State", 3);
        }

        if (hp <= maxHp / 4 && curState == GoalState.OneQuarter)
        {
            Instantiate(changeStateVFX, this.transform);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            SoundManager.instance.LastCrystalState();
            _anim.SetInteger("State", 4);
        }

        if (hp < 0)
            hp = 0;

        switch (hp)
        {
            case 1:
                break;
            case 0:
                _anim.SetInteger("State", 5);
                CameraShaker.Instance.ShakeOnce(10f, 10f, 0.1f, 1f);
                var tmp = Instantiate(explosionCristalVFX);
                tmp.transform.position = this.transform.position;
                destroyVFX.SetActive(true);
                StartCoroutine(SlowMo());
                SoundManager.instance.CrystalBreak();
                GameManager.Instance.AddPoint(gameObject.name);
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("hit");
            var pos = collision.transform;
            var vfx = Instantiate(hitVFX);
            vfx.transform.position = pos.position;
        }
    }

    public IEnumerator SlowMo()
    {
        float nTime = 1f;
        while (nTime >= 0f)
        {
            nTime -= Time.unscaledDeltaTime;
            Time.timeScale = 0.25f;
            yield return null;
        }

        Time.timeScale = 1f;
    }
}
