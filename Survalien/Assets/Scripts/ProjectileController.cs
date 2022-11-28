using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject hitParticlePrefab;

    private GameObject owner;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Rigidbody>().AddForce(this.transform.forward * 1000);

        Destroy(this.gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject != owner)
        {
            if (collision.gameObject.GetComponent<CharacterController>() != null)
            {
                collision.gameObject.GetComponent<CharacterController>().DecreaseHealth();
            }


            if (hitParticlePrefab != null) {
                GameObject hitParticle = Instantiate(hitParticlePrefab, this.transform.position, this.transform.rotation);
                Destroy(hitParticle, 5);
            }


            Destroy(this.gameObject);
        }
    }
}
