using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int maxHp = 5;
    private int hp;
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
}
