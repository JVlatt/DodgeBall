using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    private Light lightSpawner;
    public Ball ball;
    private void Awake()
    {
        lightSpawner = GetComponent<Light>();
    }

    private void Update()
    {
        if (ball == null) return;
        if (Vector3.Distance(ball.transform.position, transform.GetChild(0).position) < 0.1f && ball.direction == Vector3.zero)
        {
            lightSpawner.enabled = true;
            ball.gameObject.transform.Rotate(0,45*Time.deltaTime,0);
        }
        else
            lightSpawner.enabled = false;
    }
}
