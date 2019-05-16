using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose:    This class manages the OptionMenu that shows when the game is paused. From this menu the player can
///                 quit the game or view the game's instructions.
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    // reference to the scene's main UI overlay canvas
    GameObject overlay;

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
        Time.timeScale = 0;
    }

    /// <summary>
    /// Opens the instructions menu when the view instructions button is pressed
    /// </summary>
    public void OpenInstructions()
    {
        GameObject overlay = GameObject.Find("OverlayCanvas");
        overlay.transform.Find("HelpPanel").gameObject.SetActive(true);
    }
}
