using System.Collections;
using UnityEngine;

public class TransparentController : MonoBehaviour
{
    [Header("Transparent Setting")]
    [SerializeField] private float raycastDistance = 8f;
    [SerializeField] private float alphaTarget = 0.4f;
    [SerializeField] private float radius = 0.8f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float timeToTransparent = 0.3f;
    [SerializeField] private Material defaultMaterial;
    private Camera mainCamera;

    private RaycastHit hitInfo;
    private Vector3 origin;
    private MeshRenderer obstacleMeshRenderer;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        origin = new Vector3(mainCamera.transform.position.x, 1f, mainCamera.transform.position.z);
        CheckObstacle();
        SetObstacleDefaultMaterial();
    }

    // Verificamos si damos con un object configurado como obstaculo
    private void CheckObstacle()
    {
        Ray ray = new Ray(origin, Vector3.forward);

        // Si la esfera colisiona con un elemento que perteneza al layerMask pre-configurada, invocamos al coroutine para transparentar
        if (Physics.SphereCast(ray, radius, out hitInfo, raycastDistance, layerMask))
        {
            _ = StartCoroutine(SetTransparencyRoutine());
        }
    }

    // Aplicar transparencia al objeto colisionado con un efecto fade
    IEnumerator SetTransparencyRoutine()
    {
        obstacleMeshRenderer = hitInfo.transform.GetComponent<MeshRenderer>();

        for (float t = 1f; t >= alphaTarget; t -= (Time.deltaTime / timeToTransparent))
        {
            obstacleMeshRenderer.material.SetFloat(Shader.PropertyToID("_Alpha"), t);
            
            yield return null;
        }
    }

    // Bugfix: Metodo necesario para volver a colocar el material inicial al objeto que se transparento
    private void SetObstacleDefaultMaterial()
    {
        // Al salir del foco de la camara, seteamos el dafaultMaterial
        if (obstacleMeshRenderer != null && obstacleMeshRenderer.transform.position.z < mainCamera.transform.position.z)
        {
            obstacleMeshRenderer.material = defaultMaterial;
        }
    }
}