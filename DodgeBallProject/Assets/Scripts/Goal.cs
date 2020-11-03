using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int hp = 5;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
