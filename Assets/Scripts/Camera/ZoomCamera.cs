using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ZoomCamera : MonoBehaviour
{
    [SerializeField]
    private float incline = 45;

    [SerializeField]
    private float minDistance = 10;

    [SerializeField]
    private float maxDistance = 20;

    private float currentDistance;

    [SerializeField]
    private float zoomSensitivity = 1;

    private float zoomLevel;

    public Transform target;
    private Camera m_camera;

    public Camera ThisCamera
    {
        get
        {
            if (m_camera == null)
                m_camera = GetComponent<Camera>();
            return m_camera;
        }
    }

    void Awake()
    {
        transform.rotation = Quaternion.AngleAxis(incline, Vector3.right);
        zoomLevel = 0.5f;
        currentDistance = Mathf.Lerp(maxDistance, minDistance, zoomLevel);
        transform.position = target.position - transform.forward * currentDistance;
    }

    void FixedUpdate()
    {
        currentDistance = Vector3.Dot(transform.forward, target.position - transform.position);
        Vector3 projectVec = currentDistance * transform.forward;
        Vector3 projectPos = transform.position + projectVec;

        //zoomLevel = (currentDistance - minDistance) / (maxDistance - minDistance);
        //if (zoomLevel < 0)
        //    zoomLevel = Mathf.Lerp(zoomLevel, 0.3f, Mathf.Abs(zoomLevel) * 5 * Time.deltaTime);
        //if (zoomLevel > 1)
        //    zoomLevel = Mathf.Lerp(zoomLevel, 0.9f, Mathf.Abs(zoomLevel - 1) * 5 * Time.deltaTime);

        float zoomInput = -Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        //print("input " + zoomInput);
        if (zoomInput.Sgn() > 0)
            zoomLevel = Mathf.Lerp(zoomLevel, 1, Mathf.Abs(zoomInput) * Time.deltaTime);
        else if (zoomInput.Sgn() < 0)
            zoomLevel = Mathf.Lerp(zoomLevel, 0, Mathf.Abs(zoomInput) * Time.deltaTime);
        //print("level " + zoomLevel);

        transform.position = projectPos -
            transform.forward * (zoomLevel * (maxDistance - minDistance) + minDistance);
    }
}
