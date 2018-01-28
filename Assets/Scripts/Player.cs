using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMove))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private int m_energyCountDown = 500;
    [SerializeField]
    private int m_energyCount = 500;
    private Coroutine m_energyCosting;

    private PlayerMove m_playerMove;

    private Rigidbody m_rig;
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

    public PlayerMove ThisPlayerMove
    {
        get
        {
            if (m_playerMove == null)
                m_playerMove = GetComponent<PlayerMove>();
            return m_playerMove;
        }

        set
        {
            m_playerMove = value;
        }
    }

    private void Awake()
    {
        ThisPlayerMove.onHitFlower += () =>
        {
            print("Hit on flower");
            if(m_energyCosting != null)
            {
                StopCoroutine(m_energyCosting);
                m_energyCosting = null;
            }
            m_energyCount = m_energyCountDown;
        };
        m_energyCount = m_energyCountDown;
    }

    private void Update()
    {
        LaucnCheck();
    }

    private void LaucnCheck()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            ThisPlayerMove.IsFalling = false;
            ThisPlayerMove.IsFloating = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ThisPlayerMove.IsFalling = true;
            ThisPlayerMove.IsFloating = false;
        }
    }

    private void Launch()
    {
        Rig.velocity = Vector3.up * 0.7f;
        ThisPlayerMove.IsFalling = true;
        ThisPlayerMove.IsFloating = false;
        ThisPlayerMove.IsOnGround = false;
        ThisPlayerMove.IsOnFlower = false;

        m_energyCosting = StartCoroutine(KeepUsingEnergy());
    }

    private IEnumerator KeepUsingEnergy()
    {
        while(m_energyCount > 0)
        {

            m_energyCount--;
            yield return null;
        }

        print("No more energy");
    }
}
