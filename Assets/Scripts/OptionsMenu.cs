using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose:    This class manages the OptionMenu that shows when the game is paused. From this menu the player can
///                 quit the game or view the game's instructions.
/// </summary>
public class OptionsMenu : MonoBehaviourPunCallbacks
{
    // reference to the scene's main UI overlay canvas
    GameObject overlay;

    // Value used to confirm going to start menu or restarting the game
    bool confirmationValue;

    IEnumerator curCoroutine;

    /// <summary>
    /// Causes the program to close. Called by the quit game button.s
    /// </summary>
    public void QuitGame()
    {
        //For Editor Closing
        //UnityEditor.EditorApplication.isPlaying = false;
        overlay = GameObject.Find("OverlayCanvas");
        //For Build Closing
        Application.Quit();
    }

    /// <summary>
    /// Closes the option menu and resumes the game by resetting the timescale.
    /// </summary>
    public void CloseOptions()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Opens the options menu and pauses the game by setting the timescale to 0;
    /// </summary>
    public void OpenOptions()
    {
        if(!PhotonNetwork.IsConnected)
            Time.timeScale = 0;
    }

    /// <summary>
    /// Opens the instructions menu when the view instructions button is pressed
    /// </summary>
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

    public void goToStartMenu()
    {
        transform.Find("ConfirmationPanel").gameObject.SetActive(true);
        curCoroutine = StartMenuEnumerator();
        StartCoroutine(curCoroutine);
    }
    
    public void restartGame()
    {
        transform.Find("ConfirmationPanel").gameObject.SetActive(true);
        curCoroutine = RestartEnumerator();
        StartCoroutine(curCoroutine);
    }
    
    private IEnumerator StartMenuEnumerator()
    {
        while (!confirmationValue)
        {
            yield return null;
        }
        PhotonNetwork.Disconnect();
        //curCoroutine = DisconnectEnumerator();
        //StartCoroutine(DisconnectEnumerator());
    }

    public override void OnDisconnected(DisconnectCause cause){
        base.OnDisconnected(cause);
        GameManager.main.goToStartMenu();
    }
    private IEnumerator DisconnectEnumerator()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        GameManager.main.goToStartMenu();
    }

    private IEnumerator RestartEnumerator()
    {
        while (!confirmationValue)
        {
            yield return null;
        }
        GameManager.main.restartGame();
    }

    public void confirmConfirmation()
    {
        confirmationValue = true;
        transform.Find("ConfirmationPanel").gameObject.SetActive(false);
    }

    public void cancelConfirmation()
    {
        StopCoroutine(curCoroutine);
        transform.Find("ConfirmationPanel").gameObject.SetActive(false);
    }
}
