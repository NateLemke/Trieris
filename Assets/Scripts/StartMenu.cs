using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Contains functions that are used for the buttons on the main menu.
/// </summary>
public class StartMenu : MonoBehaviour
{
    void Start()
    {
        transform.Find("MP/InputField").gameObject.GetComponent<InputField>().text = Environment.UserName;
    }

    /// <summary>
    /// Starts the game by loading the main game scene.
    /// </summary>
    [PunRPC]
    public void startGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void startMultiplayerGame()
    {
        PhotonView.Get(this).RPC("startGame", RpcTarget.All);
    }

    public void multiplayerGame()
    {
        GameObject parent = GameObject.Find("Canvas").gameObject;
        parent.transform.Find("MultiplayerPanel").gameObject.SetActive(true);
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
        GameObject credits = GameObject.Find("Canvas");
        credits.transform.Find("Credits").gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the Credits page
    /// </summary>
    public void CloseCredits()
    {
        GameObject credits = GameObject.Find("Canvas");
        credits.transform.Find("Credits").gameObject.SetActive(false);
    }

    public void CloseMultiplayerPanel()
    {
        GameObject.Find("Canvas/MultiplayerPanel").SetActive(false);
    }

    public void LeaveRoom()
    {
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel").gameObject.SetActive(false);
    }

    public void OpenRoom()
    {
        GameObject mpPanel = GameObject.Find("Canvas/MultiplayerPanel").gameObject;
        mpPanel.transform.Find("RoomPanel").gameObject.SetActive(true);
        //RoomHandling rh = mpPanel.transform.Find("RoomPanel").GetComponent<RoomHandling>();
        //rh.setLocalPlayerTeam();
    }
}
