using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int hp = 3;
    public Material mat;

    public void Hurt()
    {
        hp--;
        switch (hp)
        {
            case 2:
                mat.color = Color.yellow;
                break;
            case 1:
                mat.color = Color.red;
                break;
            case 0:
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
    }
}
