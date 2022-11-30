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
        //InvokeRepeating("Shoot", 1, 1);

        state = State.Idle;
        stateChangeTimer = timeBetweenStateChanges;
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.health <= 2) {
            state = State.RunningAway;
            CancelInvoke();
        }
        else if (state != State.Attacking) {
            if (Vector3.Distance(transform.position, playerTransform.position) < 5f)
            {
                characterController.Idle();
                InvokeRepeating("Shoot", 1, 1);
                state = State.Attacking;
            } else {
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
                if (Vector3.Distance(transform.position, playerTransform.position) > 5f)
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
                characterController.Move();
                characterController.RotateTowards(lastKnownPlayerPosition);
                if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < 1f)
                {
                    characterController.Walking();
                    state = State.Idle;
                }
            break;

            case State.RunningAway:
                characterController.Sprinting();
                characterController.RotateAwayFrom(playerTransform.position);
                characterController.Move();
                if (Vector3.Distance(transform.position, playerTransform.position) > 10f)
                {
                    characterController.Walking();
                    characterController.Idle();
                    state = State.Idle;
                }
            break;
        }

        //characterController.Move();
        //characterController.RotateTowards(playerTransform.position);
    }
    
    void Shoot() {
        GameObject projectile = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);
    }

    public void Alert() {
        if (state == State.Idle || state == State.Searching || state == State.Stalking) {
            state = State.Stalking;
            lastKnownPlayerPosition = playerTransform.position;
        }
    }
}
