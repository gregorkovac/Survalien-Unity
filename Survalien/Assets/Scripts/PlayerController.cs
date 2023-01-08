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

    public AudioSource[] alienSounds;
    public AudioSource[] shootSounds;
    public AudioSource[] stepSounds;
    public AudioSource reloadSound;
    public AudioSource deathSound;
    public AudioSource collectSpacePartSound;
    public AudioSource returnSpacePartSound;
    public AudioSource victorySound;

    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject introPanel;
    
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

    private float stepSoundCooldown = 0.0f;

    private int stepSoundIndex = 0;

    private bool introSequence = true;

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

            hearts.Add(heart);
        }

        isDead = false;
        isVictory = false;
        characterController.StopMoveParticles();

        postProcessVolume.profile.TryGetSettings(out colorGradingVolume);
        postProcessVolume.profile.TryGetSettings(out vignetteVolume);

        FindNearestSpacePart();
    }

    // Update is called once per frame
    void Update()
    {
        if (introSequence)
            return;

        if (currentObjective == null) {
            if (this.returned >= 3)
                FindBoss();
            else
                FindNearestSpacePart();
        }

        Vector3 objectiveDir = transform.position - currentObjective.transform.position;
        objectiveDir = new Vector3(objectiveDir.x, 0, objectiveDir.z);
        indicator.transform.rotation = Quaternion.LookRotation(objectiveDir);

        if (isVictory)
            return;

        if (isDead) {
            gameOverPanel.SetActive(true);

            if (Time.timeScale > 0.5f) {
                Time.timeScale -= 0.01f;
                colorGradingVolume.saturation.value -= 1f;
                vignetteVolume.intensity.value += 0.001f;
            }
            return;
        }

        // Get movement input
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

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

            stepSoundCooldown = 0f;
        } else {
            if (stepSoundCooldown <= 0) {
                stepSoundCooldown = 0.3f;

                stepSounds[stepSoundIndex].Play();

                stepSoundIndex = (stepSoundIndex + 1) % stepSounds.Length;
            } else {
                stepSoundCooldown -= Time.deltaTime;
            }
        }

        playerAnimator.SetInteger("Run Direction", combinedAngle);

        // Shoot a projectile
        if (Input.GetMouseButtonDown(0)) {
            if (shotCount < 5) {
                playerAnimator.SetTrigger("Shoot");

                shootSounds[Random.Range(0, shootSounds.Length)].Play();

                Vector3 newProjectilePos = playerTransform.position + playerTransform.forward * 2.5f;
                newProjectilePos.y = 1.5f;

                GameObject projectile = Instantiate(projectilePrefab, newProjectilePos, playerTransform.rotation);

                projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);

                shotCount++;

                if (shotCount >= 5) {
                    reloadSound.Play();
                    StartCoroutine(ReloadGun());
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (introSequence || isDead || isVictory)
            return;
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

        deathSound.Play();
        
        Debug.Log("Player died");

        isDead = true;
        playerAnimator.SetTrigger("Death");
    }

    // Wait for some time before allowing the player to shoot again
    IEnumerator ReloadGun() {
        reloadParticles.Play();
        float progress = 0.0f;
        for(int i = 0; i < 25; i++) {
            progress+=0.04f;
            yield return new WaitForSeconds(0.04f);
            reloadSlider.value = progress;
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

            collectSpacePartSound.Play();

            FindSpaceship();
        }
    }
    public void ReturnSpacePart(){
        if(this.collected == 1) {
            returnSpacePartSound.Play();
            this.returned++;
            this.collected = 0;
            if (this.returned == 3)
                FindBoss();
            else 
                FindNearestSpacePart();
        }
    }

    public bool HoldingSpacePart() {
        if (this.collected != 0)
            return true;
        return false;
    }

    public void EndGame(Vector3 spaceshipPos) {
        victoryPanel.SetActive(true);
        victorySound.Play();
        indicator.SetActive(false);
        isVictory = true;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
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

    public void StartGame() {
        introSequence = false;
        introPanel.SetActive(false);
    }

}
