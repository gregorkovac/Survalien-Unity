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

    private Rigidbody rb;
    private Vector3 movement;
    private Material defaultMaterial;

    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();   
        //material = this.GetComponent<Renderer>().material;
        defaultMaterial = modelRenderer.material;

        targetRotation = Quaternion.Euler(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);

            if (gameObject.tag == "Player")
            {
                GetComponent<PlayerController>().OnDeath();
            } else {
                Destroy(this.gameObject);
            }
        }

        if (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    void LateUpdate() {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    public void Move() {
        movement = transform.forward;
    }

    public void Idle() {
        movement = new Vector3(0, 0, 0);
    }

    public void RotateRandom() {
        float angle = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(0, angle, 0);
    }

    public void RotateTowards(Vector3 target) {
        Vector3 direction = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
        targetRotation = transform.rotation;
    }

    public void RotateAwayFrom(Vector3 target) {
        Vector3 direction = transform.position - target;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
        targetRotation = transform.rotation;
    }

    public void SetTargetRotation(Quaternion rotation) {
        targetRotation = rotation;
    }

    public void DecreaseHealth() {
        health--;
        modelRenderer.material = hitMaterial;
        StartCoroutine(ResetMaterial());

        if (this.gameObject.tag == "Player")
        {
            GetComponent<PlayerController>().UpdateHearts();
        } else {
            this.GetComponent<Enemy>().Alert();
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
}
