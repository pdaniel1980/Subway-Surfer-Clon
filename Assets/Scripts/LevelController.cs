using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private float limit = 2086f;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void Update()
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
