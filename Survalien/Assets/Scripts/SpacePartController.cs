using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePartController : MonoBehaviour
{
    enum State {
        Idle,
        Collected,
        Returned
    }
    private State state;
    private GameObject player;
    private GameObject spaceship;
    private Vector3 spaceshipPos;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        player = GameObject.Find("Player");
        spaceship = GameObject.Find("Spaceship");
        spaceshipPos = spaceship.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                transform.Rotate(0, 50* Time.deltaTime, 0);
                break;
            case State.Collected:
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z + 0.8f);

                break;
            case State.Returned:
                transform.position = Vector3.Lerp(this.transform.position, spaceshipPos, 0.1f);
                if (Vector3.Distance(this.transform.position, spaceshipPos) < 0.1f) {
                    Destroy(this.gameObject);
                }
                break;
        }
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null) {
            if (state == State.Idle){
                collision.gameObject.GetComponent<PlayerController>().CollectSpacePart();
                state = State.Collected;
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
                this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

        }

    }
}
