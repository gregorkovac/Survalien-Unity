using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public AudioSource menuMusic;

    public Animator anim;

    public SceneChangerScript sceneChangerScript;

    public GameObject menuPanel;

    [SerializeField] private float _time = 6f;

    private bool startedGame = false;

    void Start() {
        Time.timeScale = 1f;
    }

    void Update() {
        if (startedGame) {
            menuMusic.volume -= 0.001f;
        }
    }

  public void Play()
    {

        menuPanel.SetActive(false);

        anim.SetTrigger("PlayAnimation");

        Invoke("StartGame", _time);

    }
    
    public void StartGame()
    {
        startedGame = true;

        sceneChangerScript.FadeToScene("Game - Level Generation");
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