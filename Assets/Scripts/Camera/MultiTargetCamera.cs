using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MultiTargetCamera : MonoBehaviour
{
    public enum MovementAxis
    {
        XY,
        XZ,
        YZ
    }

    public List<CameraTarget> CameraTargets = new List<CameraTarget>();

    public MovementAxis Axis;

    public bool FollowHorizontal = true;
    public float HorizontalFollowSmoothness = 0.15f;

    public bool FollowVertical = true;
    public float VerticalFollowSmoothness = 0.15f;

    Func<Vector3, float> Vector3H;
    Func<Vector3, float> Vector3V;
    Func<Vector3, float> Vector3D;
    Func<float, float, Vector3> VectorHV;
    Func<float, float, float, Vector3> VectorHVD;

    float _cameraTargetHorizontalPositionSmoothed;
    float _cameraTargetVerticalPositionSmoothed;
    float _previousCameraTargetHorizontalPositionSmoothed;
    float _previousCameraTargetVerticalPositionSmoothed;
    public Vector3 TargetsMidPoint { get; private set; }
    public Vector3 CameraTargetPosition { get; private set; }
    public float DeltaTime { get; private set; }

    private void Awake()
    {
        // Reset the axis functions
        ResetAxisFunctions();

        // Remove empty targets
        for (int i = CameraTargets.Count -1 ; i >= 0; i--)
        {
            if (CameraTargets[i].TargetTransform == null)
            {
                CameraTargets.RemoveAt(i);
            }
        }
    }

    private void Start()
    {
        // Set some values ahead of the update loop so that other extensions can use them on Awake/Start
        TargetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
        _cameraTargetHorizontalPositionSmoothed = Vector3H(TargetsMidPoint);
        _cameraTargetVerticalPositionSmoothed = Vector3V(TargetsMidPoint);
        DeltaTime = Time.deltaTime;

        // Center on target
        if (CameraTargets.Count > 0)
        {
            var targetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
            var cameraTargetPositionX = FollowHorizontal ? Vector3H(targetsMidPoint) : Vector3H(transform.localPosition);
            var cameraTargetPositionY = FollowVertical ? Vector3V(targetsMidPoint) : Vector3V(transform.localPosition);
            var finalPos = new Vector2(cameraTargetPositionX, cameraTargetPositionY);
            MoveCameraInstantlyToPosition(finalPos);
        }
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    /// <summary>Moves the camera instantly to the supplied position</summary>
    /// <param name="cameraPos">The final position of the camera</param>
    public void MoveCameraInstantlyToPosition(Vector2 cameraPos)
    {
        transform.localPosition = VectorHVD(cameraPos.x, cameraPos.y, Vector3D(transform.localPosition));

        ResetMovement();
    }

    /// <summary>
    /// Cancels any existing camera movement easing. 
    /// Also check CenterOnTargets and MoveCameraInstantlyToPosition.
    /// </summary>
    public void ResetMovement()
    {
        CameraTargetPosition = transform.localPosition;

        _cameraTargetHorizontalPositionSmoothed = Vector3H(CameraTargetPosition);
        _cameraTargetVerticalPositionSmoothed = Vector3V(CameraTargetPosition);

        _previousCameraTargetHorizontalPositionSmoothed = _cameraTargetHorizontalPositionSmoothed;
        _previousCameraTargetVerticalPositionSmoothed = _cameraTargetVerticalPositionSmoothed;
    }

    void ResetAxisFunctions()
    {
        switch (Axis)
        {
            case MovementAxis.XY:
                Vector3H = vector => vector.x;
                Vector3V = vector => vector.y;
                Vector3D = vector => vector.z;
                VectorHV = (h, v) => new Vector3(h, v, 0);
                VectorHVD = (h, v, d) => new Vector3(h, v, d);
                break;
            case MovementAxis.XZ:
                Vector3H = vector => vector.x;
                Vector3V = vector => vector.z;
                Vector3D = vector => vector.y;
                VectorHV = (h, v) => new Vector3(h, 0, v);
                VectorHVD = (h, v, d) => new Vector3(h, d, v);
                break;
            case MovementAxis.YZ:
                Vector3H = vector => vector.z;
                Vector3V = vector => vector.y;
                Vector3D = vector => vector.x;
                VectorHV = (h, v) => new Vector3(0, v, h);
                VectorHVD = (h, v, d) => new Vector3(d, v, h);
                break;
        }
    }

    Vector3 GetTargetsWeightedMidPoint(ref List<CameraTarget> _targets)
    {
        var midPointH = 0f;
        var midPointV = 0f;

        if (_targets.Count == 0)
            return transform.localPosition;

        var totalInfluencesH = 0f;
        var totalInfluencesV = 0f;
        var totalAccountableTargetsH = 0;
        var totalAccountableTargetsV = 0;
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == null || _targets[i].TargetTransform == null)
            {
                _targets.RemoveAt(i);
                continue;
            }

            midPointH += (Vector3H(_targets[i].TargetPosition) + _targets[i].TargetOffset.x) * _targets[i].TargetInfluenceH;
            midPointV += (Vector3V(_targets[i].TargetPosition) + _targets[i].TargetOffset.y) * _targets[i].TargetInfluenceV;

            totalInfluencesH += _targets[i].TargetInfluenceH;
            totalInfluencesV += _targets[i].TargetInfluenceV;

            if (_targets[i].TargetInfluenceH > 0)
                totalAccountableTargetsH++;

            if (_targets[i].TargetInfluenceV > 0)
                totalAccountableTargetsV++;
        }

        // Set totalInfluences to be 1
        if (totalInfluencesH < 1 && totalAccountableTargetsH == 1)
            totalInfluencesH += (1 - totalInfluencesH);

        // Set totalInfluences to be 1
        if (totalInfluencesV < 1 && totalAccountableTargetsV == 1)
            totalInfluencesV += (1 - totalInfluencesV);

        if (totalInfluencesH > .0001f)
            midPointH /= totalInfluencesH;

        if (totalInfluencesV > .0001f)
            midPointV /= totalInfluencesV;

        return VectorHV(midPointH, midPointV);
    }

    /// <summary>
    /// Move the camera to the average position of all the targets.
    /// This method is automatically called when using LateUpdate or FixedUpdate.
    /// If using ManualUpdate, you have to call it yourself.
    /// </summary>
    /// <param name="deltaTime">The time in seconds it took to complete the last frame</param>
    public void Move(float deltaTime)
    {
        // Delta time
        DeltaTime = deltaTime;
        if (DeltaTime < .0001f)
            return;
        
        // Calculate targets mid point
        TargetsMidPoint = GetTargetsWeightedMidPoint(ref CameraTargets);
        CameraTargetPosition = TargetsMidPoint;

        // Follow only on selected axis
        var cameraTargetPositionX = FollowHorizontal ? Vector3H(CameraTargetPosition) : Vector3H(transform.localPosition);
        var cameraTargetPositionY = FollowVertical ? Vector3V(CameraTargetPosition) : Vector3V(transform.localPosition);
        CameraTargetPosition = VectorHV(cameraTargetPositionX, cameraTargetPositionY);

        // Tween camera final position
        _cameraTargetHorizontalPositionSmoothed = SmoothApproach(_cameraTargetHorizontalPositionSmoothed, _previousCameraTargetHorizontalPositionSmoothed, Vector3H(CameraTargetPosition), 1f / HorizontalFollowSmoothness, DeltaTime);
        _previousCameraTargetHorizontalPositionSmoothed = _cameraTargetHorizontalPositionSmoothed;

        _cameraTargetVerticalPositionSmoothed = SmoothApproach(_cameraTargetVerticalPositionSmoothed, _previousCameraTargetVerticalPositionSmoothed, Vector3V(CameraTargetPosition), 1f / VerticalFollowSmoothness, DeltaTime);
        _previousCameraTargetVerticalPositionSmoothed = _cameraTargetVerticalPositionSmoothed;

        // Movement this step
        var horizontalDeltaMovement = _cameraTargetHorizontalPositionSmoothed - Vector3H(transform.localPosition);
        var verticalDeltaMovement = _cameraTargetVerticalPositionSmoothed - Vector3V(transform.localPosition);

        // Calculate the base delta movement
        var deltaMovement = VectorHV(horizontalDeltaMovement, verticalDeltaMovement);
        

        // Calculate the new position
        var newPos = transform.localPosition + deltaMovement;

        // Apply the new position
        transform.localPosition = VectorHVD(Vector3H(newPos), Vector3V(newPos), Vector3D(transform.localPosition));
    }

    public float SmoothApproach(float pastPosition, float pastTargetPosition, float targetPosition, float speed, float deltaTime)
    {
        float t = deltaTime * speed;
        float v = (targetPosition - pastTargetPosition) / t;
        float f = pastPosition - pastTargetPosition + v;
        return targetPosition - v + f * Mathf.Exp(-t);
    }
}
