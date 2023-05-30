using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Transform selfTransform;
    private Vector3 cameraOffset;
    private Vector3 followPosition;
    [SerializeField] private float speedOffset;
    [SerializeField] private float maxDistance;
    private float y;

    private void Awake()
    {
        selfTransform = GetComponent<Transform>();
    }

    private void Start()
    {
        cameraOffset = selfTransform.position;
    }

    private void LateUpdate()
    {
        followPosition = target.position + cameraOffset;
        selfTransform.position = followPosition;
        UpdteCameraOffset();
    }

    private void UpdteCameraOffset()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(target.position, Vector3.down, out hitInfo, maxDistance))
        {
            y = Mathf.Lerp(y, hitInfo.point.y, Time.deltaTime * speedOffset);
        }

        followPosition.y = cameraOffset.y + y;
        selfTransform.position = followPosition;
    }

}
