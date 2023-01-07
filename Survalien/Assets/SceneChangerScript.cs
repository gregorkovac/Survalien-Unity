using UnityEngine;

public class SceneChangerScript : MonoBehaviour
{

    public Animator animator;

    // Update is called once per frame
    void Update()
    {
         Debug.Log("In Update");
        if (Input.GetKey("space")) {
             Debug.Log("In if");
            FadeToScene(1);
        }
    }

    public void FadeToScene (int sceneIndex) 
    {
        Debug.Log("in fade function");
        animator.SetTrigger("FadeInTrigger");
    }
}
