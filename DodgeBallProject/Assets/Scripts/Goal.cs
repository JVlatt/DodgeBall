using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int maxHp = 5;
    private int hp;

    public GameObject hitVFX;

    void Start()
    {
        hp = maxHp;
        GameManager.Instance.goals.Add(this);
    }

    public void Reset()
    {
        hp = maxHp;
    }

    public void Hurt()
    {
        hp--;
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
