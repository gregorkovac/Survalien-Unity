using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamWallInteraction : MonoBehaviour
{
    public float maxCompensate = 1.0f;

    private float compensate = 0.0f;
    private bool inTrigger = false;

    private void OnTriggerEnter(Collider collision)
    {
        inTrigger = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        inTrigger = false;
    }

    void OnTriggerStay(Collider collision)
    {
        // if (collision.gameObject.name == "spaceship")
        //     return;

        // if (compensate < maxCompensate) {
        //     compensate += 0.1f;
        //     transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
        //     //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 0.5f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        // }        
    }
}
