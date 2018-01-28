﻿using System.Collections;
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

    public SkinnedMeshRenderer skinRenderer;
    private Material m_mat;

    [SerializeField]
    private bool m_IsDisabled = false;

    public Flower startFlower;
    public Flower lastFlower;

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

    private Material Mat
    {
        get
        {
            if (m_mat == null)
                m_mat = skinRenderer.material;

            return m_mat;
        }
    }

    public bool IsDisabled
    {
        get
        {
            return m_IsDisabled;
        }

        set
        {
            m_IsDisabled = value;
        }
    }

    private void Awake()
    {
        ThisPlayerMove.onHitFlower += () =>
        {
            print("Hit on flower");
            if (m_energyCosting != null)
            {
                StopCoroutine(m_energyCosting);
                m_energyCosting = null;
            }
            m_energyCount = m_energyCountDown;
        };
        //m_energyCount = m_energyCountDown;
    }

    private void Start()
    {
        ThisPlayerMove.DisableLaunch();
        StartCoroutine(MoveToFlower(startFlower));
    }

    private void Update()
    {
        LaucnCheck();
    }

    private void LaucnCheck()
    {
        if (m_IsDisabled)
            return;

        if (lastFlower == null)
            return;

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
        Color origin = Mat.GetColor("_EmissionColor");
        while (m_energyCount > 0)
        {
            Color color = origin *
                (m_energyCount * 1f) / (m_energyCountDown * 1f);
            //print("color " + color);
            Mat.SetColor("_EmissionColor", color);
            m_energyCount--;
            yield return null;
        }

        print("No more energy");
        UseupEnergy();
    }

    private void UseupEnergy()
    {
        m_IsDisabled = true;
        ThisPlayerMove.IsFalling = true;
        ThisPlayerMove.IsFloating = false;
    }

    private IEnumerator MoveToFlower(Flower _target)
    {
        skinRenderer.enabled = false;

        if (_target.HasLanded == false)
            _target.HasLanded = true;

        while((transform.position - _target.flowerCenter.position).sqrMagnitude > 0.01f)
        {
            Rig.MovePosition(Vector3.Lerp(transform.position, _target.flowerCenter.position, 0.02f));

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(RaiseFromFlower(_target));
    }

    private IEnumerator RaiseFromFlower(Flower _target)
    {
        skinRenderer.enabled = true;

        while(transform.position.y < _target.flowerCenter.position.y + 1)
        {
            Rig.MovePosition(transform.position + Vector3.up * Time.deltaTime * 0.1f);

            yield return null;
        }

        lastFlower = startFlower;
        m_energyCount = m_energyCountDown;
        ThisPlayerMove.SetLaunchable();
    }
}
