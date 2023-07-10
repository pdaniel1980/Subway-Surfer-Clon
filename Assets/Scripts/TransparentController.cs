using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentController : MonoBehaviour
{

    private Camera mainCamera;
    [SerializeField] private float raycastDistance = 8f;
    [SerializeField] private float alphaTarget = 0.4f;
    [SerializeField] private float radius = 0.8f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float timeToTransparent = 0.3f;
    [SerializeField] private Material defaultMaterial;

    private RaycastHit hitInfo;
    private Vector3 origin;
    private MeshRenderer obstacleMeshRenderer;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        origin = new Vector3(mainCamera.transform.position.x, 1f, mainCamera.transform.position.z);
        CheckObstacle();
        SetObstacleDefaultMaterial();
    }

    private void CheckObstacle()
    {
        Ray ray = new Ray(origin, Vector3.forward);

        if (Physics.SphereCast(ray, radius, out hitInfo, raycastDistance, layerMask))
        {
            _ = StartCoroutine(SetTransparencyRoutine());
        }
    }

    IEnumerator SetTransparencyRoutine()
    {
        obstacleMeshRenderer = hitInfo.transform.GetComponent<MeshRenderer>();

        for (float t = 1f; t >= alphaTarget; t -= (Time.deltaTime / timeToTransparent))
        {
            obstacleMeshRenderer.material.SetFloat(Shader.PropertyToID("_Alpha"), t);
            
            yield return null;
        }
    }

    private void SetObstacleDefaultMaterial()
    {
        if (obstacleMeshRenderer != null && obstacleMeshRenderer.transform.position.z < mainCamera.transform.position.z)
        {
            obstacleMeshRenderer.material = defaultMaterial;
        }
    }
}
