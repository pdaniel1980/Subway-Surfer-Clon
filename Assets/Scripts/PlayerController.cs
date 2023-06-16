using System;
using UnityEngine;

public enum Side { Left = -2, Middle = 0, Right = 2 }
public enum LayerAnimator { Base, StumbleCorner }

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private Side position;
    [SerializeField] private Side previuosPosition;
    private Transform selfTransform;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isJumping;
    private bool _isRolling;
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
    private int _IdStumbleLow = Animator.StringToHash("StumbleLow");
    private int _IdStumbleCornerLeft = Animator.StringToHash("StumbleCornerLeft");
    private int _IdStumbleCornerRight = Animator.StringToHash("StumbleCornerRight");
    private int _IdStumbleFall = Animator.StringToHash("StumbleFall");
    private int _IdStumbleOffLeft = Animator.StringToHash("StumbleOffLeft");
    private int _IdStumbleOffRight = Animator.StringToHash("StumbleOffRight");
    private int _IdStumbleSideLeft = Animator.StringToHash("StumbleSideLeft");
    private int _IdStumbleSideRight = Animator.StringToHash("StumbleSideRight");
    private int _IdDeathBounce = Animator.StringToHash("DeathBounce");
    private int _IdDeathLower = Animator.StringToHash("DeathLower");
    private int _IdDeathMovingTrain = Animator.StringToHash("DeathMovingTrain");
    private int _IdDeathUpper = Animator.StringToHash("DeathUpper");
    public int IdStumbleLow { get => _IdStumbleLow; set => _IdStumbleLow = value; }
    public int IdStumbleCornerLeft { get => _IdStumbleCornerLeft; set => _IdStumbleCornerLeft = value; }
    public int IdStumbleCornerRight { get => _IdStumbleCornerRight; set => _IdStumbleCornerRight = value; }
    public int IdStumbleFall { get => _IdStumbleFall; set => _IdStumbleFall = value; }
    public int IdStumbleOffLeft { get => _IdStumbleOffLeft; set => _IdStumbleOffLeft = value; }
    public int IdStumbleOffRight { get => _IdStumbleOffRight; set => _IdStumbleOffRight = value; }
    public int IdStumbleSideLeft { get => _IdStumbleSideLeft; set => _IdStumbleSideLeft = value; }
    public int IdStumbleSideRight { get => _IdStumbleSideRight; set => _IdStumbleSideRight = value; }
    public int IdDeathBounce { get => _IdDeathBounce; set => _IdDeathBounce = value; }
    public int IdDeathLower { get => _IdDeathLower; set => _IdDeathLower = value; }
    public int IdDeathMovingTrain { get => _IdDeathMovingTrain; set => _IdDeathMovingTrain = value; }
    public int IdDeathUpper { get => _IdDeathUpper; set => _IdDeathUpper = value; }

    private PlayerCollision playerCollision;
    private CharacterController _selfCharacterController;
    public CharacterController SelfCharacterController { get => _selfCharacterController; set => _selfCharacterController = value; }
    public bool IsRolling { get => _isRolling; set => _isRolling = value; }

    private Vector3 motion;

    private GameManager gameManager;

    private void Awake()
    {
        selfTransform = GetComponent<Transform>();
        selfAnimator = GetComponent<Animator>();
        _selfCharacterController = GetComponent<CharacterController>();
        playerCollision = GetComponent<PlayerCollision>();
    }

    void Start()
    {
        position = Side.Middle;
        yPosition = -7f;
        standCharacterCenter = _selfCharacterController.center;
        standCharacterHeight = _selfCharacterController.height;
        rollCharacterCenter = new Vector3(0, 0.2f, 0);
        rollCharacterHeight = 0.4f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void Update()
    {
        if (!gameManager.GameOver && gameManager.Go)
        {
            GetSwipe();
            SetPlayerPosition();
            MovePlayer();
            BlouncePlayer();
            Jump();
            Roll();
        }
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
        if (swipeLeft && !_isRolling)
        {
            previuosPosition = position;

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
        else if (swipeRight && !_isRolling)
        {
            previuosPosition = position;

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

    public void SetPlayerAnimator(int id, bool isCrossFade, float fadeFixedTime = 0.1f)
    {
        selfAnimator.SetLayerWeight((int) LayerAnimator.Base, 1);

        if (isCrossFade)
        {
            selfAnimator.CrossFadeInFixedTime(id, fadeFixedTime);
        }
        else
        {
            selfAnimator.Play(id);
        }

        ResetPlayerCollisions();
    }

    public void SetPlayerAnimatorWithLayer(int id)
    {
        selfAnimator.SetLayerWeight((int) LayerAnimator.StumbleCorner, 1);
        selfAnimator.Play(id);
        ResetPlayerCollisions();
    }

    private void ResetPlayerCollisions()
    {
        playerCollision.CollisionX = CollisionX.None;
        playerCollision.CollisionY = CollisionY.None;
        playerCollision.CollisionZ = CollisionZ.None;
    }

    private void MovePlayer()
    {
        xPosition = Mathf.Lerp(xPosition, newXPosition, dodgeSpeed * Time.deltaTime);
        motion = new Vector3(xPosition - selfTransform.position.x, yPosition * Time.deltaTime, forwardSpeed * Time.deltaTime);
        _selfCharacterController.Move(motion);
    }

    private void BlouncePlayer()
    {
        if (playerCollision.SideBounce)
        {
            UpdatePlayerXPosition(previuosPosition);
            MovePlayer();
            playerCollision.SideBounce = false;
        }
    }

    private void Jump()
    {
        if (_selfCharacterController.isGrounded)
        {
            isJumping = false;

            if (selfAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                SetPlayerAnimator(IdLanding, false);

            if (swipeUp && !_isRolling)
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
                _isRolling = false;
                rollTimer = 0;
                _selfCharacterController.center = standCharacterCenter;
                _selfCharacterController.height = standCharacterHeight;
            }

            if (swipeDown && !isJumping)
            {
                _isRolling = true;
                rollTimer = 0.5f;
                _selfCharacterController.center = rollCharacterCenter;
                _selfCharacterController.height = rollCharacterHeight;
                SetPlayerAnimator(IdRoll, true);
            }
        }
    }
}
