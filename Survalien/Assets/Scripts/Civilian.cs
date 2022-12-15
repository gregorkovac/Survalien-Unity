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

    private CharacterController characterController; 
    private Transform playerTransform;  

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
        }
        else if (state != State.RunningAway) {
            if (characterController.isVisionUnobstructed(this.gameObject, playerTransform.gameObject)) {
                state = State.RunningAway;
            } else {
                state = State.Idle;
            }

            // Change state from idle to walking and vice versa
            stateChangeTimer -= Time.deltaTime;
            if (stateChangeTimer <= 0) {
                stateChangeTimer = timeBetweenStateChanges;
                if (state == State.Idle) {
                    characterController.RotateRandom();
                    state = State.Walking;
                } else if (state == State.Walking) {
                    state = State.Idle;
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
                }
            break;
        }
    }

    public void Alert() {
        if (state == State.Idle || state == State.Walking) {
            state = State.RunningAway;
        }
    }
}
