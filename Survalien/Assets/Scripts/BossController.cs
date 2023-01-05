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
        Defeated
    }

    public GameObject projectilePrefab;
    public GameObject rocket;
    public Slider healthBar;
    private CharacterController characterController; s
    private Transform playerTransform;  
    private Vector3 lastKnownPlayerPosition;
    private Vector3 vectTo;
    private float playerX;
    private float playerY;
    private float playerZ;
    private GameObject player;

    private State state;

    // Start is called before the first frame update
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        playerTransform = GameObject.Find("Player").transform;
        state = State.Stage2;
          InvokeRepeating("Stage2Attack", 1, 0.20f);

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = characterController.health;

        if(state == State.Stage1 && characterController.health < 50) {
            state = State.Stage2;
            CancelInvoke("Stage1Attack");
            InvokeRepeating("Stage2Attack", 1, 0.20f);
        }
        else if(state == State.Stage2 && characterController.health < 30) {
            state = State.Stage3;
            CancelInvoke("Stage2Attack");
        }
        else if(state == State.Stage3 && characterController.health <= 0) {
            state = State.Defeated;
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
        GameObject projectile = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);
    }

    void Stage2Attack(){
        GameObject projectile = Instantiate(rocket, this.transform.position, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);

    }

}
