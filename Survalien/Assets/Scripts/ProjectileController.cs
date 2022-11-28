using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private GameObject owner;

    public GameObject hitParticlePrefab;

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != owner)
        {
            GameObject hitParticle = Instantiate(hitParticlePrefab, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
            Destroy(hitParticle, 5);
        }
    }
}
