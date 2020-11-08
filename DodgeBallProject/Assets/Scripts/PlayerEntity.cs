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
    [HideInInspector] public Ball playerBall = null;

    [Header("Game Objects")]
    private GameObject modelObj;
    public Transform ballPivot;
    public Transform launchPoint;
    public GameObject indic;
    public ParticleSystem catchVFX;
    public GameObject catchCollider;
    private Vector3 spawnPoint;
    private Quaternion oriRot;

    [Header("Respawn")]
    public float waitForSpawn = 3.0f;
    private float waitForSpawnClock = 0f;
    public float respawnCooldown = 5.0f;
    private float respawnCooldownClock = 0f;
    public GameObject respawnDisplay;
    public GameObject respawnVFX;
    public GameObject dashVFX;
    private GameObject respawnDisplayInstance;
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
    [HideInInspector]
    public bool stopModelOrient;
    private bool fallSound = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        modelObj = this.gameObject;
        _anim = GetComponentInChildren<Animator>();
        _anim.SetFloat("CatchDuration", _catchDurationMultiplier);
    }

    void Start()
    {
        isOnGround = true;
        catchCollider.SetActive(false);
        waitForSpawnClock = waitForSpawn;
        respawnCooldownClock = respawnCooldown;
        spawnPoint = this.transform.position;
        oriRot = modelObj.transform.rotation;
        if (GameManager.Instance != null)
            GameManager.Instance.players.Add(this);
    }

    private void Update()
    {
        if (GameManager.Instance.state != GameManager.GAME_STATE.PLAY)
        {
            _anim.Rebind();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.state != GameManager.GAME_STATE.PLAY) return;
        _UpdateGravity();

        if (!stopMove)
        {
            _UpdateMove();
            if (!stopModelOrient)
            {
                _UpdateModelOrient();
            }
        }

        if (playerBall)
        {
            playerBall.transform.position = ballPivot.transform.position;

            _dropClock += Time.deltaTime;
            if (_dropClock > _maxHoldTime)
                LaunchBall();
        }
        if (_catchClock > 0f)
            _catchClock -= Time.deltaTime;

        if (_dashClock > 0f)
        {
            _dashClock -= Time.deltaTime;
        }

        rb.velocity = new Vector3(_velocity.x, rb.velocity.y, _velocity.z);
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

        if (_velocity != Vector3.zero)
        {
            _anim.SetBool("Run", true);
        }
        else
        {
            _anim.SetBool("Run", false);
        }


        if (!rightAxisTouch)
        {
            _orientDir = _velocity.normalized;
        }

        if (rightAxisTouch && playerBall)
        {
            indic.SetActive(true);
            _anim.SetBool("Aim", true);
        }
        else
        {
            indic.SetActive(false);
            _anim.SetBool("Aim", false);
        }
    }
    #endregion

    private void _UpdateGravity()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2, 1 << LayerMask.NameToLayer("Ground")))
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
            if (waitForSpawn <= 0)
            {
                if (playerBall != null)
                {
                    _anim.SetBool("Hold", false);
                    playerBall.Reset();
                    playerBall = null;
                }
                respawnCooldown -= Time.deltaTime;
            }
            Respawn();
            LaunchBall();
        }
    }

    private void Respawn()
    {

        if (waitForSpawn <= 0)
        {
            rb.isKinematic = true;
            if (!respawnDisplayCreated)
            {
                respawnDisplayInstance = Instantiate(respawnDisplay);
                respawnDisplayInstance.transform.position = spawnPoint;
                respawnImage = respawnDisplayInstance.GetComponentInChildren<Image>();
                respawnDisplayCreated = true;
            }

            respawnImage.fillAmount = respawnCooldown / respawnCooldownClock;

            if (respawnCooldown <= 0)
            {
                rb.isKinematic = false;
                this.transform.position = spawnPoint;
                waitForSpawn = waitForSpawnClock;
                respawnCooldown = respawnCooldownClock;
                var vfx = Instantiate(respawnVFX);
                vfx.transform.position = respawnDisplayInstance.transform.position;
                Destroy(respawnDisplayInstance);
                respawnDisplayCreated = false;
                fallSound = false;
            }
        }
    }

    public void LaunchBall()
    {
        SoundManager.instance.ThrowBall();
        _anim.SetBool("Shoot",true);
        _anim.SetBool("Hold", false);
        _anim.SetBool("Aim", false);
        Vector3 ballDirection = (launchPoint.position - transform.position).normalized;
        ballDirection.y = 0;
        if(playerBall != null)
        {
            playerBall.stateIndex++;
            playerBall.stateIndex = Mathf.Clamp(playerBall.stateIndex, 1, 4);
            playerBall.transform.position = launchPoint.position;
            playerBall.direction = ballDirection;
            playerBall._collider.enabled = true;
            playerBall._rb.isKinematic = false;
            playerBall.transform.GetChild(1).gameObject.SetActive(true);
            playerBall = null;
        }
        chargedShoot = false;
        _chargeClock = 0f;
        _dropClock = 0f;
        _catchClock = _catchCooldown;
        StartCoroutine(ResetShootAnim());
    }
    public IEnumerator ResetShootAnim()
    {
        yield return new WaitForSeconds(0.2f);
        _anim.SetBool("Shoot", false);
    }
    public void TryCatch()
    {
        if (_catchClock > 0f || playerBall) return;

        catchCollider.SetActive(true);
        _catchClock = _catchCooldown;
        StartCoroutine(ResetCatchAnim());
    }

    public IEnumerator ResetCatchAnim()
    {
        _anim.SetBool("Catch", true);
        yield return new WaitForSeconds(1.0f);
        catchCollider.SetActive(false);
        _anim.SetBool("Catch", false);
    }

    public void Catch(Ball ball)
    {
        SoundManager.instance.CatchBall();
        catchCollider.SetActive(false);
        catchVFX.Play();
        playerBall = ball;
        _anim.SetBool("Hold", true);
        Bump(playerBall.direction, playerBall.bumpForce[playerBall.stateIndex], 0.1f);
        playerBall.transform.GetChild(1).gameObject.SetActive(false);
        playerBall.direction = Vector3.zero;
    }

    public IEnumerator Dash()
    {
        if (_dashClock > 0) yield break;

        if(_velocity != Vector3.zero)
        {
            SoundManager.instance.Dash();
            var pos = this.transform.position;
            var vfx = Instantiate(dashVFX);
            vfx.transform.position = pos;
            stopMove = true;
            _dashClock = _dashCooldown;
            _velocity = _moveDir * _dashForce;
            yield return new WaitForSeconds(_dashDuration);
            _velocity = Vector3.zero;
            stopMove = false;
        }
    }

    public void Reset()
    {
        _velocity = Vector3.zero;
        transform.position = spawnPoint;
        modelObj.transform.rotation = oriRot;
        if (playerBall != null)
            playerBall = null;
    }

    public void Bump(Vector3 dir, float bumpForce, float duration)
    {
        StartCoroutine(BumpCoroutine(dir, bumpForce, duration));
    }

    IEnumerator BumpCoroutine(Vector3 dir, float bumpForce, float duration)
    {
        stopMove = true;
        _velocity = dir * bumpForce;
        yield return new WaitForSeconds(duration);
        _velocity = Vector3.zero;
        stopMove = false;
    }
}
