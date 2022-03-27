using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartButton(int scene) 
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
