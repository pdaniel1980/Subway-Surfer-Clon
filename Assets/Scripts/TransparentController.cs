using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentController : MonoBehaviour
{

    private CharacterController characterController;
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private float alpha = 0.5f;

    private RaycastHit hitInfo;
    private Vector3 origin;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        origin = new Vector3(characterController.transform.position.x, 1.5f, characterController.transform.position.z);
        CheckObstacle();
    }

    private void CheckObstacle()
    {
        Ray ray = new Ray(origin, Vector3.forward);

        if (Physics.Raycast(ray, out hitInfo, raycastDistance))
        {
            Debug.Log("Hit: " + hitInfo.transform.name);

            if (hitInfo.transform.CompareTag("Obstacle"))
            {
                Debug.Log("Hit: " + hitInfo.transform.name);
                SetTransparency();
            }
        }
    }

    private void SetTransparency()
    {
        Renderer renderer = hitInfo.transform.GetComponent<Renderer>();
        renderer.material.SetFloat(Shader.PropertyToID("_Alpha"), alpha);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 sphereDirection = Vector3.forward;

        // Draw the sphere cast shape
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, sphereDirection * raycastDistance);

    }
}
