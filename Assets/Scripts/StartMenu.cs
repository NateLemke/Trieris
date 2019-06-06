using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains functions that are used for the buttons on the main menu.
/// </summary>
public class StartMenu : MonoBehaviour
{
    /// <summary>
    /// Starts the game by loading the main game scene.
    /// </summary>
    public void startGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Exits the game and closes the application.
    /// </summary>
    public void exitGame()
    {
        //For Editor Closing
        //UnityEditor.EditorApplication.isPlaying = false;
        //For Build Closing
        Application.Quit();
    }

    /// <summary>
    /// Opens the rulebook.
    /// </summary>
    public void RuleBookButton()
    {
        Application.OpenURL("http://trieris.ca/rulebook/");
    }

    /// <summary>
    /// Opens the Credits page
    /// </summary>
    public void CreditsButton()
    {
        GameObject credits = GameObject.Find("Canvas/Panel");
        credits.transform.Find("Credits").gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the Credits page
    /// </summary>
    public void CloseCredits()
    {
        GameObject credits = GameObject.Find("Canvas/Panel");
        credits.transform.Find("Credits").gameObject.SetActive(false);
    }
}
