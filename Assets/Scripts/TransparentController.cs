using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentController : MonoBehaviour
{

    private CharacterController characterController;
    [SerializeField] private float raycastDistance = 3f;

    private RaycastHit hitInfo;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }

    private void CheckObstacle()
    {
        Ray ray = new Ray(characterController.transform.position, Vector3.forward);

        if (Physics.Raycast(ray, out hitInfo, raycastDistance))
        {

        }
    }
}
