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

    public bool isCollected = false;
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
            case State.Collected:
                break;
            case State.Returned:
            
                break;
        }
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (!isCollected){
                collision.gameObject.GetComponent<PlayerController>().collect();
                state = State.Collected;
            }

        }
    }
}
