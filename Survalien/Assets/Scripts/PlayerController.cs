using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;
    public Animator playerAnimator;
    public Transform cameraTransform;
    public float speed = 10.0f;
    public GameObject projectilePrefab;
    public GameObject reloadPanel;
    public Canvas userInterface;
    public GameObject heartSprite;
    public Slider reloadSlider;


    private Rigidbody rb;
    private Vector3 movement;

    private float cameraRotationOffsetX;

    private int shotCount = 0;

    private List<GameObject> hearts;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        reloadPanel.SetActive(false);
        rb = this.GetComponent<Rigidbody>();

        cameraRotationOffsetX = cameraTransform.rotation.eulerAngles.x;

        hearts = new List<GameObject>();

        for (int i = 0; i < this.GetComponent<CharacterController>().health; i++)
        {
            GameObject heart = Instantiate(heartSprite, userInterface.transform);

            heart.transform.position = new Vector3(heart.transform.position.x + (((float)i/20) * Screen.width), heart.transform.position.y, heart.transform.position.z);

            //heart.transform.position = new Vector3(heart.transform.position.x + (i * (heart.GetComponent<RectTransform>().sizeDelta.x)), heart.transform.position.y, heart.transform.position.z);
            hearts.Add(heart);
        }

        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        // Get movement input
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        int m = 0;
        if (Input.GetAxis("Vertical") > 0) {
            m = 1;
        } else if (Input.GetAxis("Vertical") < 0) {
            m = -1;
        }

        playerAnimator.SetInteger("Movement", m);

        Vector3 mousePos = Input.mousePosition;

        mousePos.z = 0;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(playerTransform.position);
        mousePos.x = mousePos.x - playerPos.x;
        mousePos.y = mousePos.y - playerPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        // Rotate the player towards the mouse
        playerTransform.rotation = Quaternion.Euler(new Vector3(0, -angle - 270, 0));

        // Shoot a projectile
        if (Input.GetMouseButtonDown(0)) {
            if (shotCount < 5) {
                playerAnimator.SetTrigger("Shoot");

                Vector3 newProjectilePos = playerTransform.position + playerTransform.forward;
                newProjectilePos.y = 1;

                GameObject projectile = Instantiate(projectilePrefab, newProjectilePos, playerTransform.rotation);

                projectile.GetComponent<ProjectileController>().SetOwner(this.gameObject);

                shotCount++;

                if (shotCount >= 5)
                    StartCoroutine(ReloadGun());
            }
        }

        //  Vector3 newCameraPos = playerTransform.position;
        //  newCameraPos.y += 10;
        //  newCameraPos.x = (int)((newCameraPos.x + 10) / 20) * 20f;
        //  newCameraPos.z = (int)((newCameraPos.z + 10) / 20) * 20f - 10f;

        //  cameraTransform.position = Vector3.Lerp(cameraTransform.position, newCameraPos, 0.1f);

        // Debug.Log(((playerTransform.position.x + 10) % 20 - 10));

       // Vector3 newCameraRot = new Vector3(90, -90, ((playerTransform.position.x + 10) % 20 - 10));

        //cameraTransform.rotation = Quaternion.Euler(newCameraRot);

        //cameraTransform.rotation = Quaternion.Euler(new Vector3(, playerTransform.rotation.eulerAngles.y, 0));
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    void LateUpdate() {
        // Lock parent object rotation
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void OnDeath() {
        if (isDead)
            return;
        
        Debug.Log("Player died");

        isDead = true;
        playerAnimator.SetTrigger("Death");
    }

    // Wait for some time before allowing the player to shoot again
    IEnumerator ReloadGun() {
        reloadPanel.SetActive(true);
        float progress = 0.0f;
        for(int i = 0; i < 25; i++) {
            progress+=0.04f;
            yield return new WaitForSeconds(0.04f);
            reloadSlider.value = progress;
            // Debug.Log(progress);
        }

        shotCount = 0;
        reloadPanel.SetActive(false);
    }
    
    public void UpdateHearts() {
        for (int i = 0; i < hearts.Count; i++)
        {   
            if (i >= this.GetComponent<CharacterController>().health) {
                hearts[i].SetActive(false);
            } else {
                hearts[i].SetActive(true);
            }
        }
    }
}
