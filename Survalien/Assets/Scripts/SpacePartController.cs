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
        spaceship = GameObject.Find("UFO");
        spaceshipPos = spaceship.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                transform.Rotate(50* Time.deltaTime, 50* Time.deltaTime, 0);
                transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * 0.005f, transform.position.z);
                break;
            case State.Collected:
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z + 0.8f);

                break;
            case State.Returned:
                transform.position = Vector3.Lerp(this.transform.position, spaceshipPos, 0.05f);
                if (Vector3.Distance(this.transform.position, spaceshipPos) < 0.1f) {
                    Destroy(this.gameObject);
                }
                break;
        }
        
    }
    public void SetReturned() {
        state = State.Returned;
    }

    public bool StateColleted() {
        return state == State.Collected;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null && !collision.gameObject.GetComponent<PlayerController>().HoldingSpacePart()) {
            if (state == State.Idle){
                collision.gameObject.GetComponent<PlayerController>().CollectSpacePart();
                state = State.Collected;
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
                this.transform.localScale = this.transform.localScale * 0.8f;
            }

        }

    }
}
