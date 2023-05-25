using System;
using UnityEngine;

public enum PlayerPosition { Left = -2, Middle = 0, Right = 2 }

public class PlayerController : MonoBehaviour
{
    private PlayerPosition playerPosition;
    private Transform playerTransform;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;
    private float newXPosition;
    private float xPosition;
    private float yPosition;

    [SerializeField] private float dodgeSpeed;

    private Animator playerAnimator;
    private int IdDodgeLeft = Animator.StringToHash("DodgeLeft");
    private int IdDodgeRight = Animator.StringToHash("DodgeRight");
    private int IdJump = Animator.StringToHash("Jump");
    private int IdFall = Animator.StringToHash("Fall");
    private int IdLanding = Animator.StringToHash("Landing");

    private CharacterController characterController;
    private Vector3 motionPlayer;
    [SerializeField] private float jumpPower = 7f;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
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
        swipeUp = Input.GetKeyDown(KeyCode.UpArrow);
        swipeDown = Input.GetKeyDown(KeyCode.DownArrow);
    }

    private void SetPlayerPosition()
    {
        if (swipeLeft)
        {
            if (playerPosition == PlayerPosition.Middle)
            {
                UpdatePlayerXPosition(PlayerPosition.Left);
                SetPlayerAnimator(IdDodgeLeft, false);
            }
            else if (playerPosition == PlayerPosition.Right)
            {
                UpdatePlayerXPosition(PlayerPosition.Middle);
                SetPlayerAnimator(IdDodgeLeft, false);
            }
        }
        else if (swipeRight)
        {
            if (playerPosition == PlayerPosition.Left)
            {
                UpdatePlayerXPosition(PlayerPosition.Middle);
                SetPlayerAnimator(IdDodgeRight, false);
            }
            else if (playerPosition == PlayerPosition.Middle)
            {
                UpdatePlayerXPosition(PlayerPosition.Right);
                SetPlayerAnimator(IdDodgeRight, false);
            }
        }

        MovePlayer();
        Jump();
    }

    private void UpdatePlayerXPosition(PlayerPosition playerPos)
    {
        newXPosition = (int)playerPos;
        playerPosition = playerPos;
    }

    private void SetPlayerAnimator(int id, bool isCrossFade, float fadeFixedTime = 0.1f)
    {
        if (isCrossFade)
        {
            playerAnimator.CrossFadeInFixedTime(id, fadeFixedTime);
        }
        else
        {
            playerAnimator.Play(id);
        }
    }
    
    private void MovePlayer()
    {
        xPosition = Mathf.Lerp(xPosition, newXPosition, dodgeSpeed * Time.deltaTime);
        motionPlayer = new Vector3(xPosition - playerTransform.position.x, yPosition * Time.deltaTime, 0);
        characterController.Move(motionPlayer);
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                SetPlayerAnimator(IdLanding, false);

            if (swipeUp)
            {
                yPosition = jumpPower;
                SetPlayerAnimator(IdJump, true, 1f);
            }
        }
        else
        {
            yPosition -= jumpPower * 2 * Time.deltaTime;
            SetPlayerAnimator(IdFall, true);
        }
    }
}
