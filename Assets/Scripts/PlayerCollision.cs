using UnityEngine;

public enum CollisionX { None, Left, Middle, Right }
public enum CollisionY { None, Up, Middle, Down, LowDown}
public enum CollisionZ { None, Forward, Middle, Backward }

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private CollisionX _collisionX;
    [SerializeField] private CollisionY _collisionY;
    [SerializeField] private CollisionZ _collisionZ;

    private bool _sideCollision = false;

    private CharacterController characterController;
    private PlayerController playerController;

    public CollisionX CollisionX { get => _collisionX; set => _collisionX = value; }
    public CollisionY CollisionY { get => _collisionY; set => _collisionY = value; }
    public CollisionZ CollisionZ { get => _collisionZ; set => _collisionZ = value; }

    public bool SideCollision { get => _sideCollision; set => _sideCollision = value; }

    private string _colliderTag;
    public string ColliderTag { get => _colliderTag; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    // Obtenemos informacion de la colision y establecemos la animacion
    public void OnCharacterCollision(Collider collider)
    {
        _collisionX = GetCollisionX(collider);
        _collisionY = GetCollisionY(collider);
        _collisionZ = GetCollisionZ(collider);
        _colliderTag = collider.tag;
        SetAnimatorCollision(collider);
    }

    private void SetAnimatorCollision(Collider collider)
    {
        // Aseguramos que las colisiones de impacto no se ejecuten cuando subimos a una rampa (Objetos con tag: Ramp)
        if (!collider.CompareTag("Ramp"))
        { 
            if (_collisionZ == CollisionZ.Backward && _collisionX == CollisionX.Middle)
            {
                SetAnimatorCollisionZBackward(collider);
            }
            else if (_collisionZ == CollisionZ.Middle)
            {
                SetAnimatorCollisionZMiddle();
            }
            else
            {
                SetAnimatorCollisionZCorner(collider);
            }
        }
    }

    // Establecer de animaciones cuando colisiona en la parte de atras del objeto
    private void SetAnimatorCollisionZBackward(Collider collider)
    {
        if (_collisionY == CollisionY.LowDown && !playerController.IsRolling)
        {
            collider.enabled = false;
            playerController.SetPlayerAnimator(playerController.IdStumbleLow, false);
        }
        else if (_collisionY == CollisionY.Down)
        {
            SetDeathAnimator(playerController.IdDeathLower);
        }
        else if (_collisionY == CollisionY.Middle)
        {
            if (collider.CompareTag("MovingTrain"))
            {
                SetDeathAnimator(playerController.IdDeathMovingTrain);
            }
            else
            {
                SetDeathAnimator(playerController.IdDeathBounce);
            }
        }
        else if (_collisionY == CollisionY.Up && !playerController.IsRolling)
        {
            SetDeathAnimator(playerController.IdDeathUpper);
        }
    }

    // Establecer de animaciones cuando colisiona en los lados del objeto
    private void SetAnimatorCollisionZMiddle()
    {
        if (_collisionX == CollisionX.Left)
        {
            playerController.SetPlayerAnimator(playerController.IdStumbleSideLeft, false);
            _sideCollision = true;
        }
        else if (_collisionX == CollisionX.Right)
        {
            playerController.SetPlayerAnimator(playerController.IdStumbleSideRight, false);
            _sideCollision = true;
        }
    }

    // Establecer de animaciones cuando colisiona en las esquinas de los objetos
    private void SetAnimatorCollisionZCorner(Collider collider)
    {
        if (collider.CompareTag("Obstacle") || collider.CompareTag("Train"))
        {
            SetDeathAnimator(playerController.IdDeathBounce);
        }
        else if (collider.CompareTag("MovingTrain"))
        {
            SetDeathAnimator(playerController.IdDeathMovingTrain);
        }
        else if (_collisionX == CollisionX.Left)
        {
            playerController.SetPlayerAnimatorWithLayer(playerController.IdStumbleCornerLeft);
        }
        else if (_collisionX == CollisionX.Right)
        {
            playerController.SetPlayerAnimatorWithLayer(playerController.IdStumbleCornerRight);
        }
    }

    private void SetDeathAnimator(int IdAnimation)
    {
        playerController.SetPlayerAnimator(IdAnimation, false);
        playerController.GameManager.EndGame();
    }

    // Calculo de los puntos de colision en el eje X (lado del objeto)
    private CollisionX GetCollisionX(Collider collider)
    {
        Bounds characterControllerBounds = characterController.bounds;
        Bounds colliderBounds = collider.bounds;
        float minX = Mathf.Max(colliderBounds.min.x, characterControllerBounds.min.x);
        float maxX = Mathf.Min(colliderBounds.max.x, characterControllerBounds.max.x);
        float average = (minX + maxX) / 2 - colliderBounds.min.x;

        CollisionX colX;

        if (average > colliderBounds.size.x - 0.33f)
        {
            colX = CollisionX.Right;
        }
        else if (average < 0.33f)
        {
            colX = CollisionX.Left;
        }
        else
        {
            colX = CollisionX.Middle;
        }

        return colX;
    }

    // Calculo de los puntos de colision en el eje Y (altura del objeto)
    private CollisionY GetCollisionY(Collider collider)
    {
        Bounds characterControllerBounds = characterController.bounds;
        Bounds colliderBounds = collider.bounds;
        float minY = Mathf.Max(colliderBounds.min.y, characterControllerBounds.min.y);
        float maxY = Mathf.Min(colliderBounds.max.y, characterControllerBounds.max.y);
        float average = (minY + maxY) / 2 - colliderBounds.min.y;

        CollisionY colY;

        if (average > colliderBounds.size.y - 0.33f)
        {
            colY = CollisionY.Up;
        }
        else if (average < 0.16f)
        {
            colY = CollisionY.LowDown;
        }
        else if (average < 0.33f)
        {
            colY = CollisionY.Down;
        }
        else
        {
            colY = CollisionY.Middle;
        }

        return colY;
    }

    // Calculo de los puntos de colision en el eje Z (largo del objeto)
    private CollisionZ GetCollisionZ(Collider collider)
    {
        Bounds characterControllerBounds = characterController.bounds;
        Bounds colliderBounds = collider.bounds;
        float minZ = Mathf.Max(colliderBounds.min.z, characterControllerBounds.min.z);
        float maxZ = Mathf.Min(colliderBounds.max.z, characterControllerBounds.max.z);
        float average = (minZ + maxZ) / 2 - colliderBounds.min.z;

        CollisionZ colZ;

        if (average > colliderBounds.size.z - 0.33f)
        {
            colZ = CollisionZ.Forward;
        }
        else if (average < 0.33f)
        {
            colZ = CollisionZ.Backward;
        }
        else
        {
            colZ = CollisionZ.Middle;
        }

        return colZ;
    }
}
