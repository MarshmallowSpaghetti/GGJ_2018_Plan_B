using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowerCamera : MonoBehaviour
{
    public Rect hardEdge;
    public Rect softEdge;
    public Transform target;
    private Camera m_camera;
    private Vector3 m_previousPos;

    public bool isAlwaysBehind;

    public Camera ThisCamera
    {
        get
        {
            if (m_camera == null)
                m_camera = GetComponent<Camera>();
            return m_camera;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_previousPos = ThisCamera.WorldToViewportPoint(target.position);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Due to render order, the rect may not be the exact one.
        //DrawRect();

        CheckTargetViewportPos();

        RotateCamera();
    }

    private void CheckTargetViewportPos()
    {
        Vector3 targetViewportPos = ThisCamera.WorldToViewportPoint(target.position);

        //if (hardEdge.Contains(targetViewportPos) == false)
        //{
        //    //print("target pos " + targetViewportPos);
        //    // Clamp it
        //    targetViewportPos = new Vector3(
        //        // A little offse is to make sure it fall back into soft region.
        //        Mathf.Clamp(targetViewportPos.x, hardEdge.xMin * 1.05f, hardEdge.xMax * 0.95f),
        //        Mathf.Clamp(targetViewportPos.y, hardEdge.yMin * 1.05f, hardEdge.yMax * 0.95f),
        //        targetViewportPos.z
        //        );

        //    Vector3 clampedWorldPos = ThisCamera.ViewportToWorldPoint(targetViewportPos);
        //    // Fix camera to a certain height
        //    ThisCamera.transform.position -= (clampedWorldPos - target.position).SetY(0);
        //}
        //else if (softEdge.Contains(targetViewportPos) == false)
        if (softEdge.Contains(targetViewportPos) == false)
        {
            float offset =
            Mathf.Max(softEdge.xMin - targetViewportPos.x, 0) / (softEdge.xMin - hardEdge.xMin)
            + Mathf.Max(targetViewportPos.x - softEdge.xMax, 0) / (hardEdge.xMax - softEdge.xMax)
            + Mathf.Max(softEdge.yMin - targetViewportPos.y, 0) / (softEdge.yMin - hardEdge.yMin)
            + Mathf.Max(targetViewportPos.y - softEdge.yMax, 0) / (hardEdge.yMax - softEdge.yMax);
            offset = Mathf.Max(offset * 5, 0.4f);

            Vector3 lerpWorldPos = ThisCamera.ViewportToWorldPoint(
                Vector3.Lerp(targetViewportPos, new Vector3(
                    0.5f,
                    0.5f,
                    targetViewportPos.z), offset * 3f * Time.deltaTime));
            // If glitch still exists, use the fix camera below
            //Vector3 lerpWorldPos = ThisCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, targetViewportPos.z));

            ThisCamera.transform.position -= (lerpWorldPos - target.position);
        }

        m_previousPos = targetViewportPos;
    }

    private void RotateCamera()
    {
        if (isAlwaysBehind == false)
            return;
        
        float yOffset = transform.position.y - target.position.y;

        transform.position = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward.SetY(0), target.forward.SetY(0), Vector3.up), Vector3.up)
            * (transform.position - target.position) + target.position;

        //float forwardYOffset = transform.forward.y;
        //transform.forward = (target.position - transform.position).SetY(forwardYOffset);

        transform.forward = Quaternion.FromToRotation(transform.forward.SetY(0).normalized,
            (target.position - transform.position).SetY(0).normalized) * transform.forward;
        //print("forward " + transform.forward);
    }

    private void OnDrawGizmos()
    {
        DrawRect(hardEdge, Color.red);
        DrawRect(softEdge, Color.yellow);
    }

    private void DrawRect(Rect _rect, Color _color)
    {
        Vector3 leftDown = ThisCamera.ViewportToWorldPoint(((Vector3)_rect.min).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightUp = ThisCamera.ViewportToWorldPoint(((Vector3)_rect.max).SetZ(ThisCamera.nearClipPlane + 0.1f));
        Vector3 leftUp = ThisCamera.ViewportToWorldPoint(new Vector3(_rect.xMin, _rect.yMax, ThisCamera.nearClipPlane + 0.1f));
        Vector3 rightDown = ThisCamera.ViewportToWorldPoint(new Vector3(_rect.xMax, _rect.yMin, ThisCamera.nearClipPlane + 0.1f));

        Gizmos.color = _color;
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftUp, rightUp);
        Gizmos.DrawLine(rightUp, rightDown);
        Gizmos.DrawLine(rightDown, leftDown);
    }
}
