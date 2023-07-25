using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Header("Train Settings")]
    [Tooltip("Velocidad del tren")]
    [SerializeField] private float forwardSpeed = 35f;
    [Tooltip("Distancia del ray para detectar al player")]
    [SerializeField] private float scopeDistance = 110f;
    [SerializeField] private LayerMask layerMask;

    private GameManager gameManager;
    private Camera mainCamera;
    private Transform trainTransform;
    private Vector3 startTrainPosition;
    private Rigidbody rb;

    // Variables de movimiento del tren
    private Vector3 motion;
    private bool moveTrain;

    // Raycast variables
    private Ray rayPosition;

    private readonly float sphereRadius = 5f;
    private float limitToMove;

    private void Awake()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        trainTransform = gameObject.transform;
        startTrainPosition = trainTransform.position;

        // Etiquetamos al objecto como MovingTrain necesario para establecer la animacion cuando choca al player
        gameObject.tag = "MovingTrain";
        // Calculamos el limite hasta donde puede avanzar el ten
        limitToMove = trainTransform.position.z - scopeDistance;
        // Establecemos la velocidad para el movimiento del tren
        motion = new Vector3(0, 0, -forwardSpeed);

        SetInitRigibodyConstraints();
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainCamera = GameObject.FindObjectOfType<Camera>();
    }

    private void Update()
    {
        CheckTrainPosition();
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

    private void CheckTrainPosition()
    {
        // Si llegamos al limite de movimiento frenamos el tren para evitar que avance de forma infinita
        if (trainTransform.position.z <= limitToMove || gameManager.GameOver == true)
        {
            StopTrain();
        }

        // Si la camara pasa la posicion inicial del tren + su tamanho, entonces inicializamos los atributos
        if (trainTransform.position != startTrainPosition && mainCamera.transform.position.z > startTrainPosition.z + GetTrainSize())
        {
            ResetTrainAttributes();
        }
    }

    private float GetTrainSize()
    {
        return trainTransform.GetComponent<Collider>().bounds.size.z;
    }

    private void MoveTrain()
    {
        rb.velocity = motion;
    }

    public void StopTrain()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        moveTrain = false;
    }

    private void ResetTrainAttributes()
    {
        SetInitRigibodyConstraints();
        trainTransform.position = startTrainPosition;
    }

    private void SetInitRigibodyConstraints()
    {
        // Solo dejamos la posicion Z sin congelar para que el tren pueda moverse cuando sea necesario
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }
}