using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed = 1.0f;
    public float turnSpeed = 1.0f;
    [SerializeField] private float _dashForce = 1.0f;
    [SerializeField] private float _dashDuration = 1.0f;
    [SerializeField] private float _dashCooldown = 5.0f;
    [SerializeField] private float _holdTime = 1.0f;
    [SerializeField] private float _maxHoldTime = 5.0f;
    [SerializeField] private float _catchCooldown = 5.0f;
    private float _chargeClock = 0f;
    private float _catchClock = 0f;
    private float _dropClock = 0f;
    private float _dashClock = 0f;
    private bool chargedShoot = false;

    private Animator _anim;
    private Ball playerBall = null;

    [Header("Game Objects")]
    private GameObject modelObj;
    public Transform ballPivot;
    public Transform launchPoint;
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        modelObj = this.gameObject;
        _anim = GetComponent<Animator>();
    }

    void Start()
    {

    }

    private void FixedUpdate()
    {
        if (!stopMove)
        {
            _UpdateMove();
            _UpdateModelOrient();
        }
        if(playerBall)
        {
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

    public void LaunchBall()
    {
        Vector3 ballDirection = (launchPoint.position - transform.position).normalized;
        ballDirection.y = 0;
        playerBall.transform.position = launchPoint.position;
        playerBall.direction = ballDirection;
        playerBall._collider.enabled = true;
        playerBall._rb.isKinematic = false;
        playerBall.transform.parent = null;
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
        playerBall.transform.parent = ballPivot;
        playerBall.transform.localPosition = Vector3.zero;
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
