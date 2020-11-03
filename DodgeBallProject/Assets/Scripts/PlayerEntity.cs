using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntity : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed = 1.0f;
    public float turnSpeed = 1.0f;
    [Header("Abilities")]
    [SerializeField] private float _dashForce = 1.0f;
    [SerializeField] private float _dashDuration = 1.0f;
    [SerializeField] private float _dashCooldown = 5.0f;
    [SerializeField] private float _holdTime = 1.0f;
    [SerializeField] private float _maxHoldTime = 5.0f;
    [SerializeField] private float _catchCooldown = 5.0f;
    [SerializeField] private float _catchDurationMultiplier = 1.0f;
    private float _chargeClock = 0f;
    private float _catchClock = 0f;
    private float _dropClock = 0f;
    private float _dashClock = 0f;
    private bool chargedShoot = false;

    private Animator _anim;
    [HideInInspector]public Ball playerBall = null;

    [Header("Game Objects")]
    private GameObject modelObj;
    public Transform ballPivot;
    public Transform launchPoint;
    private Vector3 spawnPoint;

    [Header("Respawn")]
    public float waitForSpawn = 3.0f;
    private float waitForSpawnClock = 0f;
    public float respawnCooldown = 5.0f;
    private float respawnCooldownClock = 0f;
    public GameObject respawnDisplay;
    private GameObject respawnDisplayInstance;
    private Text respawnText;
    private Image respawnImage;
    private bool respawnDisplayCreated;

    //Dev vars
    private Vector3 _moveDir;
    private Vector3 _orientDir;
    private Vector3 _velocity = Vector3.zero;

    [HideInInspector]
    public Rigidbody rb;
    private Animator animator;
    [HideInInspector]
    public bool stopMove;
    [HideInInspector]
    public bool rightAxisTouch;
    [HideInInspector]
    public bool isOnGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        modelObj = this.gameObject;
        _anim = GetComponent<Animator>();
        _anim.SetFloat("CatchDuration", _catchDurationMultiplier);
    }

    void Start()
    {
        waitForSpawnClock = waitForSpawn;
        respawnCooldownClock = respawnCooldown;
        spawnPoint = this.transform.position;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.state == GameManager.GAME_STATE.FREEZE) return;
        _UpdateGravity();

        if (!stopMove)
        {
            _UpdateMove();
            _UpdateModelOrient();
        }
        if(playerBall)
        {
            playerBall.transform.position = ballPivot.transform.position;
            if(rightAxisTouch)
            {
                _chargeClock += Time.deltaTime;
                if (_chargeClock >= _holdTime)
                    chargedShoot = true;
                if (_chargeClock >= _maxHoldTime)
                    LaunchBall();
            }
            else
            {
                if (chargedShoot)
                    LaunchBall();
                else
                {
                    _dropClock += Time.deltaTime;
                    if (_dropClock > _maxHoldTime)
                        LaunchBall();
                }
            }
        }
        if (_catchClock > 0f)
            _catchClock -= Time.deltaTime;

        if(_dashClock > 0f)
        {
            _dashClock -= Time.deltaTime;
        }

        rb.velocity = new Vector3(_velocity.x,rb.velocity.y,_velocity.z);
    }

    #region Functions Move

    public void Orient(Vector3 ori)
    {
        _orientDir = ori;
    }

    public void _UpdateModelOrient()
    {
        float startAngle = modelObj.transform.eulerAngles.y;
        float endAngle = startAngle + Vector3.SignedAngle(modelObj.transform.forward, _orientDir, Vector3.up);
        float angle = Mathf.Lerp(startAngle, endAngle, turnSpeed);

        Vector3 eulerAngles = modelObj.transform.eulerAngles;
        eulerAngles.y = angle;
        modelObj.transform.eulerAngles = eulerAngles;
    }

    public void Move(Vector3 dir)
    {
        _moveDir = dir;
    }
    private void _UpdateMove()
    {
            _velocity = _moveDir * moveSpeed;
            
            if (!rightAxisTouch)
            {
                _orientDir = _velocity.normalized;
            }
    }
    #endregion

    private void _UpdateGravity()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            isOnGround = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            //Debug.Log("Did not Hit");
            isOnGround = false;
            waitForSpawn -= Time.deltaTime;
            if(waitForSpawn <= 0)
            {
                respawnCooldown -= Time.deltaTime;
            }
            Respawn();
        }
    }

    private void Respawn()
    {
        if(waitForSpawn <= 0)
        {
            rb.isKinematic = true;
            if (!respawnDisplayCreated)
            {
                respawnDisplayInstance = Instantiate(respawnDisplay);
                respawnDisplayInstance.transform.position = spawnPoint;
                respawnText = respawnDisplayInstance.GetComponentInChildren<Text>();
                respawnImage = respawnDisplayInstance.GetComponentInChildren<Image>();
                respawnDisplayCreated = true;
            }

            respawnText.text = respawnCooldown.ToString("F1");
            respawnImage.fillAmount = respawnCooldown / respawnCooldownClock;

            if (respawnCooldown <= 0)
            {
                rb.isKinematic = false;
                this.transform.position = spawnPoint;
                waitForSpawn = waitForSpawnClock;
                respawnCooldown = respawnCooldownClock;
                Destroy(respawnDisplayInstance);
                respawnDisplayCreated = false;
            }
        }
    }

    public void LaunchBall()
    {
        Vector3 ballDirection = (launchPoint.position - transform.position).normalized;
        ballDirection.y = 0;
        playerBall.transform.position = launchPoint.position;
        playerBall.direction = ballDirection;
        playerBall._collider.enabled = true;
        playerBall._rb.isKinematic = false;
        playerBall = null;
        chargedShoot = false;
        _chargeClock = 0f;
        _dropClock = 0f;
        _catchClock = _catchCooldown;
    }

    public void TryCatch()
    {
        if (_catchClock > 0f || playerBall) return;

        _catchClock = _catchCooldown;
        _anim.SetTrigger("Catch");
    }
    public void Catch(Ball ball)
    {
        playerBall = ball;
        playerBall.direction = Vector3.zero;
    }

    public IEnumerator Dash()
    {
        if (_dashClock > 0) yield break;

        _dashClock = _dashCooldown;
        rb.AddForce(_moveDir * _dashForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(_dashDuration);
        rb.velocity = Vector3.zero;
    }
}
