using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangerScript : MonoBehaviour
{

    public Animator animator;
    private string sceneToLoad;

    // Update is called once per frame
    
    void Update()
    {
        /*if(SceneManager.GetActiveScene().buildIndex == 1) {
            Debug.Log("In game");
            animator.ResetTrigger("FadeOutTrigger");
            animator.SetTrigger("FadeInTrigger");
        }
        Debug.Log("In Update");*/
    }

    public void FadeToScene (string sceneName) 
    {
        Debug.Log("in fade function");
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOutTrigger");
        
    }

    public void OnFadeComplete () {
        Debug.Log("oncomplete");
        animator.ResetTrigger("FadeOutTrigger");
        animator.SetTrigger("FadeInTrigger");
        LoadSc();
        
    } 
    public void LoadSc () {
        SceneManager.LoadScene(1);
    }

    public void MethodA() {
        Debug.Log("delaaa");

    }
}
