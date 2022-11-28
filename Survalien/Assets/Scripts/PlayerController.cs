using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public Transform playerTransform;
    public Transform cameraTransform;
    private Rigidbody rb;
    private Vector3 movement;
    
    private float cameraRotationOffsetX;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        cameraRotationOffsetX = cameraTransform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector3 mousePos = Input.mousePosition;

        cameraTransform.rotation = Quaternion.Euler(cameraRotationOffsetX - mousePos.y / 100 + 5, mousePos.x / 100 - 5, cameraTransform.rotation.z);

        mousePos.z = 0;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(playerTransform.position);
        mousePos.x = mousePos.x - playerPos.x;
        mousePos.y = mousePos.y - playerPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        playerTransform.rotation = Quaternion.Euler(new Vector3(0, -angle - 270, 0));
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
}
