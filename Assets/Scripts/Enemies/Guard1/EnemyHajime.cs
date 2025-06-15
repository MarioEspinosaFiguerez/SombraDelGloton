using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;


public class EnemyHajime : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform player;
    public LayerMask isGround, isPlayer;

    public Vector3 walkStep;
    bool walkStepSet;
    public float walkStepRange;

    public float timeBetweenAttack;
    bool alreadyAttack;

    public float visionRange, attackRange;
    public bool playerInVisionRange, playerInAttackRange;

    public Animator animator;
    public Collider swordCollider;

    public Transform destinationA, destinationB;
    private Vector3 pointA, pointB, toDestination;
    public float rotationSpeed = 180f;
    private Quaternion targetRotation;
    private bool isTurning, isReAttack = false;
    public float live;
    private Material materialNpc;
    private RagdollActivator ragdollActivator;

    private bool isDead = false;
    private float fixedZ;


    public GameObject completeKatana;
    public GameObject sheath;
    public GameObject seatheKatana;
    public GameObject attackKatana;

    public AudioClip attackSound; 
    private AudioSource audioSource; 

    private void Awake()
    {
        player = GameObject.Find("Tanuki_Animations").transform;
        agent = GetComponent<NavMeshAgent>();
        pointA = new Vector3(destinationA.position.x, destinationA.position.y, destinationA.position.z);
        pointB = new Vector3(destinationB.position.x, destinationB.position.y, destinationB.position.z);

        materialNpc = transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material;

        fixedZ = transform.position.z;

        animator.SetBool("Walk", true);
        toDestination = pointB;
        agent.SetDestination(toDestination);

        ragdollActivator = GetComponent<RagdollActivator>();
    
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isDead)
        {
            playerInVisionRange = Physics.CheckSphere(transform.position, visionRange, isPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

            if (alreadyAttack && !playerInAttackRange)
            {
                animator.SetBool("isBetweenAttack", false);
                animator.SetBool("Sheathe", true);
                animator.SetBool("Attack", false);
                alreadyAttack = false;
                isReAttack = false;

                Invoke(nameof(EndSheathe), 0.4f);
            }
            if (!playerInVisionRange && !playerInAttackRange) Patrol();
            if (playerInVisionRange && !playerInAttackRange) FollowPlayer();
            if (playerInVisionRange && playerInAttackRange) AttackPlayer();

            if (isTurning)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    transform.rotation = targetRotation;
                    isTurning = false;
                }
            }

            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ActivateRagdoll();
            }
        }
        
    }

    private void ActivateRagdoll()
    {
        isDead = true;
        if (animator != null)
        {
            animator.Update(0f);
            animator.enabled = false;
        }

        if (agent != null) agent.enabled = false;

        if (ragdollActivator != null) ragdollActivator.ActivateRagdoll();
    }

    private void Patrol()
    {  
        bool isDestinationOk = (Mathf.Abs(agent.destination.x - pointA.x) < 0.1f) || (Mathf.Abs(agent.destination.x - pointB.x) < 0.1f);

        if (agent.remainingDistance <= agent.stoppingDistance || !isDestinationOk)
        {
            Vector3 pos = transform.position;
            float distToA = Vector3.Distance(pos, pointA);
            float distToB = Vector3.Distance(pos, pointB);
            toDestination = (distToA > distToB) ? pointA : pointB;

            Vector3 newDestination = new Vector3(toDestination.x, transform.position.y, transform.position.z);

            Turn();

            agent.SetDestination(newDestination);

            ChangeVisibilityKatanaToPatrol();
        }

        animator.SetBool("Walk", true);
    }

    private void FollowPlayer()
    {
        agent.isStopped = false;
        animator.SetBool("Walk", true);
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, fixedZ);
        agent.SetDestination(targetPosition);

        Vector3 correctedPosition = new Vector3(transform.position.x, transform.position.y, fixedZ);
        transform.position = correctedPosition;

        if (player.position.x - transform.position.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (player.position.x - transform.position.x < 0) transform.rotation = Quaternion.Euler(0, 180, 0);
    }


    private void AttackPlayer()
    {
        animator.SetBool("Walk", false);
        //agent.SetDestination(transform.position);
        agent.isStopped = true;

        if (player.position.x - transform.position.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (player.position.x - transform.position.x < 0) transform.rotation = Quaternion.Euler(0, 180, 0);

        if (!alreadyAttack)
        {
            if (!isReAttack)
            {
                animator.SetBool("Unsheathe", true);
                Invoke(nameof(ChangeVisibilityKatanaToUnsheathe), 0.65f);
            }

            alreadyAttack = true;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Unsheathe") && stateInfo.normalizedTime >= 1f)
        {
            animator.SetBool("Unsheathe", false);

            ChangeVisibilityKatanaToAttack();

            animator.SetBool("Attack", true);

            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound); 
            }
        }
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1f)
        {
            animator.SetBool("isBetweenAttack", true);
            Invoke(nameof(ResetAttack), timeBetweenAttack);
        }
    }

    private void ResetAttack()
    {
        animator.SetBool("isBetweenAttack", false);
        alreadyAttack = false;
        isReAttack = true;
        agent.isStopped = false;
    }
    private void EndSheathe()
    {
        animator.SetBool("Sheathe", false);
    }
    public void UpdateLife(float damage)
    {
        agent.SetDestination(transform.position);
        Debug.Log("UPDATE LIFE: " + live);
        live += damage;

        if (live <= 0) ActivateRagdoll();
        else StartCoroutine("SwitchColor");
    }
    IEnumerator SwitchColor()
    {
        /*materialNpc.color = new Color(203, 20, 20, 1);*/
        for (int i = 0; i < 6; i++)
        {
            agent.SetDestination(transform.position);
            if (i % 2 == 0) materialNpc.color = new Color(203f / 255f, 20f / 255f, 20f / 255f, 1f);
            else materialNpc.color = Color.white;

            yield return new WaitForSeconds(0.1f);
        }
    }
    private void Turn()
    {
        float currentY = transform.eulerAngles.y;
        float newY = (currentY + 180f) % 360f;
        targetRotation = Quaternion.Euler(0, newY, 0);
        isTurning = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (walkStepSet)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, walkStep);
            Gizmos.DrawWireSphere(walkStep, 0.5f);
        }

        if (player != null && playerInVisionRange)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    private void ChangeVisibilityKatanaToUnsheathe()
    {
        completeKatana.SetActive(false);
        attackKatana.SetActive(false);

        seatheKatana.SetActive(true);
        sheath.SetActive(true);
    }
    private void ChangeVisibilityKatanaToAttack()
    {
        completeKatana.SetActive(false);
        seatheKatana.SetActive(false);

        attackKatana.SetActive(true);
        sheath.SetActive(true);
    }
    private void ChangeVisibilityKatanaToPatrol()
    {
        completeKatana.SetActive(true);

        seatheKatana.SetActive(false);
        attackKatana.SetActive(false);
        sheath.SetActive(false);
    }
}
