using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 direction;

    public List<int> speedIncrease;
    public List<int> damageIncrease;
    public int stateIndex = 0;

    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Collider _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    private void Start()
    {
        GameManager.Instance.balls.Add(this);
        stateIndex = 0;
    }
    public void Update()
    {
        if (!GameManager.Instance.balls.Contains(this)) Destroy(this.gameObject);

        //Sécurités pour pas etre Out of Bounds
        if(stateIndex < 0)
        {
            stateIndex = 0;
        }

        if(stateIndex > 9)
        {
            stateIndex = 9;
        }

    }
    private void LateUpdate()
    {
        if (GameManager.Instance.state != GameManager.GAME_STATE.PLAY) return;
        _rb.velocity = direction.normalized * (speedIncrease[stateIndex]);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.Instance.state != GameManager.GAME_STATE.PLAY) return;
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
        }
        if (collision.gameObject.CompareTag("Goal"))
        {
            collision.gameObject.GetComponent<Goal>().Hurt(damageIncrease[stateIndex]);
            GameManager.Instance.balls.Remove(this);
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && !_rb.isKinematic)
        {
            stateIndex--;
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
            stateIndex++;
        }
        if(other.CompareTag("KillBall"))
        {
            GameManager.Instance.LaunchBall();
            Destroy(this.gameObject);
        }
    }
}
