using UnityEngine;

public class SceneController : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private float limit = 2013.5f;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        if (playerTransform.position.z >= limit)
        {
            playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);
        }
    }

}
