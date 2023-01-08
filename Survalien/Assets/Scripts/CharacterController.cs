using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float walkingSpeed = 1.0f;
    public float runningSpeed = 3.0f;

    public int health = 10;
    public Material hitMaterial;
    public Renderer modelRenderer;
    public GameObject deathParticles;
    public GameObject bleedingParticles;
    public ParticleSystem stepParticles;

    public AudioSource[] stepSounds;
    public AudioSource[] hitSounds;
    public AudioSource impactSound;
    public AudioSource deathSound;

    private float speed = 10.0f;
    private Rigidbody rb;
    private Vector3 movement;
    private Material defaultMaterial;

    private Quaternion targetRotation;

    private GameObject bleedingInstance = null;

    private bool isDead = false;

    private float stepTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();   
        defaultMaterial = modelRenderer.material;

        targetRotation = Quaternion.Euler(0,0,0);

        speed = walkingSpeed;

        if (stepParticles != null) {
            stepParticles.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        // On death
        if (health <= 0)
        {
            isDead = true;

            Destroy(bleedingInstance);
            Instantiate(deathParticles, transform.position, Quaternion.identity);

            deathSound.Play();

            if (gameObject.tag == "Player")
            {
                GetComponent<PlayerController>().OnDeath();
            } else if (gameObject.tag == "Enemy")
            {
                GetComponent<Enemy>().OnDeath();
            } else if (gameObject.tag == "Civilian") {
                GetComponent<Civilian>().OnDeath();
            } else if (gameObject.tag == "Soldier") {
                GetComponent<Soldier>().OnDeath();
            }

        } else if (health <= 2 && bleedingInstance == null) {
            bleedingInstance = Instantiate(bleedingParticles, transform.position, Quaternion.identity);
            bleedingInstance.transform.position = new Vector3(bleedingInstance.transform.position.x, bleedingInstance.transform.position.y + 0.5f, bleedingInstance.transform.position.z);
            bleedingInstance.transform.parent = this.transform;
        } else if (health > 2) {
            bleedingInstance = null;
        }

        // Rotate the character towards the target
        if (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }

    void FixedUpdate()
    {
        // Move the character
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    void LateUpdate() {
        // Lock the character rotation
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    public void Move() {
        // Move the character and rotate if there is an object in front of it
        if (DistanceFromObjectInFront() < 2f) {
                targetRotation = Quaternion.Euler(0, transform.localEulerAngles.y + 90, 0);
        } 
        movement = transform.forward;

        if (stepTimer < 0.0f) {
            stepSounds[Random.Range(0, stepSounds.Length)].Play();
            
            if (speed == runningSpeed) {
                stepTimer = 0.2f;
            } else {
                stepTimer = 0.5f;
            }
        } else {
            stepTimer += Time.deltaTime;
        }
    }

    public void Idle() {
        // Stop character movement
        movement = new Vector3(0, 0, 0);
        StopMoveParticles();
        stepTimer = 0.2f;
    }

    public void RotateRandom() {
        // Randomly rotate
        float angle = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(0, angle, 0);
    }

    public void RotateRandomFor(int x){
        x %= 360;
        float angle = Random.Range(-x, x);
        targetRotation = Quaternion.Euler(0, angle, 0);
    }

    public void RotateTowards(Vector3 target) {
        // Rotate towards a target
        Vector3 direction = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
        targetRotation = transform.rotation;
    }

    public void RotateAwayFrom(Vector3 target) {
        // Rotate away from a target
        Vector3 direction = transform.position - target;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
        targetRotation = transform.rotation;
    }

    public void SetTargetRotation(Quaternion rotation) {
        // Set the angle to rotate to
        targetRotation = rotation;
    }

    public void DecreaseHealth(int ammount) {

        hitSounds[Random.Range(0, hitSounds.Length)].Play();
        impactSound.Play();

        health -= ammount;
        modelRenderer.material = hitMaterial;
        StartCoroutine(ResetMaterial());

        if (this.gameObject.tag == "Player")
        {
            GetComponent<PlayerController>().UpdateHearts();
        } else if (this.gameObject.tag == "Enemy") {
            this.GetComponent<Enemy>().Alert();
        } else if (this.gameObject.tag == "Civilian") {
            this.GetComponent<Civilian>().Alert();
        } else if (this.gameObject.tag == "Soldier") {
            this.GetComponent<Soldier>().Alert();
        }
    }

    public void Sprinting() {
        speed = runningSpeed;
        PlayMoveParticles();
    }

    public void Walking() {
        speed = walkingSpeed;
        StopMoveParticles();
    }

    IEnumerator ResetMaterial() {
        yield return new WaitForSeconds(0.08f);
        modelRenderer.material = defaultMaterial;
    }

    // Check if there is an object between obj1 and obj2
    public bool isVisionUnobstructed(GameObject obj1, GameObject obj2) {
        Vector3 direction = obj2.transform.position - obj1.transform.position;
        float distance = Vector3.Distance(obj1.transform.position, obj2.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(obj1.transform.position, direction, out hit, distance)) {
            if (hit.collider.gameObject == obj2) {
                return true;
            }
        }
        return false;
    }

    // Return the distance from the object in front of the character
    public float DistanceFromObjectInFront() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
            return hit.distance;
        }
        return 0;
    }

    public bool IsDead() {
        return isDead;
    }

    public void PlayMoveParticles() {
        if (stepParticles != null && !stepParticles.isPlaying)
            stepParticles.Play();
    }

    public void StopMoveParticles() {
        if (stepParticles != null && stepParticles.isPlaying)
            stepParticles.Stop();
    }

    public void RotateMoveParticles(float angle) {
        if (stepParticles != null) {
            ParticleSystem.ShapeModule shape = stepParticles.shape;
            shape.rotation = new Vector3(0, angle, 0);
        }
    }
}
