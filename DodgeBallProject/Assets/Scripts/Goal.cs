using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public int maxHp = 5;
    private int hp;

    public GameObject hitVFX;
    public Slider healthBar;
    private Animation animBackground;
    private Animation animFill;

    void Start()
    {
        hp = maxHp;
        GameManager.Instance.goals.Add(this);
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

        if (hp < 0)
            hp = 0;

        if(hp > 0)
            GameManager.Instance.LaunchBall();
        switch (hp)
        {
            case 1:
                break;
            case 0:
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
