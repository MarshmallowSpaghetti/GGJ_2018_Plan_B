using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveComponent : MonoBehaviour
{
    public float speed = 1;

    private Vector3 m_motion;
    private bool m_isOnGround;

    public Action onHitGround;
    public Action<Vector3> onBounceFromGround;

    public float gravityMultiplier = 0.2f;

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
                print("Hit on ground");
                if (onHitGround != null)
                    onHitGround();
            }
        }
    }

    private Rigidbody m_rig;

    private void FixedUpdate()
    {
        // If the character is in the air, do nothing
        //if (CharController.enabled == false)
        //    return;

        //MoveWhileFaceMouse();
        MoveInForward();

        // Apply gravity
        //CharController.Move(Rig.velocity + Physics.gravity * Time.deltaTime);
        //print("gravity " + Rig.velocity + Physics.gravity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
            FloatForAWhile();
    }

    private void MoveWhileFaceMouse()
    {
        m_motion = new Vector3(
            CrossPlatformInputManager.GetAxis("Horizontal"),
            0,
            CrossPlatformInputManager.GetAxis("Vertical"));

        //CharController.Move(m_motion * speed);

        float yVel = Rig.velocity.y;
        Rig.velocity = (m_motion * speed / Time.fixedDeltaTime).SetY(yVel);

        Rig.velocity += Physics.gravity * Time.fixedDeltaTime * gravityMultiplier;

        // Always keep in horizontal plane
        transform.forward =
            Vector3.Slerp(transform.forward,
            (MouseInput.Instance.MousePos - transform.position).SetY(0), 0.2f);
        // Without lerp
        //transform.forward = (MouseInput.Instance.MousePos - transform.position).SetY(0);
    }

    private void MoveInForward()
    {
        m_motion = new Vector3(
            CrossPlatformInputManager.GetAxis("Horizontal"),
            0,
            CrossPlatformInputManager.GetAxis("Vertical") + 0.3f);

        float angleInAFrame = 100 * Time.deltaTime;

        transform.Rotate(Vector3.up, angleInAFrame * m_motion.x);
        float yVel = Rig.velocity.y;
        Rig.velocity = (transform.rotation * new Vector3(0, 0, m_motion.z * speed)).SetY(yVel);
        Rig.velocity += Physics.gravity * Time.fixedDeltaTime * gravityMultiplier;
    }

    private void FloatForAWhile()
    {
        Rig.velocity = Vector3.up;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("collide with " + collision.gameObject.layer);
        // We give player control only when player land on ground
        if (collision.gameObject.layer == 8)
        {
            bool isOnGroundBefore = m_isOnGround;
            IsOnGround = true;
            if (isOnGroundBefore == false && onBounceFromGround != null)
            {
                onBounceFromGround(collision.relativeVelocity);
            }
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

    private bool CheckGroundInRange(float _distance)
    {
        return Physics.Raycast(transform.position, Vector3.down, _distance, 1 << LayerMask.NameToLayer("Ground"));
    }
}