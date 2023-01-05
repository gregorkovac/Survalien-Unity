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
    private State state;
    public GameObject boss;
    private GameObject player;
    private float x;
    private float y;
    private float z;
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
                break;
            case State.Victory:
                transform.Rotate(0, 80* Time.deltaTime, 0);
                transform.position = Vector3.Lerp(this.transform.position, new Vector3(x, y + 15, z), 0.005f);
                if(Vector3.Distance(this.transform.position, new Vector3(x, y + 15, z)) < 0.1f) {
                    Destroy(this.gameObject);
                    //display victory screen
                }
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
                   
                    Instantiate(boss, new Vector3(transform.position.x + 10, transform.position.y + 20, transform.position.z) , Quaternion.identity);
                }
                if (collision.gameObject.GetComponent<PlayerController>().returned == 3 && state != State.Battle){
                state = State.Victory;
                }
            }
        }
    }
}
