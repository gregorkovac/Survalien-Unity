using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 10.0f;
    public int health = 10;
    public Material hitMaterial;
    public Renderer modelRenderer;
    public GameObject deathParticles;
    public GameObject bleedingParticles;

    private Rigidbody rb;
    private Vector3 movement;
    private Material defaultMaterial;

    private Quaternion targetRotation;

    private GameObject bleedingInstance = null;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();   
        defaultMaterial = modelRenderer.material;

        targetRotation = Quaternion.Euler(0,0,0);
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

            if (gameObject.tag == "Player")
            {
                GetComponent<PlayerController>().OnDeath();
            } else {
                //Destroy(this.gameObject);
                GetComponent<Enemy>().OnDeath();
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
           // if (Random.value > 0.5)
                targetRotation = Quaternion.Euler(0, transform.localEulerAngles.y + 90, 0);
         //   else
         //       targetRotation = Quaternion.Euler(0, transform.localEulerAngles.y - 90, 0);
        } 
        movement = transform.forward;
    }

    public void Idle() {
        // Stop character movement
        movement = new Vector3(0, 0, 0);
    }

    public void RotateRandom() {
        // Randomly rotate
        float angle = Random.Range(0, 360);
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

    public void DecreaseHealth() {
        health--;
        modelRenderer.material = hitMaterial;
        StartCoroutine(ResetMaterial());

        if (this.gameObject.tag == "Player")
        {
            GetComponent<PlayerController>().UpdateHearts();
        } else if (this.gameObject.tag == "Enemy") {
            this.GetComponent<Enemy>().Alert();
        } else if (this.gameObject.tag == "Civilian") {
            this.GetComponent<Civilian>().Alert();
        }
    }

    public void Sprinting() {
        speed = 3.0f;
    }

    public void Walking() {
        speed = 1.0f;
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
}
