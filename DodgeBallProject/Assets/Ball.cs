﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 2.0f;
    public float multiplierFactor = 1.0f;
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Collider _collider;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    private void LateUpdate()
    {
        _rb.velocity = direction.normalized * speed * multiplierFactor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Catch"))
        {
            _collider.enabled = false;
            other.GetComponentInParent<PlayerEntity>().Catch(this);
            _rb.rotation = Quaternion.identity;
            _rb.isKinematic = true;
        }
    }
}