using System;
using UnityEngine;

public enum PlayerPosition { Left = -2, Middle = 0, Right = 2 }

public class PlayerController : MonoBehaviour
{
    private PlayerPosition playerPosition;
    private Transform playerTransform;
    private bool swipeLeft, swipeRight;
    private float newXPosition;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
    }

    void Start()
    {
        playerPosition = PlayerPosition.Middle;
    }
    
    void Update()
    {
        GetSwipe();
        SetPlayerPosition();
    }

    private void GetSwipe()
    {
        swipeLeft = Input.GetKeyDown(KeyCode.LeftArrow);
        swipeRight = Input.GetKeyDown(KeyCode.RightArrow);
    }

    private void SetPlayerPosition()
    {
        if (swipeLeft)
        {
            if (playerPosition == PlayerPosition.Middle)
            {
                UpdatePlayerXPosition(PlayerPosition.Left);
            }
            else if (playerPosition == PlayerPosition.Right)
            {
                UpdatePlayerXPosition(PlayerPosition.Middle);
            }
        }
        else if (swipeRight)
        {
            if (playerPosition == PlayerPosition.Left)
            {
                UpdatePlayerXPosition(PlayerPosition.Middle);
            }
            else if (playerPosition == PlayerPosition.Middle)
            {
                UpdatePlayerXPosition(PlayerPosition.Right);
            }
        }

        MovePlayer();
    }

    private void UpdatePlayerXPosition(PlayerPosition playerPos)
    {
        newXPosition = (int)playerPos;
        playerPosition = playerPos;
    }

    private void MovePlayer()
    {
        playerTransform.position = new Vector3(newXPosition, 0, 0);
    }
}
