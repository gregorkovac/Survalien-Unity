using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public GameObject projectilePrefab;

    private CharacterController characterController; 
    private Transform playerTransform;  

    // Start is called before the first frame update
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        playerTransform = GameObject.Find("Player").transform;
        InvokeRepeating("Shoot", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move();
        characterController.RotateTowards(playerTransform.position);
    }
    
    void Shoot() {
        GameObject projectile = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);
    }
}
