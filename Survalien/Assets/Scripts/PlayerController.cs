using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;
    public Animator playerAnimator;
    public Transform cameraTransform;
    public float speed = 10.0f;
    public int collected = 0;
    public int returned = 0;
    public GameObject projectilePrefab;
    public GameObject reloadPanel;
    public Canvas userInterface;
    public GameObject heartSprite;
    public Slider reloadSlider;
    public PostProcessVolume postProcessVolume;
    public GameObject indicator;
    public ParticleSystem reloadParticles;
    
    private ColorGrading colorGradingVolume;
    private Vignette vignetteVolume;

    private Rigidbody rb;
    private Vector3 movement;

    private float cameraRotationOffsetX;

    private int shotCount = 0;

    private List<GameObject> hearts;

    private bool isDead = false;

    private CharacterController characterController;

    private bool isVictory;

    private GameObject currentObjective;

    // Start is called before the first frame update
    void Start()
    {
        reloadPanel.SetActive(false);
        rb = this.GetComponent<Rigidbody>();

        characterController = this.GetComponent<CharacterController>();

        cameraRotationOffsetX = cameraTransform.rotation.eulerAngles.x;

        hearts = new List<GameObject>();

        for (int i = 0; i < characterController.health; i++)
        {
            GameObject heart = Instantiate(heartSprite, userInterface.transform);

            heart.transform.position = new Vector3(heart.transform.position.x + (((float)i/20) * Screen.width), heart.transform.position.y, heart.transform.position.z);

            //heart.transform.position = new Vector3(heart.transform.position.x + (i * (heart.GetComponent<RectTransform>().sizeDelta.x)), heart.transform.position.y, heart.transform.position.z);
            hearts.Add(heart);
        }

        isDead = false;
        isVictory = false;
        characterController.StopMoveParticles();

        postProcessVolume.profile.TryGetSettings(out colorGradingVolume);
        postProcessVolume.profile.TryGetSettings(out vignetteVolume);

        //currentObjective = gameObject;

        FindNearestSpacePart();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentObjective == null) {
            if (this.returned >= 3)
                FindBoss();
            else
                FindNearestSpacePart();
        }

        Vector3 objectiveDir = transform.position - currentObjective.transform.position;
        objectiveDir = new Vector3(objectiveDir.x, 0, objectiveDir.z);
        indicator.transform.rotation = Quaternion.LookRotation(objectiveDir);
        //indicator.transform.position = transform.position + indicator.transform.forward * Mathf.Sin(Time.time) * 0.3f;
        //indicator.transform.position = new Vector3(indicator.transform.position.x, 0.5f, indicator.transform.position.z);

        if (isVictory)
            return;

        if (isDead) {
            if (Time.timeScale > 0.5f) {
                Time.timeScale -= 0.01f;
                colorGradingVolume.saturation.value -= 1f;
                vignetteVolume.intensity.value += 0.001f;
            }
            return;
        }

        // Get movement input
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // int m = 0;
        // if (Input.GetAxis("Vertical") > 0) {
        //     m = 1;
        // } else if (Input.GetAxis("Vertical") < 0) {
        //     m = -1;
        // }

        // playerAnimator.SetInteger("Movement", m);

        Vector3 mousePos = Input.mousePosition;

        mousePos.z = 0;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(playerTransform.position);
        mousePos.x = mousePos.x - playerPos.x;
        mousePos.y = mousePos.y - playerPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        // Rotate the player towards the mouse
        playerTransform.rotation = Quaternion.Euler(new Vector3(0, -angle - 270, 0));

        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");

        if (hMove > 0) { 
            hMove = 1;
        }
        else if (hMove < 0) {
            hMove = -1;
        }

        if (vMove > 0) {
            vMove = 1;
        } else if (vMove < 0) {
            vMove = -1;
        }

        int moveAngle = 0;
        int lookAngle = 0;

        if (hMove == 1) {
            moveAngle = 0;
        } else if (vMove == 1) {
            moveAngle = 1;
        } else if (hMove == -1) {
            moveAngle = 2;
        } else if (vMove == -1) {
            moveAngle = 3;
        }

        if (angle >= 45 && angle < 135)
            lookAngle = 0;
        else if (angle >= 135 || angle < -135)
            lookAngle = 1;
        else if (angle >= -135 && angle < -45)
            lookAngle = 2;
        else if (angle >= -45 && angle < 45)
            lookAngle = 3;

        int combinedAngle = (moveAngle + 4 - lookAngle) % 4;

        if (moveAngle == 0) {
            characterController.RotateMoveParticles(270);
            characterController.PlayMoveParticles();
        } else if (moveAngle == 1) {
            characterController.RotateMoveParticles(0);
            characterController.PlayMoveParticles();
        } else if (moveAngle == 2) {
            characterController.RotateMoveParticles(90);
            characterController.PlayMoveParticles();
        } else if (moveAngle == 3) {
            characterController.RotateMoveParticles(180);
            characterController.PlayMoveParticles();
        }

        if (hMove == 0 && vMove == 0) {
            combinedAngle = -1;
            characterController.StopMoveParticles();
        }

        playerAnimator.SetInteger("Run Direction", combinedAngle);

        // Shoot a projectile
        if (Input.GetMouseButtonDown(0)) {
            if (shotCount < 5) {
                playerAnimator.SetTrigger("Shoot");

                Vector3 newProjectilePos = playerTransform.position + playerTransform.forward * 2.5f;
                newProjectilePos.y = 1.5f;

                GameObject projectile = Instantiate(projectilePrefab, newProjectilePos, playerTransform.rotation);

                projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);

                shotCount++;

                if (shotCount >= 5)
                    StartCoroutine(ReloadGun());
            }
        }

        //  Vector3 newCameraPos = playerTransform.position;
        //  newCameraPos.y += 10;
        //  newCameraPos.x = (int)((newCameraPos.x + 10) / 20) * 20f;
        //  newCameraPos.z = (int)((newCameraPos.z + 10) / 20) * 20f - 10f;

        //  cameraTransform.position = Vector3.Lerp(cameraTransform.position, newCameraPos, 0.1f);

        // Debug.Log(((playerTransform.position.x + 10) % 20 - 10));

       // Vector3 newCameraRot = new Vector3(90, -90, ((playerTransform.position.x + 10) % 20 - 10));

        //cameraTransform.rotation = Quaternion.Euler(newCameraRot);

        //cameraTransform.rotation = Quaternion.Euler(new Vector3(, playerTransform.rotation.eulerAngles.y, 0));
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    void LateUpdate() {
        // Lock parent object rotation
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void OnDeath() {
        if (isDead)
            return;

        
        Debug.Log("Player died");

        isDead = true;
        playerAnimator.SetTrigger("Death");
    }

    // Wait for some time before allowing the player to shoot again
    IEnumerator ReloadGun() {
        reloadParticles.Play();
        //reloadPanel.SetActive(true);
        float progress = 0.0f;
        for(int i = 0; i < 25; i++) {
            progress+=0.04f;
            yield return new WaitForSeconds(0.04f);
            reloadSlider.value = progress;
            // Debug.Log(progress);
        }

        shotCount = 0;
        reloadPanel.SetActive(false);
    }
    
    public void UpdateHearts() {
        for (int i = 0; i < hearts.Count; i++)
        {   
            if (i >= this.GetComponent<CharacterController>().health) {
                hearts[i].SetActive(false);
            } else {
                hearts[i].SetActive(true);
            }
        }
    }
    public void CollectSpacePart(){
        if(this.collected == 0) {
            this.collected++;

            FindSpaceship();
        }
    }
    public void ReturnSpacePart(){
        if(this.collected == 1) {
            this.returned++;
            this.collected = 0;
            if (this.returned == 3)
                FindBoss();
            else 
                FindNearestSpacePart();
        }
    }

    public void EndGame(Vector3 spaceshipPos) {
        //isDead = true;
        indicator.SetActive(false);
        isVictory = true;
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<Rigidbody>().useGravity = false;
    }

    public void FindNearestSpacePart() {
        GameObject[] spaceParts = GameObject.FindGameObjectsWithTag("SpacePart");
        float minDistance = 1000000f;
        GameObject nearestSpacePart = null;

        foreach (GameObject spacePart in spaceParts) {
            float distance = Vector3.Distance(spacePart.transform.position, this.transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                nearestSpacePart = spacePart;
            }
        }

        if (nearestSpacePart != null) {
            currentObjective = nearestSpacePart;
        }
    }

    public void FindSpaceship() {
        currentObjective = GameObject.FindGameObjectWithTag("Spaceship");
    }

    public void FindBoss() {
        currentObjective = GameObject.FindGameObjectWithTag("Boss");
    }

}
