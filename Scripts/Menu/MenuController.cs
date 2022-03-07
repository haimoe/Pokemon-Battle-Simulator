using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Application was quit.");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F5))
        {
            Restart();
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Application was quit.");
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}



