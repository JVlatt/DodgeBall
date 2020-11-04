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
        switch (hp)
        {
            case 1:
                GameManager.Instance.recorder.StartRecord();
                break;
            case 0:
                GameManager.Instance.recorder.StopRecord();
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
