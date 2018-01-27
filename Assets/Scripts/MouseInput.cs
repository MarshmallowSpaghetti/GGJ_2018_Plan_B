using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class MouseInput : BaseSingletonMono<MouseInput>
{
    private Vector3 m_mousePos;
    public Transform cursorTrans;
    private Vector3 m_mouseLastPos;

    public Vector3 MousePos
    {
        get
        {
            return m_mousePos;
        }

        private set
        {
            m_mousePos = value;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsMouseMoving())
            UpdateMouseHitPos();
    }

    private void UpdateMouseHitPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(CrossPlatformInputManager.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
        {
            MousePos = hit.point;
        }

        if (cursorTrans)
            cursorTrans.position = MousePos;
        else
            transform.position = MousePos;
    }

    private bool IsMouseMoving()
    {
        if ((CrossPlatformInputManager.mousePosition - m_mouseLastPos).sqrMagnitude > 0.001f)
        {
            m_mouseLastPos = CrossPlatformInputManager.mousePosition;
            return true;
        }
        else
        {
            m_mouseLastPos = CrossPlatformInputManager.mousePosition;
            return false;
        }
    }
}
