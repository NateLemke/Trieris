using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{

    GameObject overlay;

    public void QuitGame()
    {
        //For Editor Closing
        //UnityEditor.EditorApplication.isPlaying = false;
        overlay = GameObject.Find("OverlayCanvas");
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

    public void OpenControls()
    {
        GameObject overlay = GameObject.Find("OverlayCanvas");
        overlay.transform.Find("HelpPanel").gameObject.SetActive(true);
        overlay.transform.Find("HelpPanel/Rules").gameObject.SetActive(false);
    }

    public void OpenRules()
    {
        GameObject overlay = GameObject.Find("OverlayCanvas");
        overlay.transform.Find("HelpPanel").gameObject.SetActive(true);
        overlay.transform.Find("HelpPanel/Rules").gameObject.SetActive(true);
    }
}
