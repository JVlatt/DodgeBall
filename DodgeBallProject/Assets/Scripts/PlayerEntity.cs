using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{

    //Vitesse
    private float acceleration;
    private float moveSpeedMax;

    //Inertie
    private float friction;
    private float turnFriction;

    //Rotation
    private float turnSpeed;

    [Header("Vitesse")]
    [Header("Stats Player")]
    public float accelerationPlayer = 20f;
    public float moveSpeedMaxPlayer = 10f;

    [Header("Inertie")]
    public float frictionPlayer = 0f;
    public float turnFrictionPlayer = 20f;

    [Header("Rotation")]
    public float turnSpeedPlayer = 15f;

    [Header("Game Objects")]
    private GameObject modelObj;

    private bool stopMove;

    private Vector3 _moveDir;
    private Vector3 _orientDir = Vector3.back;
    private Vector3 _velocity = Vector3.zero;

    [HideInInspector]
    public Rigidbody rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        modelObj = this.gameObject;
        acceleration = accelerationPlayer;
        moveSpeedMax = moveSpeedMaxPlayer;
        friction = frictionPlayer;
        turnFriction = turnFrictionPlayer;
        turnSpeed = turnSpeedPlayer;
    }

    void Start()
    {

    }

    private void FixedUpdate()
    {
        if (!stopMove)
        {
            _UpdateMove();
        }

        Vector3 newPosition = transform.position;
        newPosition.x += _velocity.x * Time.fixedDeltaTime;
        newPosition.z += _velocity.z * Time.fixedDeltaTime;
        rb.velocity = _velocity;
    }

    #region Functions Move

    private void _UpdateModelOrient()
    {
        float startAngle = modelObj.transform.eulerAngles.y;
        float endAngle = startAngle + Vector3.SignedAngle(modelObj.transform.forward, _orientDir, Vector3.up);
        float angle = Mathf.Lerp(startAngle, endAngle, turnSpeed * Time.deltaTime);

        Vector3 eulerAngles = modelObj.transform.eulerAngles;
        eulerAngles.y = angle;
        modelObj.transform.eulerAngles = eulerAngles;
    }

    public void Move(Vector3 dir)
    {
        _moveDir = dir;
    }

    public void StopMove()
    {
        stopMove = true;
        _velocity = Vector3.zero;
    }

    public void RestartMove()
    {
        stopMove = false;
    }

    private void _UpdateMove()
    {
        if (_moveDir != Vector3.zero)
        {

            float turnAngle = Vector3.SignedAngle(_velocity, Vector3.zero, _moveDir);
            turnAngle = Mathf.Abs(turnAngle);
            float frictionRatio = turnAngle / 360f;
            float turnFrictionWithRatio = turnFriction * frictionRatio;

            _velocity += _moveDir * acceleration * Time.fixedDeltaTime;
            if (_velocity.sqrMagnitude > moveSpeedMax * moveSpeedMax)
            {
                _velocity = _velocity.normalized * moveSpeedMax;
            }

            Vector3 frictionDir = _velocity.normalized;
            _velocity -= frictionDir * turnFrictionWithRatio * Time.fixedDeltaTime;

            _orientDir = _velocity.normalized;
        }
        else if (_velocity != Vector3.zero)
        {
            Vector3 frictionDir = _velocity.normalized;
            float frictionToApply = friction * Time.fixedDeltaTime;
            if (_velocity.sqrMagnitude <= frictionToApply * frictionToApply)
            {
                _velocity = Vector3.zero;
            }
            else
            {
                _velocity -= frictionDir * frictionToApply;               
            }
        }
    }
    #endregion

}
