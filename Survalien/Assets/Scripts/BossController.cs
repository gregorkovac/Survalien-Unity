using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    enum State
    {
        Hidden,
        Summoned,
        Stage1,
        Stage2,
        Stage3,
        Defeated,
        Run
    }

    public Animator animator;
    public GameObject projectilePrefab;
    public GameObject rocket;
    public Slider healthBar;

    private CharacterController characterController;
    private Transform playerTransform;  
    private Vector3 lastKnownPlayerPosition;
    private Vector3 vectTo;
    private float playerX;
    private float playerY;
    private float playerZ;
    private GameObject player;

    private State state;
    private State prevState;

    private bool isDead;

    private float stateChangeTimer;

    // Start is called before the first frame update
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        playerTransform = GameObject.Find("Player").transform;
        state = State.Stage1;

        stateChangeTimer = 2f;

        isDead = false;

        InvokeRepeating("Stage1Attack", 1, 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        if (characterController.health <= 0) {
            state = State.Defeated;
            isDead = true;
            animator.SetTrigger("isDead");
        }

        healthBar.value = characterController.health;

        stateChangeTimer -= Time.deltaTime;

        if (stateChangeTimer <= 0) {
            stateChangeTimer = 2f;

            if (state == State.Stage1) {
                prevState = state;
                state = State.Run;
                characterController.Sprinting();
                CancelInvoke("Stage1Attack");
                animator.SetBool("isRunning", true);
            } else if (state == State.Stage2) {
                prevState = state;
                state = State.Run;
                characterController.Sprinting();
                CancelInvoke("Stage2Attack");
                animator.SetBool("isRunning", true);
            } else if (state == State.Run) {
                animator.SetBool("isRunning", false);
                characterController.Idle();
                state = prevState;
                if (state == State.Stage1) {
                    InvokeRepeating("Stage1Attack", 1, 0.7f);
                } else if (state == State.Stage2) {
                    InvokeRepeating("Stage2Attack", 1, 2f);
                }
            }
        }

        if(state == State.Stage1 && characterController.health < 50) {
            state = State.Stage2;
            CancelInvoke("Stage1Attack");
            InvokeRepeating("Stage2Attack", 1, 2f);
        }
        else if(state == State.Stage2 && characterController.health < 30) {
            state = State.Stage3;
            CancelInvoke("Stage2Attack");
        }
        else if(state == State.Stage3 && characterController.health <= 0) {
            state = State.Defeated;
            animator.SetTrigger("IsDead");
        }

        switch (state) {
            case State.Hidden:
                // Hide game object
                this.gameObject.SetActive(false);
                break;
            case State.Summoned:
                // Show game object
                playerX = playerTransform.position.x;
                playerY = playerTransform.position.y;
                playerZ = playerTransform.position.z;
                vectTo = new Vector3(playerX, playerY, playerZ);
                BossIntro(this.transform.position, vectTo);
                state = State.Stage1;
                InvokeRepeating("Stage1Attack", 1, 0.33f);
                break;
            case State.Stage1:
                characterController.RotateTowards(playerTransform.position);
                // characterController.Idle();
                break;
            case State.Stage2:
            characterController.RotateTowards(playerTransform.position);
                break;
            case State.Stage3:
                break;
            case State.Defeated:
                break;
            case State.Run:
                characterController.RotateTowards(playerTransform.position);
                characterController.Move();
                break;
        }
    }
    public void StateSummoned() {
        state = State.Summoned;
        this.gameObject.SetActive(true);
    }
    public void BossIntro(Vector3 from, Vector3 to) {
        transform.position = Vector3.Lerp(from, to, 0.005f);
    }
    
    void Shoot() {
        GameObject projectile = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);
    }



    void Stage1Attack(){
        Vector3 projectileSpawnPoint = this.transform.position + this.transform.forward * 2;

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);

        animator.SetTrigger("Attack1");
    }

    void Stage2Attack(){
        Vector3 projectileSpawnPoint = this.transform.position + this.transform.forward * 5;

        GameObject projectile = Instantiate(rocket, projectileSpawnPoint, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);

        animator.SetTrigger("Attack2");
    }

}
