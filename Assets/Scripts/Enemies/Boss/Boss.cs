using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    private Animator animator;
    private Material AkechiMaterial;
    private Rigidbody rigidBody;

    public BoxCollider palmCollider;
    public CapsuleCollider tackleCollider;



    public float life;
    public float moveSpeed;


    public float defenseRange;
    public float palmDistance;
    public float tackleDistance;
    public float palmDamage;
    public float tackleDamage;

    public stateMachine actualState;                // Estado actual del enemigo
    public enum stateMachine
    {
        Approaching,
        AttackingTackle,
        AttackingPalm,
        Protecting,
        Defeated,
    }


    private Transform detectedPlayer;
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isPalming = false;
    private bool isDefending = false;

    private Vector3 chargeDirection;
    private float chargeTimer = 0f;
    public float chargeDuration = 1f;
    public float chargeSpeedMultiplier = 2f;
    public float postChargeCooldown = 0.75f;       // Tiempo de espera tras el placaje
    private float postChargeTimer = 0f;

    private float hitDelayTimer = 0f;
    private float delayBeforeStop = 0.01f;          // Retardo fijo para choque con jugador
    public float detectionDistance = 1.5f;          // Distancia fija de detección

    private bool isInvul = false;




    void Start()
    {
        AkechiMaterial = transform.GetChild(1).GetComponent<Renderer>().material;
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        actualState = stateMachine.Approaching;

        palmCollider.enabled = false;
        tackleCollider.enabled = false;
    }

    void Update()
    {
        if (!isAttacking && !isPalming && !isCharging && !isDefending)
        {
            CheckPlayerDistance();

            switch (actualState)
            {
                case stateMachine.Approaching:
                    if (UnityEngine.Random.value <= 0.1f)
                    {
                        actualState = stateMachine.Protecting;
                    }
                    else
                    {
                        Approach();
                    }
                    break;

                case stateMachine.AttackingTackle:
                    float randTackle = UnityEngine.Random.value;
                    if (randTackle <= 0.1f)
                    {
                        actualState = stateMachine.AttackingPalm;
                    }
                    else if (randTackle <= 0.05f)
                    {
                        actualState = stateMachine.Protecting;
                    }
                    else
                    {
                        AttackTackle();
                    }
                    break;

                case stateMachine.AttackingPalm:
                    if (UnityEngine.Random.value <= 0.1f)
                    {
                        actualState = stateMachine.AttackingTackle;
                    }
                    else
                    {
                        AttackPalm();
                    }
                    break;

                case stateMachine.Protecting:
                    if (UnityEngine.Random.value <= 0.2f)
                    {
                        actualState = stateMachine.AttackingTackle;
                    }
                    else
                    {
                        Protect();
                    }
                    break;

                case stateMachine.Defeated:
                    Defeat();
                    break;

                default:
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isCharging)
        {
            if (chargeTimer > 0f)
            {
                if (IsObstacleAhead())
                {
                    tackleCollider.enabled = false;
                    animator.SetTrigger("AttackTackleEnd");
                    chargeTimer = 0f;
                    postChargeTimer = postChargeCooldown;
                    return;
                }

                chargeTimer -= Time.fixedDeltaTime;
                Vector3 newPos = rigidBody.position + chargeDirection * moveSpeed * chargeSpeedMultiplier * Time.fixedDeltaTime;
                tackleCollider.enabled = true;
                rigidBody.MovePosition(newPos);

                if (chargeTimer <= 0f)
                {
                    tackleCollider.enabled = false;
                    animator.SetTrigger("AttackTackleEnd");
                    postChargeTimer = postChargeCooldown;
                }
            }
            else
            {
                tackleCollider.enabled = false;
                // Esperar el cooldown de la embestida
                if (postChargeTimer > 0f)
                {
                    postChargeTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    isCharging = false;
                    isAttacking = false;
                }
            }
        }

        if (isPalming)
        {
            bool isPalmAnimating = animator.GetCurrentAnimatorStateInfo(0).IsName("Palm");

            if (chargeTimer > 0f)
            {
                chargeTimer -= Time.fixedDeltaTime;
            }
            else
            {
                if (!isPalmAnimating)  // Solo continuar si no está en la animación Palm
                {
                    if (postChargeTimer > 0f)
                    {
                        postChargeTimer -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        isPalming = false;
                        isAttacking = false;
                    }
                }
            }
        }

        if (isDefending)
        {
            if (chargeTimer > 0f)
            {
                chargeTimer -= Time.fixedDeltaTime;
            }
            else
            {
                if (postChargeTimer > 0f)
                {
                    postChargeTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    animator.SetBool("IsDefending", false);
                    isDefending = false;
                    isAttacking = false;
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        origin.y += 0.5f;

        Physics.SphereCast(origin, palmDistance, transform.forward, out RaycastHit hit, palmDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, palmDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, tackleDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, defenseRange);

        Gizmos.color = Color.red;
        Vector3 lineEnd = origin + transform.forward * detectionDistance;
        Gizmos.DrawLine(origin, lineEnd);

    }

    private bool IsObstacleAhead()
    {
        Vector3 origin = rigidBody.position + Vector3.up * 0.5f;

        // Raycast para detectar obstáculos (paredes)
        if (Physics.Raycast(origin, chargeDirection, detectionDistance, obstacleLayer))
        {
            return true;
        }

        // Raycast para detectar jugador
        if (Physics.Raycast(origin, chargeDirection, detectionDistance, playerLayer))
        {
            if (hitDelayTimer <= 0f)
            {
                hitDelayTimer = delayBeforeStop;
            }
            else
            {
                hitDelayTimer -= Time.fixedDeltaTime;
                if (hitDelayTimer <= 0f)
                {
                    return true;
                }
            }
        }
        else
        {
            hitDelayTimer = 0f;
        }

        return false;
    }

    private void CheckPlayerDistance()
    {
        Vector3 origin = transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, defenseRange, playerLayer);

        if (hits.Length > 0)
        {
            Transform player = hits[0].transform;
            detectedPlayer = player;

            float distance = Vector3.Distance(origin, player.position);

            if (distance <= palmDistance)
            {
                actualState = stateMachine.AttackingPalm;
            }
            else if (distance <= tackleDistance)
            {
                actualState = stateMachine.AttackingTackle;
            }
            else
            {
                actualState = stateMachine.Protecting;
            }
        }
        else
        {
            actualState = stateMachine.Approaching;
            //detectedPlayer = null;
        }
    }

    //-----------------------------------------------------------------------------------

    private void Approach()
    {
        Debug.Log("Aproach");
        AttackTackle();
    }

    private void AttackTackle()
    {
        Debug.Log("Tackle");
        if (!isAttacking && detectedPlayer != null)
        {
            chargeDirection = (detectedPlayer.position - transform.position).normalized;
            chargeDirection = (chargeDirection.x > 0) ? Vector3.right : Vector3.left;
            chargeDirection.y = 0f;

            StartCoroutine(RotateTowardsPlayer(0.2f));
            animator.SetTrigger("AttackTackle");
            isCharging = true;
            isAttacking = true;

            chargeTimer = chargeDuration;
            postChargeTimer = postChargeCooldown;
        }
    }

    private void AttackPalm()
    {
        Debug.Log("Palm");
        if (!isAttacking && detectedPlayer != null)
        {
            chargeDirection = (detectedPlayer.position - transform.position).normalized;
            chargeDirection = (chargeDirection.x > 0) ? Vector3.right : Vector3.left;
            chargeDirection.y = 0f;

            StartCoroutine(RotateTowardsPlayer(0.2f));
            animator.SetTrigger("AttackPalm");

            isPalming = true;
            isAttacking = true;
            postChargeTimer = postChargeCooldown / 2;
        }
    }

    private void Protect()
    {
        Debug.Log("Defense");
        if (!isAttacking && !isDefending && detectedPlayer != null)
        {
            chargeDirection = (detectedPlayer.position - transform.position).normalized;
            chargeDirection = (chargeDirection.x > 0) ? Vector3.right : Vector3.left;
            chargeDirection.y = 0f;

            StartCoroutine(RotateTowardsPlayer(0.2f));
            animator.SetBool("IsDefending", true);

            isDefending = true;
            isAttacking = true;
            postChargeTimer = postChargeCooldown * UnityEngine.Random.Range(2f, 4f);
        }
    }

    private void Defeat()
    {
        //Lógica de defeat, caer muerto, pasar a la siguiente escena (cinematica final)
    }

    public void PalmHit()
    {
        palmCollider.enabled = true;
        Invoke(nameof(DisablePalmCollider), 0.1f);
    }

    private void DisablePalmCollider()
    {
        palmCollider.enabled = false;
    }

    public void UpdateLife(float damage, Vector3 BulletPosition)
    {
        if (isInvul) return;

        // Si está defendiendo, solo recibe daño si el ataque viene por la espalda
        if (isDefending)
        {
            Vector3 directionToBullet = (BulletPosition - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToBullet);

            if (angle < 60f)
            {
                Debug.Log("Ataque frontal");
                return;
            }
            else
            {
                animator.SetBool("IsDefending", false);
                isDefending = false;
                isAttacking = false;
            }
        }

        life += damage;
        if (!isAttacking && UnityEngine.Random.value <= 0.3f)
        {
            animator.SetTrigger("Damaged");
        }

        if (life <= 0)
        {
            //ragdoll, aplicar la logica aqui, ánimo gente
            TransitionCanvas.Instance.CallNextScene();
        }
        else
        {
            StartCoroutine(SwitchColor());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float damage = 0f;

            if (isCharging)
            {
                damage = tackleDamage;
            }
            else if (isPalming)
            {
                damage = palmDamage;
            }

            if (damage > 0f)
            {
                var playerLife = other.GetComponent<PlayerLife>();
                if (playerLife != null)
                {
                    playerLife.ChangeCurrentLife(-damage);
                }
            }
        }
    }

    IEnumerator SwitchColor()
    {
        isInvul = true;

        for (int i = 0; i < 3; i++)
        {
            AkechiMaterial.color = new Color(203f / 255f, 20f / 255f, 20f / 255f, 1f);
            yield return new WaitForSeconds(0.05f);
            AkechiMaterial.color = Color.white;
            yield return new WaitForSeconds(0.05f);
        }

        isInvul = false;
    }

    private IEnumerator RotateTowardsPlayer(float duration)
    {
        if (detectedPlayer == null || isDefending)
            yield break;

        Quaternion startRotation = transform.rotation;

        Vector3 direction = detectedPlayer.position - transform.position;
        direction.y = 0f;

        if (direction == Vector3.zero)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }
    }

}
