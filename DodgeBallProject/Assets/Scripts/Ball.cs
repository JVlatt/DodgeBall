using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 direction;

    public List<int> speedIncrease;
    public List<int> damageIncrease;
    public List<int> bumpForce;
    public List<Gradient> trailColors;
    public int stateIndex = 0;

    public GameObject hitPlayerVFX;
    public List<GameObject> ballStateVFX;

    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Collider _collider;
    public Transform ballSpawner;
    private bool canBumpPlayer = true;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    private void Start()
    {
        ballSpawner.GetComponentInParent<BallSpawner>().ball = this;
        GameManager.Instance.balls.Add(this);
        stateIndex = 0;
        transform.position = ballSpawner.transform.position;
        direction = Vector3.zero;
        _collider.isTrigger = true;
        /*for(int i = 0; i < gameObject.transform.childCount - 2; i++)
        {
            ballStateVFX.Add(gameObject.transform.GetChild(i + 2).gameObject);
        }*/
        for(int i = 0; i < ballStateVFX.Count; i++)
        {
            ballStateVFX[i].gameObject.SetActive(false);
        }
    }
    public void Update()
    {
        direction.y = 0;
        UpdateStateVFX();
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
        if (collision.gameObject.CompareTag("Destructible"))
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
            collision.gameObject.GetComponent<Destructible>().Hurt();
        }
        if (collision.gameObject.CompareTag("Goal"))
        {
            SoundManager.instance.CrystalTouch();
            collision.gameObject.GetComponent<Goal>().Hurt(damageIncrease[stateIndex]);
            var tmp = Instantiate(collision.gameObject.GetComponent<Goal>().hitVFX, collision.gameObject.GetComponent<Goal>().transform);
            tmp.transform.position = this.transform.position;
            tmp.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            Reset();
        }

        if (collision.gameObject.CompareTag("Player")&& canBumpPlayer)
        {
            Debug.Log("Collide Player");

            var pos = collision.transform;
            var vfx = Instantiate(hitPlayerVFX);
            vfx.transform.position = pos.position;
            vfx.transform.rotation = pos.rotation;

            collision.gameObject.GetComponent<PlayerEntity>().Bump(collision.contacts[0].normal * -1, bumpForce[stateIndex], 0.5f);
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
            stateIndex--;
            stateIndex = Mathf.Clamp(stateIndex, 1, 4);
            StartCoroutine(CanBumpPlayerCoroutine());
        }
    }

    void UpdateStateVFX()
    {
        this.transform.GetChild(1).GetComponent<TrailRenderer>().colorGradient = trailColors[stateIndex];

        if (stateIndex <= 1)
        {
            ballStateVFX[0].SetActive(false);
            ballStateVFX[1].SetActive(false);
            ballStateVFX[2].SetActive(false);
        }

        if (stateIndex == 2)
        {
            ballStateVFX[0].SetActive(true);
            ballStateVFX[1].SetActive(false);
            ballStateVFX[2].SetActive(false);
        }

        if (stateIndex == 3)
        {
            ballStateVFX[0].SetActive(true);
            ballStateVFX[1].SetActive(true);
            ballStateVFX[2].SetActive(false);
        }

        if (stateIndex == 4)
        {
            ballStateVFX[0].SetActive(true);
            ballStateVFX[1].SetActive(true);
            ballStateVFX[2].SetActive(true);
        }
    }

    IEnumerator CanBumpPlayerCoroutine()
    {
        canBumpPlayer = false;
        yield return new WaitForSeconds(0.5f);
        canBumpPlayer = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Catch"))
        {
            PlayerEntity entity = other.GetComponentInParent<PlayerEntity>();
            if (!entity || entity.playerBall != null) return;
            _collider.isTrigger = false;
            _collider.enabled = false;
            other.GetComponentInParent<PlayerEntity>().Catch(this);
            _rb.rotation = Quaternion.identity;
            _rb.isKinematic = true;
            stateIndex++;
            stateIndex = Mathf.Clamp(stateIndex, 1, 4);
        }
        if (other.CompareTag("KillBall"))
        {
            Reset();
        }
    }

    public void Reset()
    {
        _collider.isTrigger = true;
        transform.position = ballSpawner.position;
        _rb.velocity = Vector3.zero;
        direction = Vector3.zero;
        stateIndex = 0;

        for (int i = 0; i < ballStateVFX.Count; i++)
        {
            ballStateVFX[i].gameObject.SetActive(false);
        }
    }
}
