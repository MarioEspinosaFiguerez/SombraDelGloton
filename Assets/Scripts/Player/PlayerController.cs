using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Walk, Dash, Jump, Attack, WallJump }
    [SerializeField][Header("Testing")] private PlayerState currentState;
    private ShootController shootController;
    private CharacterController characterController;
    private Animator animator;

    private bool IsPlayerGrounded => characterController.isGrounded || isOnTreadmill;

    private float dashTimer = 0;
    private bool hasDashed = false;

    private float? lastGrounded, jumpButtonPressTime;

    private float lastShootTime, magnitude, baseGravityMultiplier;

    private Vector3 moveDirection, velocity, basePosition;
    private bool isWallJumping, shouldInstantRotate;

    public GameObject slingshot;

    [Header("Gravity")]
    public float gravityMultiplier;

    [Header("Basics")]
    public float rotation;
    public float speed;
    public float jumpSpeed;
    public float ySpeed;

    [Header("Jump")]
    public float jumpButtonPeriod;
    public float jumpApexValue;
    public bool hasReachedApex = false;

    [Header("Dash")]
    public KeyCode dashKey = KeyCode.Z;
    public KeyCode dashKeyJoystick = KeyCode.JoystickButton2;
    public float dashCooldown;
    public float accelerationDash;

    [Header("WallJump")]
    public float wallJumpHorizontalForce;
    public float wallJumpVerticalForce;
    public float distanceWallJumpLine = 1.0f;
    public LayerMask ableToWallJumpLayer;
    public bool hasWallJumped = false;

    [Header("WallSlide")]
    [Min(0)]
    public float wallSlideSpeed;

    [Header("Shoot")]
    public float timeBetweenShoots;

    [Header("Others")]
    public bool goRight;
    public bool isGrounded;
    public bool isJumping;
    public bool wasGrounded;
    public bool isShooting;
    public bool isOnTreadmill;
    public bool isHitted;
    public bool isSlingshotActive;

    [Header("Particle Effects")]
    public GameObject dashParticles;
    public GameObject jumpParticles;
    public GameObject landParticles;
    public GameObject hitParticles;

    [Header("OnDrawGizmos")]
    public float distanceWallJumpLineGizmos;

    [Header("Testing - Debugging slow motion")]
    [SerializeField] private bool debugActive;
    [SerializeField][Range(0, 1)] private float debugTimeScale;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        shootController = GetComponent<ShootController>();
        animator = GetComponent<Animator>();

        lastShootTime = Time.time - timeBetweenShoots;
        goRight = true;
        isSlingshotActive = slingshot.activeInHierarchy;

        basePosition.z = transform.position.z;
        baseGravityMultiplier = gravityMultiplier;

        ChangeState(PlayerState.Idle);
    }

    void Update()
    {
        if (debugActive) Time.timeScale = debugTimeScale;
        else Time.timeScale = 1f;

        wasGrounded = isGrounded;
        isGrounded = IsPlayerGrounded;
        isJumping = !isGrounded;

        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal is not 0 && !isWallJumping) goRight = IsGoingRight(horizontal);

        if (!wasGrounded && isGrounded)
        {
            SpawnParticles(landParticles);
        }
        switch (currentState)
        {
            case PlayerState.Idle: HandleIdleState(horizontal); break;
            case PlayerState.Walk: HandleWalkState(horizontal); break;
            case PlayerState.Dash: HandleDashState(horizontal); break;
            case PlayerState.Jump: HandleJumpState(horizontal); break;
            case PlayerState.WallJump: HandleWallJumpState(horizontal); break;
            case PlayerState.Attack: HandleAttackState(horizontal); break;
        }

        HandleMovement(characterController, moveDirection, speed, magnitude);
        ApplyGravity();
        HandleAnimations();
    }

    private void LateUpdate()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.z = basePosition.z;
        transform.position = currentPosition;

        if (isSlingshotActive) slingshot.SetActive(true);
        else slingshot.SetActive(false);

        HandleRotation();
    }

    #region States Handlers

    private void HandleIdleState(float horizontal)
    {
        isShooting = false;

        if (horizontal is not 0) ChangeState(PlayerState.Walk);
        if (CheckIfCanWallJump())
        {
            ChangeState(PlayerState.WallJump);
            return;
        }
        if (CheckIfCanJump()) ChangeState(PlayerState.Jump);
        if (Input.GetButtonDown("Fire1") && isSlingshotActive) ChangeState(PlayerState.Attack);

    }

    private void HandleWalkState(float horizontal)
    {
        isShooting = false;
        isWallJumping = false;
        hasWallJumped = false;
        HandleInputMovement(horizontal);

        if (horizontal is 0) ChangeState(PlayerState.Idle);
        if (CheckIfCanWallJump())
        {
            ChangeState(PlayerState.WallJump);
            return;
        }
        if (CheckIfCanJump()) ChangeState(PlayerState.Jump);
        if (Input.GetButtonDown("Fire1") && !hasDashed && isSlingshotActive) ChangeState(PlayerState.Attack);
        if (Input.GetKeyDown(dashKey) || Input.GetKeyDown(dashKeyJoystick) && horizontal is not 0 && isGrounded) ChangeState(PlayerState.Dash);
    }

    private void HandleDashState(float horizontal)
    {
        HandleDash(characterController, ref magnitude);
    }

    private void HandleJumpState(float horizontal)
    {
        isShooting = false;
        isWallJumping = false;
        hasWallJumped = false;
        HandleInputMovement(horizontal);

        HandleJump(horizontal);

        if (CheckIfCanWallJump()) ChangeState(PlayerState.WallJump);
        if (!isJumping && horizontal is 0) ChangeState(PlayerState.Idle);
        if (!isJumping && horizontal is not 0) ChangeState(PlayerState.Walk);
        if (Input.GetButtonDown("Fire1") && isSlingshotActive) ChangeState(PlayerState.Attack);
    }

    private void HandleWallJumpState(float horizontal)
    {
        isShooting = false;

        HandleWallJump(horizontal);
    }

    private void HandleAttackState(float horizontal)
    {
        HandleShoot(goRight);

        if (isJumping) ChangeState(PlayerState.Jump);
        if (isGrounded && horizontal is 0) ChangeState(PlayerState.Idle);
        else if (isGrounded && horizontal is not 0) ChangeState(PlayerState.Walk);
    }

    #endregion 

    #region Basic Functions

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            if (ySpeed < 0.1f)
            {
                ySpeed = -0.5f;
            }

            gravityMultiplier = 1f;
        }
        else
        {
            if (ShouldWallSlide())
            {
                ySpeed = Mathf.Max(ySpeed, -wallSlideSpeed);
            }

            if (ySpeed >= jumpApexValue)
            {
                gravityMultiplier = 1f;
            }
            else
            {
                gravityMultiplier = baseGravityMultiplier;
            }

            ySpeed += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        velocity = moveDirection * magnitude;
        velocity.y = ySpeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleInputMovement(float horizontalInput)
    {
        if (isWallJumping) return;

        moveDirection = new Vector3(horizontalInput, 0, 0);
        moveDirection.Normalize();
        magnitude = GetInputMagnitude(moveDirection);

        /*Debug.Log($"HandleInputMovement - horizontalInput: {horizontalInput}");
        Debug.Log($"HandleInput - MoveDirection: {moveDirection}");
        Debug.Log($"HandleInput - Magnitude: {magnitude}"); */
    }

    private void HandleMovement(CharacterController characController, Vector3 moveDirection, float speed, float magnitude)
    {
        if (isWallJumping) return;

        characController.Move(moveDirection * speed * Time.deltaTime * magnitude);
    }

    private void HandleDash(CharacterController characController, ref float magnitude)
    {
        if (dashTimer <= 0 && !hasDashed)
        {
            dashTimer = dashCooldown;
            hasDashed = true;

            magnitude *= accelerationDash;
            SpawnParticles(dashParticles);
        }

        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            magnitude = Mathf.Lerp(magnitude, magnitude * accelerationDash, Time.deltaTime * dashCooldown);
        }
        else if (hasDashed)
        {
            hasDashed = false;
            magnitude = 1f;

            ChangeState(PlayerState.Walk);
        }
    }

    private void HandleJump(float horizontal)
    {
        if (Time.time - lastGrounded <= 0.1f)
        {
            if (Time.time - jumpButtonPressTime <= jumpButtonPeriod)
            {
                ySpeed = jumpSpeed;
                jumpButtonPressTime = null;
                lastGrounded = null;
                hasWallJumped = false;
                isJumping = true;

                SpawnParticles(jumpParticles);
            }
        }
    }

    private void HandleWallJump(float horizontal)
    {
        if (isWallJumping) return;

        Vector3 wallDirection = GetWallDirection();

        if (wallDirection != Vector3.zero)
            StartCoroutine(WallJumpMovement(wallDirection));
    }

    private IEnumerator WallJumpMovement(Vector3 wallDirection)
    {
        if (wallDirection == Vector3.zero) yield break;

        isWallJumping = true;
        shouldInstantRotate = true;

        float wallJumpDuration = 1f;
        float timer = 0f;

        Vector3 smoothVelocity = Vector3.zero;
        float smoothTime = 0.3f;

        float horizontalDirection = -wallDirection.x > 0 ? 1 : -1;
        Vector3 targetHorizontalVelocity = new Vector3(horizontalDirection * wallJumpHorizontalForce, 0, 0);

        ySpeed = wallJumpVerticalForce;


        while (timer < wallJumpDuration)
        {
            timer += Time.deltaTime;

            Vector3 smoothMovement = Vector3.SmoothDamp(Vector3.zero, targetHorizontalVelocity, ref smoothVelocity, smoothTime);
            goRight = horizontalDirection > 0;

            characterController.Move(smoothMovement * Time.deltaTime);

            yield return null;
        }

        if (isGrounded) ChangeState(PlayerState.Idle);
        else ChangeState(PlayerState.Jump);

        isWallJumping = false;
        hasWallJumped = true;
        moveDirection = Vector3.zero;
    }

    private void HandleShoot(bool goRight)
    {
        if (Time.time > timeBetweenShoots + lastShootTime)
        {
            isShooting = true;
            lastShootTime = Time.time;
            shootController.Shoot(goRight);
        }
    }

    private void HandleRotation(bool instantRotation = false)
    {
        if (isWallJumping && !shouldInstantRotate) return;

        Quaternion toRotation = Quaternion.Euler(0, goRight ? 90 : -90, 0);
        transform.rotation = instantRotation || shouldInstantRotate ? toRotation : Quaternion.RotateTowards(transform.rotation, toRotation, rotation * Time.deltaTime);

        shouldInstantRotate = false;
    }

    #endregion

    #region Animations Functions

    private void HandleAnimations()
    {
        if (isGrounded)
        {
            HandleDefaultGroundedParameters();
            HandleBasicMovementAnimations();
            HandleDashAnimation();

            HandleAttackAnimations();
            HandleHittedAnimations();
        }
        else
        {
            HandleDefaultJumpParameters();

            HandleJumpAnimations();
            HandleAttackAnimations("attackAir");
            HandleHittedAnimations("hittedAir");
        }
    }

    private void HandleDefaultGroundedParameters()
    {
        animator.SetFloat("isGrounded", 1.0f);
        animator.SetFloat("isJumping", 0.0f);
        animator.SetFloat("isOnWall", 0.0f);
    }
    private void HandleDefaultJumpParameters()
    {
        animator.SetFloat("isJumping", 1.0f);
        animator.SetFloat("isGrounded", 0.0f);
    }
    private void HandleBasicMovementAnimations() => animator.SetFloat("inputMovement", magnitude);
    private void HandleDashAnimation()
    {
        if (currentState == PlayerState.Dash)
        {
            animator.SetFloat("isDashing", 1.0f);
        }
        else
        {
            animator.SetFloat("isDashing", 0.0f);
        }
    }
    private void HandleJumpAnimations()
    {
        if (HasReachedApex())
            animator.SetFloat("hasReachedApex", 1.0f);
        else
            animator.SetFloat("hasReachedApex", -1.0f);
    }
    private void HandleHittedAnimations(string mode = "hittedGround")
    {
        if (isHitted)
        {
            SpawnParticles(hitParticles);

            if (mode == "hittedAir")
            {
                animator.SetTrigger("hittedAir");
                isHitted = false;
                return;
            }

            animator.SetTrigger("hittedGround");
            isHitted = false;
        }
    }
    private void HandleAttackAnimations(string mode = "attackGround")
    {
        if (currentState == PlayerState.Attack && !hasDashed && isSlingshotActive)
        {
            animator.SetFloat("isJumping", -1f);

            // Ativating Upper Layer (Base one aswell just in case)
            animator.SetLayerWeight(0, 1f);
            animator.SetLayerWeight(1, 1f);

            if (mode == "attackAir")
            {
                animator.SetTrigger("attackAir");
                return;
            }

            animator.SetTrigger("attackGround");

            StartCoroutine(ResetLayerAfterAttack());
        }
    }
    private IEnumerator ResetLayerAfterAttack()
    {
        yield return new WaitForSeconds(0.25f);
        animator.SetLayerWeight(1, 0f);
    }

    #endregion

    #region Support FSM Functions

    private bool CheckIfCanJump()
    {
        if (isGrounded)
        {
            hasWallJumped = false;
            lastGrounded = Time.time;
            if (Input.GetButtonDown("Jump"))
            {
                jumpButtonPressTime = Time.time;
                return true;
            }
        }

        return false;
    }
    private bool CheckIfCanWallJump()
    {
        return !isGrounded && !hasWallJumped && CheckIfCollisionWithWalls() && Input.GetButtonDown("Jump") && currentState != PlayerState.Dash && currentState != PlayerState.Attack;
    }
    private Vector3 GetWallDirection()
    {
        Vector3 origin = transform.position;
        RaycastHit hit;

        if (Physics.Raycast(origin, transform.forward, out hit, distanceWallJumpLine)) return transform.forward;
        if (Physics.Raycast(origin, -transform.forward, out hit, distanceWallJumpLine)) return -transform.forward;

        return Vector3.zero;
    }
    private bool CheckIfCollisionWithWalls()
    {
        Vector3 origin = transform.position;
        return Physics.Raycast(origin, transform.forward, distanceWallJumpLine, ableToWallJumpLayer) || Physics.Raycast(origin, -transform.forward, distanceWallJumpLine, ableToWallJumpLayer);
    }
    private void ChangeState(PlayerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    #endregion

    #region Others / Utils

    public CharacterController GetCharacterController() => characterController;
    private bool IsGoingRight(float horizontal) => horizontal > 0;
    private float GetInputMagnitude(Vector3 moveDirection) => Mathf.Clamp01(moveDirection.magnitude);
    private bool HasReachedApex() => velocity.y >= jumpApexValue;
    private bool ShouldWallSlide() => CheckIfCollisionWithWalls() && ySpeed < 0 && !isWallJumping;

    #endregion

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector3 origin = transform.position;

        origin.y += 0.5f;
        Debug.DrawRay(origin, transform.forward * distanceWallJumpLineGizmos, Color.black);
        Debug.DrawRay(origin, -transform.forward * distanceWallJumpLineGizmos, Color.magenta);
    }

    private void SpawnParticles(GameObject particlePrefab)
    {
        if (particlePrefab == null) return;

        GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        Destroy(particles, 2f);
    }



    public float knockbackXForce = 5.0f;
    public float knockbackYForce = 5.0f;
    public float knockbackDuration = 0.5f;

    public IEnumerator ApplyKnockback()
    {
        float elapsedTimeCoroutineStarted = 0;
        //Vector3 knockback = new Vector3(facingRight ? -knockbackXForce : knockbackXForce, knockbackYForce, 0);
        Vector3 knockback = (-transform.forward * knockbackXForce) + new Vector3(0, knockbackYForce, 0);

        while (elapsedTimeCoroutineStarted < knockbackDuration)
        {
            // Progress = 0 -> Start of Coroutine / Progress = 1 -> End of Coroutine
            float progress = elapsedTimeCoroutineStarted / knockbackDuration;
            Vector3 currentKnockback = Vector3.Lerp(knockback, Vector3.zero, progress);

            characterController.Move(currentKnockback * Time.deltaTime);
            currentKnockback.y = Physics.gravity.y * Time.deltaTime;

            elapsedTimeCoroutineStarted += Time.deltaTime;
            yield return null;
        }

        // The Knockback stopped - To be sure the player stop getting force to continue moving
        characterController.Move(Vector3.zero);
    }

}
