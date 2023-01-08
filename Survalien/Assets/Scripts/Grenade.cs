using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    private Transform playerTransform;
    private float timer;
    private float half;
    private GameObject owner;
    [SerializeField] GameObject explosion;
    private Vector3 playerPosition;
    // Start is called before the first frame update
    void Start()
    {
        timer = 1.5f;
        playerTransform = GameObject.Find("Player").transform;
        
        float distToPlayer = Vector3.Distance(this.transform.position, playerTransform.position);

        if (distToPlayer < 15f) {
            playerPosition = playerTransform.position;
        } else {
            Vector3 dirToPlayer = (playerTransform.position - this.transform.position).normalized;
            playerPosition = this.transform.position + dirToPlayer * 15f;
        }

        half = (transform.position.x + playerPosition.x )/ 2 - 0.33f;

        
    }

    void Update(){
        timer -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, playerPosition,0.03f);
            if(transform.position.x > half){
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.03f, transform.position.z);
            }
            else if( transform.position.x < half && transform.position.y > 0.1f) {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.03f, transform.position.z);
            }


        if(timer <= 0){
            Explode();
        }
    }

        public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }


    void Explode(){

        GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(newExplosion, 1.4f);
          if (Vector3.Distance(this.transform.position, playerTransform.position) < 3.33f) {
             playerTransform.gameObject.GetComponent<CharacterController>().DecreaseHealth(2);

          }
           Soldier[] soldiers = FindObjectsOfType<Soldier>();
                    foreach (Soldier soldier in soldiers) {
                        if (Vector3.Distance(this.transform.position, soldier.transform.position) < 3.33f) {
                            soldier.transform.gameObject.GetComponent<CharacterController>().DecreaseHealth(5);
                        }
                }
            Enemy[] enemies = FindObjectsOfType<Enemy>();
                    foreach (Enemy enemy in enemies) {
                        if (Vector3.Distance(this.transform.position, enemy.transform.position) < 3.33f) {
                            enemy.transform.gameObject.GetComponent<CharacterController>().DecreaseHealth(5);
                        }
                }


        Destroy(this.gameObject);
    }

}
