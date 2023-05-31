using UnityEngine;

public enum Side { Left = -2, Middle = 0, Right = 2 }

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float jumpPower = 10f;
    private Side position;
    private Transform selfTransform;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isJumping;
    private bool isRolling;
    private float rollTimer;
    private float newXPosition;
    private float xPosition;
    private float yPosition;
    private Vector3 standCharacterCenter;
    private Vector3 rollCharacterCenter;
    private float standCharacterHeight;
    private float rollCharacterHeight;

    private Animator selfAnimator;
    private int IdDodgeLeft = Animator.StringToHash("DodgeLeft");
    private int IdDodgeRight = Animator.StringToHash("DodgeRight");
    private int IdJump = Animator.StringToHash("Jump");
    private int IdFall = Animator.StringToHash("Fall");
    private int IdLanding = Animator.StringToHash("Landing");
    private int IdRoll = Animator.StringToHash("Roll");

    private CharacterController _selfCharacterController;
    public CharacterController SelfCharacterController { get => _selfCharacterController; set => _selfCharacterController = value; }
    private Vector3 motion;

    private void Awake()
    {
        selfTransform = GetComponent<Transform>();
        selfAnimator = GetComponent<Animator>();
        _selfCharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        position = Side.Middle;
        yPosition = -7f;
        standCharacterCenter = _selfCharacterController.center;
        standCharacterHeight = _selfCharacterController.height;
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
            if (position == Side.Middle)
            {
                UpdatePlayerXPosition(Side.Left);
                SetPlayerAnimator(IdDodgeLeft, false);
            }
            else if (position == Side.Right)
            {
                UpdatePlayerXPosition(Side.Middle);
                SetPlayerAnimator(IdDodgeLeft, false);
            }
        }
        else if (swipeRight && !isRolling)
        {
            if (position == Side.Left)
            {
                UpdatePlayerXPosition(Side.Middle);
                SetPlayerAnimator(IdDodgeRight, false);
            }
            else if (position == Side.Middle)
            {
                UpdatePlayerXPosition(Side.Right);
                SetPlayerAnimator(IdDodgeRight, false);
            }
        }
    }

    private void UpdatePlayerXPosition(Side playerPos)
    {
        newXPosition = (int)playerPos;
        position = playerPos;
    }

    private void SetPlayerAnimator(int id, bool isCrossFade, float fadeFixedTime = 0.1f)
    {
        if (isCrossFade)
        {
            selfAnimator.CrossFadeInFixedTime(id, fadeFixedTime);
        }
        else
        {
            selfAnimator.Play(id);
        }
    }
    
    private void MovePlayer()
    {
        xPosition = Mathf.Lerp(xPosition, newXPosition, dodgeSpeed * Time.deltaTime);
        motion = new Vector3(xPosition - selfTransform.position.x, yPosition * Time.deltaTime, forwardSpeed * Time.deltaTime);
        _selfCharacterController.Move(motion);
    }

    private void Jump()
    {
        if (_selfCharacterController.isGrounded)
        {
            isJumping = false;

            if (selfAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                SetPlayerAnimator(IdLanding, false);

            if (swipeUp && !isRolling)
            {
                isJumping = true;
                yPosition = jumpPower;
                SetPlayerAnimator(IdJump, true);
            }
        }
        else
        {
            yPosition -= jumpPower * 2 * Time.deltaTime;
            if (_selfCharacterController.velocity.y <= 0)
                SetPlayerAnimator(IdFall, true);
        }
    }

    private void Roll()
    {
        if (_selfCharacterController.isGrounded)
        {
            rollTimer -= Time.deltaTime;

            if (rollTimer <= 0)
            {
                isRolling = false;
                rollTimer = 0;
                _selfCharacterController.center = standCharacterCenter;
                _selfCharacterController.height = standCharacterHeight;
            }

            if (swipeDown && !isJumping)
            {
                isRolling = true;
                rollTimer = 0.5f;
                _selfCharacterController.center = rollCharacterCenter;
                _selfCharacterController.height = rollCharacterHeight;
                SetPlayerAnimator(IdRoll, true);
            }
        }
    }
}
