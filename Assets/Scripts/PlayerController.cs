using System.Collections;
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

    // Variable de movimientos
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isJumping;
    private bool _isRolling;
    public bool IsRolling { get => _isRolling; }
    private float rollTimer;
    private Vector3 motion;

    // Variable de posicion del player
    private float xPosition, newXPosition;
    private float yPosition;
    private Vector3 standCharacterCenter;
    private float standCharacterHeight;
    private Vector3 rollCharacterCenter;
    private float rollCharacterHeight;

    // Animaciones
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
    public int IdStumbleLow { get => _IdStumbleLow; }
    public int IdStumbleCornerLeft { get => _IdStumbleCornerLeft; }
    public int IdStumbleCornerRight { get => _IdStumbleCornerRight; }
    public int IdStumbleFall { get => _IdStumbleFall; }
    public int IdStumbleOffLeft { get => _IdStumbleOffLeft; }
    public int IdStumbleOffRight { get => _IdStumbleOffRight; }
    public int IdStumbleSideLeft { get => _IdStumbleSideLeft; }
    public int IdStumbleSideRight { get => _IdStumbleSideRight; }
    public int IdDeathBounce { get => _IdDeathBounce; }
    public int IdDeathLower { get => _IdDeathLower; }
    public int IdDeathMovingTrain { get => _IdDeathMovingTrain; }
    public int IdDeathUpper { get => _IdDeathUpper; }

    private PlayerCollision playerCollision;
    private CharacterController selfCharacterController;

    private GameManager _gameManager;
    public GameManager GameManager { get => _gameManager; }

    private void Awake()
    {
        selfTransform = GetComponent<Transform>();
        selfAnimator = GetComponent<Animator>();
        selfCharacterController = GetComponent<CharacterController>();
        playerCollision = GetComponent<PlayerCollision>();
    }

    void Start()
    {
        position = Side.Middle;
        // Fix: para asegurarnos de no tener la animacion de falling al inicio del juego 
        yPosition = -7f;
        standCharacterCenter = selfCharacterController.center;
        standCharacterHeight = selfCharacterController.height;
        rollCharacterCenter = new Vector3(0, 0.2f, 0);
        rollCharacterHeight = 0.4f;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void Update()
    {
        if (!_gameManager.GameOver && _gameManager.Go)
        {
            GetSwipe();
            SetPlayerPosition();
            MovePlayer();
            BlouncePlayer();
            Jump();
            Roll();
        }
    }

    // Cuando el player muere
    public void Die()
    {
        _ = StartCoroutine(FallRoutine());
    }

    // Corrutina que verifica si el player esta en el aire para que caiga hasta el piso
    private IEnumerator FallRoutine()
    {
        while (!selfCharacterController.isGrounded)
        {
            Fall();
            MovePlayer();
            yield return null;
        }

        yield return null;

        StopAnimations();
        selfCharacterController.enabled = false;
    }

    // Paramos todas las animaciones
    public void StopAnimations()
    {
        selfAnimator.StopPlayback();
    }

    // Para activar o desactivar las animaciones
    public void SetActiveAnimator(bool activate)
    {
        selfAnimator.enabled = activate;
    }

    // Obtenemos la direccion del comando
    private void GetSwipe()
    {
        swipeLeft = Input.GetKeyDown(KeyCode.LeftArrow);
        swipeRight = Input.GetKeyDown(KeyCode.RightArrow);
        swipeUp = Input.GetKeyDown(KeyCode.UpArrow);
        swipeDown = Input.GetKeyDown(KeyCode.DownArrow);
    }

    // Establecemos la posicion del player para el movimiento
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
        if (selfCharacterController.enabled)
        {
            xPosition = Mathf.Lerp(xPosition, newXPosition, dodgeSpeed * Time.deltaTime);
            motion = new Vector3(xPosition - selfTransform.position.x, yPosition * Time.deltaTime, forwardSpeed * Time.deltaTime);
            selfCharacterController.Move(motion);
        }
    }

    // En caso de colision al costado del objeto, rebota el player a la posicion anterior
    private void BlouncePlayer()
    {
        if (playerCollision.SideCollision)
        {
            _ = StartCoroutine(WaitToBackPosition(GetTimeToBack()));
        }
    }

    // Corrutina que espera el tiempo de duracion de colision de la animacion para retornar al player a la posicion anterior
    IEnumerator WaitToBackPosition(float timeToBack)
    {
        yield return new WaitForSeconds(timeToBack);

        UpdatePlayerXPosition(previuosPosition);
        MovePlayer();

        playerCollision.SideCollision = false;
    }

    // Obtenemos la duracion de la animacion que se esta ejecutando
    private float GetTimeToBack()
    {
        AnimatorStateInfo stateInfo = selfAnimator.GetCurrentAnimatorStateInfo((int)LayerAnimator.Base);

        // Si la colision es al costado de un tren en movimiento, el tiempo de espera para rebotar el player es 0, caso contrario el tiempo de la animacion
        return playerCollision.ColliderTag.Equals("MovingTrain") ? 0 : stateInfo.length;
    }

    private void Jump()
    {
        if (selfCharacterController.isGrounded)
        {
            isJumping = false;

            if (selfAnimator.GetCurrentAnimatorStateInfo((int) LayerAnimator.Base).IsName("Fall"))
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
            Fall();
        }
    }

    private void Fall()
    {
        yPosition -= jumpPower * 2 * Time.deltaTime;

        // En caso que el juego este en curso llamamos a la animacion de caida
        if (selfCharacterController.velocity.y <= 0 && !_gameManager.GameOver)
            SetPlayerAnimator(IdFall, true);
    }

    private void Roll()
    {
        if (selfCharacterController.isGrounded)
        {
            rollTimer -= Time.deltaTime;

            if (rollTimer <= 0)
            {
                _isRolling = false;
                rollTimer = 0;
                selfCharacterController.center = standCharacterCenter;
                selfCharacterController.height = standCharacterHeight;
            }

            if (swipeDown && !isJumping)
            {
                _isRolling = true;
                rollTimer = 0.5f;
                selfCharacterController.center = rollCharacterCenter;
                selfCharacterController.height = rollCharacterHeight;
                SetPlayerAnimator(IdRoll, true);
            }
        }
    }
}
