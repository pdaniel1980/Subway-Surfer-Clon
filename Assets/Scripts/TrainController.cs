using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 35f;
    [SerializeField] private GameObject gameObjectTrain;

    private GameManager gameManager;
    private Transform trainTransform;
    private Transform selfTransform;
    private Rigidbody rb;

    // Variables de movimiento del tren
    private Vector3 motion;
    private bool moveTrain;

    // Raycast variables
    private RaycastHit hit;
    private Vector3 rayPosition;
    private readonly float distance = 110f;
    private readonly float sphereRadius = 5f;

    private void Awake()
    {
        rb = gameObjectTrain.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
        trainTransform = gameObjectTrain.transform;
        selfTransform = transform;
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        CheckTrainMovement();
        if (moveTrain)
            MoveTrain();
    }

    private void FixedUpdate()
    {
        rayPosition = new Vector3(trainTransform.position.x, trainTransform.position.y, trainTransform.position.z);
        if (Physics.SphereCast(rayPosition, sphereRadius, Vector3.back, out hit, distance))
        {
            if (hit.transform.parent != null && hit.transform.parent.CompareTag("Player"))
                moveTrain = true;
        }
    }

    private void CheckTrainMovement()
    {
        if (trainTransform.position.z <= selfTransform.position.z || gameManager.GameOver == true)
        {
            StopTrain();
        }       
    }

    private void OnDrawGizmos()
    {
        // Draw a visual representation of the sphere cast in the scene view
        Vector3 sphereDirection = Vector3.back;

        // Draw the sphere cast shape
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rayPosition + sphereDirection * distance, sphereRadius);
    }

    private void MoveTrain()
    {
        motion = new Vector3(trainTransform.position.x, trainTransform.position.y, -forwardSpeed);
        rb.velocity = motion;
    }

    public void StopTrain()
    {
        forwardSpeed = 0;
    }
}
