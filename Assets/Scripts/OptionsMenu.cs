using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public void QuitGame()
    {
        //For Editor Closing
        UnityEditor.EditorApplication.isPlaying = false;
        //For Build Closing
        Application.Quit();
    }
    public void CloseOptions()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    public void OpenOptions()
    {
        Time.timeScale = 0;
    }
}
