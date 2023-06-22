using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;
    private Transform playerTransform;
    [SerializeField] private float roadSize;
    [SerializeField] private float limit;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        gameObjects = GameObject.FindGameObjectsWithTag("Road");
        GetRoadSize();
        SetLimitToRestartPostion();
        
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

    private void GetRoadSize()
    {
        foreach (var go in gameObjects)
        {
            roadSize += go.GetComponent<BoxCollider>().bounds.size.z;
        }
    }

    private void SetLimitToRestartPostion()
    {
        limit = roadSize * 0.9f;
    }

}
