using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private float limit = 2000f;
    [Tooltip("Offset del player al reiniciar su posicion para evitar salto visual segun disposicion del mundo")]
    [SerializeField] private float offsetOnResetPosition = 0f;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        CheckPlayerPosition();
    }

    // Verificamos si llego al limite establecido para resetear la posicion en Z del player
    private void CheckPlayerPosition()
    {
        if (playerTransform.position.z >= limit)
        {
            playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, 0 + offsetOnResetPosition);
        }
    }

}
