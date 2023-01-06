using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum State
    {
        Idle,
        Searching,
        Attacking,
        Stalking,
        RunningAway
    }

    public GameObject projectilePrefab;
    public GameObject pistolFireProjectile;
    public float timeBetweenStateChanges = 5.0f;

    public Transform bulletSpawnPoint;

    private CharacterController characterController; 
    private Transform playerTransform;  
    private Vector3 lastKnownPlayerPosition;

    private Animator animator;

    private bool isDead;

    [SerializeField] private State state;
    [SerializeField] private float stateChangeTimer;

    // Start is called before the first frame update
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        animator = this.transform.GetChild(0).GetComponent<Animator>();

        playerTransform = GameObject.Find("Player").transform;

        state = State.Idle;
        stateChangeTimer = timeBetweenStateChanges;

        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        // If health is low, start running away
        if (characterController.health <= 2) {
            state = State.RunningAway;
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsShooting", false);
            CancelInvoke();
        }
        else if (state != State.Attacking) {
            // If the player is in sight, start attacking
            if (Vector3.Distance(transform.position, playerTransform.position) < 10f && 
                characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject))
            {
                characterController.Idle();
                InvokeRepeating("Shoot", 1, 1);
                animator.SetBool("IsShooting", true);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", false);
                state = State.Attacking;
            } else {
                // Change state from idle to searching and vice versa
                stateChangeTimer -= Time.deltaTime;
                if (stateChangeTimer <= 0) {
                    stateChangeTimer = timeBetweenStateChanges;
                    if (state == State.Idle) {
                        characterController.RotateRandom();
                        state = State.Searching;
                        animator.SetBool("IsWalking", true);
                    } else if (state == State.Searching) {
                        state = State.Idle;
                        animator.SetBool("IsWalking", false);
                    }
                }
            }
        }

        switch (state) {
            case State.Idle:
                characterController.Idle();
            break;

            case State.Searching:
                characterController.Move();
            break;

            case State.Attacking:
                if (playerTransform.GetComponent<CharacterController>().health <= 0) {
                    CancelInvoke("Shoot");
                    state = State.Idle;
                    animator.SetBool("IsShooting", false);
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsWalking", false);
                }
                else if (Vector3.Distance(transform.position, playerTransform.position) > 10f || 
                    !characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject))
                {
                    CancelInvoke("Shoot");
                    state = State.Stalking;
                    animator.SetBool("IsRunning", true);
                    animator.SetBool("IsShooting", false);
                    characterController.Sprinting();
                } else {
                    characterController.RotateTowards(playerTransform.position);
                    lastKnownPlayerPosition = playerTransform.position;
                }
            break;

            case State.Stalking:
                // Move to the last known player position
                characterController.Move();
                characterController.RotateTowards(lastKnownPlayerPosition);
                if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < 5f)
                {
                    characterController.Walking();
                    state = State.Idle;
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsWalking", false);
                }
            break;

            case State.RunningAway:
                // Run away from the player if health is low
                characterController.Sprinting();
                characterController.RotateAwayFrom(playerTransform.position);
                characterController.Move();
                if (Vector3.Distance(transform.position, playerTransform.position) > 10f 
                || !characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject))
                {
                    characterController.Walking();
                    characterController.Idle();
                    state = State.Idle;
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsWalking", false);
                }
            break;
        }
    }
    
    void Shoot() {
        animator.SetTrigger("Shoot");
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawnPoint.position, this.transform.rotation);
        GameObject pistolFire = Instantiate(pistolFireProjectile, bulletSpawnPoint.position, this.transform.rotation);

        projectile.transform.position += projectile.transform.forward * 0.5f;
        pistolFire.transform.position += pistolFire.transform.forward * 0.5f;

        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);
    }

    public void OnDeath() {
        CancelInvoke();
        characterController.Idle();
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsShooting", false);
        animator.SetTrigger("Death");
        isDead = true;

        StartCoroutine(DeathAnimation());

        //this.GetComponent<CapsuleCollider>().height = 1f;
    }

    IEnumerator DeathAnimation() {
        yield return new WaitForSeconds(2f);
        //Destroy(this.gameObject);
        this.GetComponent<CapsuleCollider>().height = 1.5f;
    }

    // Rotate towards player if you get alerted
    public void Alert() {
        if (state == State.Idle || state == State.Searching || state == State.Stalking) {
            state = State.Stalking;
            animator.SetBool("IsWalking", true);
            lastKnownPlayerPosition = playerTransform.position;
        }
    }
}
