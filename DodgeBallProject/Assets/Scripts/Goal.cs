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

    private Slider healthBar;
    private Animation animBackground;
    private Animation animFill;

    public enum GoalState
    {
        Full,
        ThreeQuarter,
        Half,
        OneQuarter,
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
    }

    private void Update()
    {
        healthBar.value = hp;
    }

    public void Reset()
    {
        hp = maxHp;
        curState = GoalState.Full;
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

        if(hp <= 75 && curState == GoalState.Full)
        {
            Instantiate(changeStateVFX, this.transform);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            curState = GoalState.ThreeQuarter;
        }

        if(hp <= 50 && curState == GoalState.ThreeQuarter)
        {
            Instantiate(changeStateVFX, this.transform);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            curState = GoalState.Half;
        }

        if (hp <= 25 && curState == GoalState.Half)
        {
            Instantiate(changeStateVFX, this.transform);
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            SoundManager.instance.LastCrystalState();
            curState = GoalState.OneQuarter;
        }

        if (hp < 0)
            hp = 0;

        if(hp > 0)
            GameManager.Instance.LaunchBall();
        switch (hp)
        {
            case 1:
                break;
            case 0:
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
}
