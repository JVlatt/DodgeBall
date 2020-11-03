using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 2.0f;
    public float maxSpeed = 2.0f;
    public float speedBoost = 0f;
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Collider _collider;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    private void LateUpdate()
    {
        if (GameManager.Instance.state == GameManager.GAME_STATE.FREEZE) return;
        _rb.velocity = direction.normalized * (speed + speedBoost);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
        }
        if (collision.gameObject.CompareTag("Goal"))
        {
            collision.gameObject.GetComponent<Goal>().Hurt();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Catch"))
        {
            PlayerEntity entity = other.GetComponentInParent<PlayerEntity>();
            if (!entity || entity.playerBall != null) return;
            _collider.enabled = false;
            other.GetComponentInParent<PlayerEntity>().Catch(this);
            _rb.rotation = Quaternion.identity;
            _rb.isKinematic = true;
            if(speed + speedBoost <= maxSpeed)
                speedBoost += 7.5f;
        }
    }
}
