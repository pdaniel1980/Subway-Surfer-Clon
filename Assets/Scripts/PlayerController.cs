using UnityEngine;

public enum PlayerPosition { Left = -2, Middle = 0, Right = 2 }

public class PlayerController : MonoBehaviour
{
    private PlayerPosition playerPosition;
    private Transform playerTransform;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isJumping, isRolling;
    private float newXPosition;
    private float xPosition;
    private float yPosition;
    private Vector3 standCharacterCenter;
    private Vector3 rollCharacterCenter;
    private float standCharacterHeight;
    private float rollCharacterHeight;

    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float jumpPower = 10f;

    private Animator playerAnimator;
    private int IdDodgeLeft = Animator.StringToHash("DodgeLeft");
    private int IdDodgeRight = Animator.StringToHash("DodgeRight");
    private int IdJump = Animator.StringToHash("Jump");
    private int IdFall = Animator.StringToHash("Fall");
    private int IdLanding = Animator.StringToHash("Landing");
    private int IdRoll = Animator.StringToHash("Roll");

    private CharacterController characterController;
    private Vector3 motionPlayer;
    
    
    private float rollTimer;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        playerPosition = PlayerPosition.Middle;
        yPosition = -7f;
        standCharacterCenter = characterController.center;
        standCharacterHeight = characterController.height;
        rollCharacterCenter = new Vector3(0, 0.2f, 0);
        rollCharacterHeight = 0.4f;
    }
    
    void Update()
    {
        GetSwipe();
        SetPlayerPosition();
        MovePlayer();
        Jump();
        Roll();
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
        if (swipeLeft && !isRolling)
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
        else if (swipeRight && !isRolling)
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
        motionPlayer = new Vector3(xPosition - playerTransform.position.x, yPosition * Time.deltaTime, forwardSpeed * Time.deltaTime);
        characterController.Move(motionPlayer);
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            isJumping = false;

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                SetPlayerAnimator(IdLanding, false);

            if (swipeUp)
            {
                isJumping = true;
                yPosition = jumpPower;
                SetPlayerAnimator(IdJump, true);
            }
        }
        else
        {
            yPosition -= jumpPower * 2 * Time.deltaTime;
            if (characterController.velocity.y <= 0)
                SetPlayerAnimator(IdFall, true);
        }
    }

    private void Roll()
    {
        if (characterController.isGrounded)
        {
            rollTimer -= Time.deltaTime;

            if (rollTimer <= 0)
            {
                isRolling = false;
                rollTimer = 0;
                characterController.center = standCharacterCenter;
                characterController.height = standCharacterHeight;
            }

            if (swipeDown)
            {
                isRolling = true;
                rollTimer = 0.5f;
                characterController.center = rollCharacterCenter;
                characterController.height = rollCharacterHeight;
                SetPlayerAnimator(IdRoll, true);
            }
        }
    }
}
