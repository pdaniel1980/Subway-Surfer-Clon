using UnityEngine;

public enum CollisionX { None, Left, Middle, Right }
public enum CollisionY { None, Up, Middle, Down}
public enum CollisionZ { None, Forward, Middle, Backward }

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private CollisionX collisionX;
    [SerializeField] private CollisionY collisionY;
    [SerializeField] private CollisionZ collisionZ;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void OnCharacterCollision(Collider collider)
    {
        collisionX = GetCollisionX(collider);
        collisionY = GetCollisionY(collider);
        collisionZ = GetCollisionZ(collider);
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

        if (average > colliderBounds.size.x - 0.33f)
        {
            colY = CollisionY.Up;
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
