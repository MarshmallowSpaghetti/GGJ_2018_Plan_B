using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraElveTracker : MonoBehaviour
{
    public Camera Camera { get { return m_camera; } }
    public Transform target;
    private Camera m_camera;
    private float m_startHeight;

    // Use this for initialization
    void Awake()
    {
        Init();
    }

    void Init()
    {
        m_camera = GetComponent<Camera>();

        m_startHeight = m_camera.transform.position.y - target.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateHeight();
    }

    public void UpdateHeight()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0, transform.position.z) + Vector3.up * (target.position.y + m_startHeight), 8 * Time.deltaTime);
        }
    }
}
