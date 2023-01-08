using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    enum State
    {
        Idle,
        Searching,
        Attacking,
        Stalking
    }

    public GameObject grenadePrefab;
    public GameObject projectilePrefab;
    public GameObject pistolFireProjectile;
    public float timeBetweenStateChanges = 5.0f;

    public Transform bulletSpawnPoint;

    public float sightRange = 20f;

    public AudioSource[] shootSounds;

    private CharacterController characterController; 
    private Transform playerTransform;  
    private Vector3 lastKnownPlayerPosition;

    private Animator animator;

    private bool isDead;
    private bool startThrowing;

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
        startThrowing = false;
    }

    // Update is called once per frame
    void Update()
    {
            if(!startThrowing && characterController.health < 6) {
                InvokeRepeating("ThrowGrenade", 1, 1.5f);
                CancelInvoke("Shoot");
                startThrowing = true;
                }

        if (isDead){
            return;
        }
        else if (state != State.Attacking) {
            // If the player is in sight, start attacking
            if (Vector3.Distance(transform.position, playerTransform.position) < sightRange && 
                characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject))
            {
                characterController.Idle();
                if(!startThrowing) {
                InvokeRepeating("Shoot", 1, 0.7f);
                }

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
                    CancelInvoke("ThrowGrenade");
                    state = State.Idle;
                    animator.SetBool("IsShooting", false);
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsWalking", false);
                }
                else if (Vector3.Distance(transform.position, playerTransform.position) > sightRange || 
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
                if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < 1f)
                {
                    characterController.Walking();
                    state = State.Idle;
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsWalking", false);
                }
            break;
        }
    }
    
    void Shoot() {
        shootSounds[Random.Range(0, shootSounds.Length)].Play();

        animator.SetTrigger("Shoot");
        GameObject projectile = Instantiate(projectilePrefab, bulletSpawnPoint.position, this.transform.rotation);
        GameObject pistolFire = Instantiate(pistolFireProjectile, bulletSpawnPoint.position, this.transform.rotation);

        projectile.transform.position += projectile.transform.forward * 0.5f;
        pistolFire.transform.position += pistolFire.transform.forward * 0.5f;

        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);
    }
    void ThrowGrenade(){
        animator.SetTrigger("Shoot");    
        GameObject grenade = Instantiate(grenadePrefab, bulletSpawnPoint.position, this.transform.rotation);
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
    }

    IEnumerator DeathAnimation() {
        yield return new WaitForSeconds(2f);
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
