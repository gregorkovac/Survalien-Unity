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
    public float timeBetweenStateChanges = 5.0f;

    private CharacterController characterController; 
    private Transform playerTransform;  
    private Vector3 lastKnownPlayerPosition;

    [SerializeField] private State state;
    [SerializeField] private float stateChangeTimer;

    // Start is called before the first frame update
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        playerTransform = GameObject.Find("Player").transform;

        state = State.Idle;
        stateChangeTimer = timeBetweenStateChanges;
    }

    // Update is called once per frame
    void Update()
    {
        // If health is low, start running away
        if (characterController.health <= 2) {
            state = State.RunningAway;
            CancelInvoke();
        }
        else if (state != State.Attacking) {
            // If the player is in sight, start attacking
            if (Vector3.Distance(transform.position, playerTransform.position) < 10f && 
                characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject))
            {
                characterController.Idle();
                InvokeRepeating("Shoot", 1, 1);
                state = State.Attacking;
            } else {
                // Change state from idle to searching and vice versa
                stateChangeTimer -= Time.deltaTime;
                if (stateChangeTimer <= 0) {
                    stateChangeTimer = timeBetweenStateChanges;
                    if (state == State.Idle) {
                        characterController.RotateRandom();
                        state = State.Searching;
                    } else if (state == State.Searching) {
                        state = State.Idle;
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
                // If the player is out of sight, start stalking
                if (Vector3.Distance(transform.position, playerTransform.position) > 10f || 
                    !characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject))
                {
                    CancelInvoke("Shoot");
                    state = State.Stalking;
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
                }
            break;
        }
    }
    
    void Shoot() {
        GameObject projectile = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);
    }

    // Rotate towards player if you get alerted
    public void Alert() {
        if (state == State.Idle || state == State.Searching || state == State.Stalking) {
            state = State.Stalking;
            lastKnownPlayerPosition = playerTransform.position;
        }
    }
}
