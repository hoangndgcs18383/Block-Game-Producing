using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    bool pause = false;
    public void OnPause()
    {
        pause = true;
    }

    public void OnResume()
    {
        pause = false;
    }

    public void OnReset()
    {
        SceneManager.LoadScene(0);
    }

    public void onQuit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (pause == true)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
