using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{


    enum State {
        Idle,
        Battle,
        Victory
    }
    public GameObject boss;
    public Animator animator;
    public GameObject playerModel;
    public GameObject particles;

    private State state;
    private GameObject player;
    private float x;
    private float y;
    private float z;
    private GameObject bossInstance;
    
    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        //boss = GameObject.Find("Boss");
        player = GameObject.Find("Player");
        x = this.transform.position.x;
        y = this.transform.position.y;
        z = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                

                break;
            case State.Battle:
                if (bossInstance.GetComponent<CharacterController>().IsDead() &&
                Vector3.Distance(this.transform.position, player.transform.position) < 10f) {
                    state = State.Victory;
                    
                    player.transform.position = Vector3.Lerp(player.transform.position, this.transform.position, 0.005f);

                    player.GetComponent<PlayerController>().EndGame(this.transform.position);

                    particles.SetActive(true);

                    StartCoroutine(EndGame());
                }

                break;
            case State.Victory:
                /*transform.Rotate(0, 80* Time.deltaTime, 0);
                transform.position = Vector3.Lerp(this.transform.position, new Vector3(x, y + 15, z), 0.005f);
                if(Vector3.Distance(this.transform.position, new Vector3(x, y + 15, z)) < 0.1f) {
                    Destroy(this.gameObject);
                    //display victory screen
                }*/
                break;
        }
        
    }
    bool BossBattle(){
            Debug.Log(gameObject.GetComponent<PlayerController>());
        return (
            gameObject.GetComponent<PlayerController>().returned == 2 
            && gameObject.GetComponent<PlayerController>().collected == 1 
            && Vector3.Distance(player.transform.position, this.transform.position) < 10);

    }
    public void StateIdle(){
        state = State.Idle;
    }

    void OnTriggerEnter(Collider collision){
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {

            int stCollected = collision.gameObject.GetComponent<PlayerController>().collected;
            if(stCollected > 0) {
                // set SpacePart state to returned
                SpacePartController[] spaceParts = FindObjectsOfType<SpacePartController>();
                    collision.gameObject.GetComponent<PlayerController>().ReturnSpacePart();
                    foreach (SpacePartController spacePart in spaceParts) {
                        if (spacePart.StateColleted()) {
                            spacePart.SetReturned();
                            break;
                        }
                }
                int stReturned = collision.gameObject.GetComponent<PlayerController>().returned;
                if(stReturned == 1){
                    state = State.Battle;
                    Debug.Log("Battle");
                    // set boss state to summoned
                    //boss.GetComponent<BossController>().StateSummoned();
                    //Vector3 playerPos = player.transform.position;
                   
                          Enemy[] enemies = FindObjectsOfType<Enemy>();
                    Civilian[] civilians = FindObjectsOfType<Civilian>();

                    foreach (Enemy enemy in enemies) {
                        enemy.OnDeath();
                    }

                    foreach (Civilian civilian in civilians) {
                        civilian.OnDeath();
                    }

                    bossInstance = Instantiate(boss, new Vector3(transform.position.x, transform.position.y + 20, transform.position.z - 10) , Quaternion.identity);

                }
               // if (collision.gameObject.GetComponent<PlayerController>().returned == 3 && state != State.Battle){
            }
        }
    }

    IEnumerator EndGame() {
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Rigidbody>().isKinematic = true;
        playerModel.SetActive(false);
        animator.SetBool("Takeoff", true);
    }
}
