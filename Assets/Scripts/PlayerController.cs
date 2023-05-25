using System;
using UnityEngine;

public enum PlayerPosition { Left = -2, Middle = 0, Right = 2 }

public class PlayerController : MonoBehaviour
{
    private PlayerPosition playerPosition;
    private Transform playerTransform;
    private bool swipeLeft, swipeRight;
    private float newXPosition;
    private float xPosition;
    [SerializeField] private float dodgeSpeed;
    private Animator playerAnimator;
    private int IdDodgeLeft = Animator.StringToHash("DodgeLeft");
    private int IdDodgeRight = Animator.StringToHash("DodgeRight");

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
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
                SetDodgePlayerAnimator(IdDodgeLeft);
            }
            else if (playerPosition == PlayerPosition.Right)
            {
                UpdatePlayerXPosition(PlayerPosition.Middle);
                SetDodgePlayerAnimator(IdDodgeLeft);
            }
        }
        else if (swipeRight)
        {
            if (playerPosition == PlayerPosition.Left)
            {
                UpdatePlayerXPosition(PlayerPosition.Middle);
                SetDodgePlayerAnimator(IdDodgeRight);
            }
            else if (playerPosition == PlayerPosition.Middle)
            {
                UpdatePlayerXPosition(PlayerPosition.Right);
                SetDodgePlayerAnimator(IdDodgeRight);
            }
        }

        MovePlayer();
    }

    private void UpdatePlayerXPosition(PlayerPosition playerPos)
    {
        newXPosition = (int)playerPos;
        playerPosition = playerPos;
    }

    private void SetDodgePlayerAnimator(int id)
    {
        playerAnimator.Play(id);
    }
    

    private void MovePlayer()
    {
        xPosition = Mathf.Lerp(xPosition, newXPosition, dodgeSpeed * Time.deltaTime);
        playerTransform.position = new Vector3(xPosition, 0, 0);
    }
}
