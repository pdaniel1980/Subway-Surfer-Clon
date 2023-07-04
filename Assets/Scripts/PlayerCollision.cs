using UnityEngine;

public enum CollisionX { None, Left, Middle, Right }
public enum CollisionY { None, Up, Middle, Down, LowDown}
public enum CollisionZ { None, Forward, Middle, Backward }

public class PlayerCollision : MonoBehaviour
{
    private CollisionX _collisionX;
    private CollisionY _collisionY;
    private CollisionZ _collisionZ;

    private bool _sideCollision = false;


    private CharacterController characterController;
    private PlayerController playerController;

    public CollisionX CollisionX { get => _collisionX; set => _collisionX = value; }
    public CollisionY CollisionY { get => _collisionY; set => _collisionY = value; }
    public CollisionZ CollisionZ { get => _collisionZ; set => _collisionZ = value; }

    public bool SideCollision { get => _sideCollision; set => _sideCollision = value; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    public void OnCharacterCollision(Collider collider)
    {
        _collisionX = GetCollisionX(collider);
        _collisionY = GetCollisionY(collider);
        _collisionZ = GetCollisionZ(collider);
        SetAnimatorCollision(collider);
    }

    private void SetAnimatorCollision(Collider collider)
    {
        if (_collisionZ == CollisionZ.Backward && _collisionX == CollisionX.Middle)
        {
            SetAnimatorCollisionZBackward(collider);
        }
        else if (_collisionZ == CollisionZ.Middle)
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
        else
        {
            if (_collisionX == CollisionX.Left)
            {
                playerController.SetPlayerAnimatorWithLayer(playerController.IdStumbleCornerLeft);
            }
            else if (_collisionX == CollisionX.Right)
            {
                playerController.SetPlayerAnimatorWithLayer(playerController.IdStumbleCornerRight);
            }
        }
    }

    private void SetAnimatorCollisionZBackward(Collider collider)
    {
        if (_collisionY == CollisionY.LowDown && !playerController.IsRolling)
        {
            collider.enabled = false;
            playerController.SetPlayerAnimator(playerController.IdStumbleLow, false);
        }
        else if (_collisionY == CollisionY.Down)
        {
            playerController.SetPlayerAnimator(playerController.IdDeathLower, false);
            playerController.GameManager.EndGame();
        }
        else if (_collisionY == CollisionY.Middle)
        {
            if (collider.CompareTag("MovingTrain"))
            {
                playerController.SetPlayerAnimator(playerController.IdDeathMovingTrain, false);
                playerController.GameManager.EndGame();
            }
            else
            {
                playerController.SetPlayerAnimator(playerController.IdDeathBounce, false);
                playerController.GameManager.EndGame();
            }
        }
        else if (_collisionY == CollisionY.Up && !playerController.IsRolling)
        {
            playerController.SetPlayerAnimator(playerController.IdDeathUpper, false);
            playerController.GameManager.EndGame();
        }
    }

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
