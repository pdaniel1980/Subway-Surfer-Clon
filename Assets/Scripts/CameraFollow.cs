using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float speedOffset;
    [SerializeField] private float maxDistance;

    private Transform selfTransform;
    private Vector3 cameraOffset;
    private Vector3 followPosition;
    
    private RaycastHit hitInfo;
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

    // Calculamos el offset de la camara para dar seguimiento al salto segun la distancia seteada contra el nivel del suelo
    private void UpdteCameraOffset()
    {
        // En caso que el player suba sobre una plataforma, recalculamos la altura para seguir con la camara
        if (Physics.Raycast(target.position, Vector3.down, out hitInfo, maxDistance))
        {
            y = Mathf.Lerp(y, hitInfo.point.y, Time.deltaTime * speedOffset);
        }

        followPosition.y = cameraOffset.y + y;
        selfTransform.position = followPosition;
    }
}
