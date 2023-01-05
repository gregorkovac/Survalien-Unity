using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : MonoBehaviour
{
    enum State
    {
        Idle,
        Walking,
        Curious,
        RunningAway
    }

    public GameObject projectilePrefab;
    public float timeBetweenStateChanges = 5.0f;
    public Animator animator;

    private CharacterController characterController; 
    private Transform playerTransform;  

    [SerializeField] private State state;
    [SerializeField] private float stateChangeTimer;

    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
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
        }
        else if (state != State.RunningAway) {
            if (characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject)) {
                state = State.RunningAway;
                animator.SetBool("IsRunning", true);
                animator.SetBool("IsWalking", false);
            } else {

            // Change state from idle to walking and vice versa
            stateChangeTimer -= Time.deltaTime;
            if (stateChangeTimer <= 0) {
                stateChangeTimer = timeBetweenStateChanges;
                if (state == State.Idle) {
                    characterController.RotateRandom();
                    state = State.Walking;
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsRunning", false);
                } else if (state == State.Walking) {
                    state = State.Idle;
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsRunning", false);
                }
            }
            }
        }

        switch (state) {
            case State.Idle:
                characterController.Idle();
            break;

            case State.Walking:
                characterController.Move();
            break;

            case State.Curious:
                characterController.RotateTowards(playerTransform.position);
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
                    Debug.Log("State: " + state);

                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsRunning", false);
                }
            break;
        }
    }

    public void Alert() {
        if (state == State.Idle || state == State.Walking) {
            state = State.RunningAway;

            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", false);
        }
    }

    public void OnDeath() {
        isDead = true;
        animator.SetTrigger("Death");
        characterController.Idle();
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation() {
        yield return new WaitForSeconds(2f);
        //Destroy(this.gameObject);
        this.GetComponent<CapsuleCollider>().height = 1.5f;
    }
}
