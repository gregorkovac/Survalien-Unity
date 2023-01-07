using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Animator anim;

    public SceneChangerScript sceneChangerScript;

    public GameObject menuPanel;

    [SerializeField] private float _time = 6f;
  //Load Scene
  public void Play()
    {
        //Debug.Log("In play");
    
        menuPanel.SetActive(false);

        anim.SetTrigger("PlayAnimation");
        //z delayem izvedi
        //StartCoroutine(StartGame(_time));
        Invoke("StartGame", _time);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
/*
    public IEnumerator StartGame(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }*/
    
    public void StartGame()
    {
        sceneChangerScript.FadeToScene("Game - Level Generation");
        //Invoke("LoadScene", 1f);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    } 

    public void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    //Quit Game
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Said I quit");
    }
}