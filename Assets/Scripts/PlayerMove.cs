﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 1;

    private Vector3 m_motion;
    private bool m_isOnGround;
    private bool m_isOnFlower;

    public Action onHitGround;
    public Action<Vector3> onLandOnGround;
    public Action onHitFlower;

    public float gravityMultiplier = 0.2f;

    [SerializeField]
    private bool m_isFloating = false;
    [SerializeField]
    private bool m_isFalling = false;
    [SerializeField]
    private bool m_hasLanded = false;

    private Coroutine m_checkLandingCoroutine;

    private Player m_thisPlayer;

    // Once launched, player should be either falling or floating
    private bool IsMoving
    {
        get
        {
            return m_isFloating || m_isFalling;
        }
    }

    private float m_airFraction = 0.15f;

    public Rigidbody Rig
    {
        get
        {
            if (m_rig == null)
                m_rig = GetComponent<Rigidbody>();
            return m_rig;
        }

        set
        {
            m_rig = value;
        }
    }

    public bool IsOnGround
    {
        get
        {
            return m_isOnGround;
        }

        set
        {
            if (m_isOnGround == true && value == false)
            {
                m_isOnGround = value;
            }
            else if (m_isOnGround == false && value == true)
            {
                // Prevenet player slide after landing
                //Rig.velocity = Vector3.zero;

                m_isOnGround = value;
                //print("Hit on ground");
                if (onHitGround != null)
                    onHitGround();
            }
        }
    }

    public bool IsOnFlower
    {
        get
        {
            return m_isOnFlower;
        }

        set
        {
            if (m_isOnFlower == true && value == false)
            {
                m_isOnFlower = value;
            }
            else if (m_isOnFlower == false && value == true)
            {
                m_isOnFlower = value;
                //print("Land on flower");
                if (onHitFlower != null)
                    onHitFlower();
            }
            m_isOnFlower = value;
        }
    }

    public bool IsFloating
    {
        get
        {
            return m_isFloating;
        }

        set
        {
            m_isFloating = value;
        }
    }

    public bool IsFalling
    {
        get
        {
            return m_isFalling;
        }

        set
        {
            m_isFalling = value;
        }
    }

    public Player ThisPlayer
    {
        get
        {
            if (m_thisPlayer == null)
                m_thisPlayer = GetComponent<Player>();
            return m_thisPlayer;
        }

        set
        {
            m_thisPlayer = value;
        }
    }

    private Rigidbody m_rig;

    private void Awake()
    {
        Action landOnSth = () =>
        {
            DisableLaunch();
        };
        //onHitGround += landOnSth;
        onHitFlower += landOnSth;
    }

    private void FixedUpdate()
    {
        MoveFoward();

        ApplyGravity();
    }

    private void MoveFoward()
    {
        if (IsMoving == false)
            return;

        if (ThisPlayer.IsDisabled)
            return;

        m_motion = new Vector3(
            CrossPlatformInputManager.GetAxis("Horizontal"),
            0,
            CrossPlatformInputManager.GetAxis("Vertical") * gravityMultiplier + 0.07f);

        float angleInAFrame = 100 * Time.deltaTime;

        transform.Rotate(Vector3.up, angleInAFrame * m_motion.x);
        float yVel = Rig.velocity.y;
        Rig.velocity = (transform.rotation * new Vector3(0, 0, m_motion.z * speed)).SetY(yVel);
        //Rig.velocity += Physics.gravity * Time.fixedDeltaTime * gravityMultiplier;
        //Rig.MovePosition(Rig.position + Vector3.down * gravityMultiplier);
    }

    private void ApplyGravity()
    {
        if (m_isFalling)
        {
            Rig.velocity += (Physics.gravity * gravityMultiplier
                - Physics.gravity.normalized * Vector3.Dot(Rig.velocity, Physics.gravity) * m_airFraction) * Time.fixedDeltaTime;

            //print("Falling velocity " + Rig.velocity);
        }
    }

    public void DisableLaunch()
    {
        m_isFalling = false;
        m_isFloating = false;
        Rig.isKinematic = true;
        m_hasLanded = true;
    }

    public void SetLaunchable()
    {
        print("Enable relaunch");
        m_isFalling = false;
        m_isFloating = false;
        Rig.isKinematic = false;
        m_hasLanded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("collide with " + collision.gameObject.layer);
        // We give player control only when player land on ground
        if (collision.gameObject.layer == 8)
        {
            //bool isOnGroundBefore = m_isOnGround;
            IsOnGround = true;
            //print("Land on ground");
            //if (isOnGroundBefore == false && onLandOnGround != null)
            //{
            //    onLandOnGround(collision.relativeVelocity);
            //}
        }
        else if (collision.gameObject.layer == 9)
        {
            if (collision.transform.parent.GetComponent<Flower>().HasLanded == false)
                IsOnFlower = true;
        }
        else
        {
            Rig.AddForce(collision.impulse * 0.2f, ForceMode.Impulse);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            //print("Leave ground");
            //IsOnGround = false;
        }
    }

    public void StartWaitUntlLandOnSth(Action _callback)
    {
        m_checkLandingCoroutine = StartCoroutine(WaitUntilLandOnSth(_callback));
    }

    public void StopWaitUnitLandOnSth()
    {
        StopCoroutine(m_checkLandingCoroutine);
        m_checkLandingCoroutine = null;
    }

    private IEnumerator WaitUntilLandOnSth(Action _callback)
    {
        while (m_hasLanded == false)
        {
            yield return null;
        }

        print("Finally began camera moving");
        if (_callback != null)
            _callback();
    }
}