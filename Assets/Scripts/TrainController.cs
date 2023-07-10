using UnityEngine;

public class TrainController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 35f;
    [SerializeField] private GameObject gameObjectTrain;
    [SerializeField] private float scopeDistance = 110f;
    [SerializeField] private LayerMask layerMask;

    private GameManager gameManager;
    private Transform trainTransform;
    private Transform selfTransform;
    private Rigidbody rb;

    // Variables de movimiento del tren
    private Vector3 motion;
    private bool moveTrain;

    // Raycast variables
    private RaycastHit hitInfo;
    private Ray rayPosition;
    
    private readonly float sphereRadius = 5f;

    private void Awake()
    {
        rb = gameObjectTrain.AddComponent<Rigidbody>();
        trainTransform = gameObjectTrain.transform;
        gameObjectTrain.tag = "MovingTrain";
        selfTransform = transform;
        motion = new Vector3(0, 0, -forwardSpeed);
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
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
        rayPosition = new Ray(trainTransform.position, Vector3.back);

        if (Physics.SphereCast(rayPosition, sphereRadius, scopeDistance, layerMask))
        {
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

    private void MoveTrain()
    {
        rb.velocity = motion;
    }

    public void StopTrain()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        moveTrain = false;
        forwardSpeed = 0;
    }
}
