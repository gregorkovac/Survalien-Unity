using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{


    enum State {
        Idle,
        Victory
    }
    private State state;
    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Victory:
                break;
        }
        
    }

    void OnTriggerEnter(Collider collision){
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            int stCollected = collision.gameObject.GetComponent<PlayerController>().collected;
            if(stCollected > 0) {
                int stReturned = collision.gameObject.GetComponent<PlayerController>().returned;
                // set SpacePart state to returned
                SpacePartController[] spaceParts = FindObjectsOfType<SpacePartController>();
                SpacePartController iskani;
                foreach (SpacePartController spacePart in spaceParts) {
                    if (spacePart.state == SpacePartController.State.Collected){
                        iskani = spacePart;
                        break;
                    }
                }

                if (stReturned < 3) {
                    collision.gameObject.GetComponent<PlayerController>().ReturnSpacePart(iskani);
                }
                else if (collision.gameObject.GetComponent<PlayerController>().returned == 3){
                state = State.Victory;
                }
            }
        }
    }
}
