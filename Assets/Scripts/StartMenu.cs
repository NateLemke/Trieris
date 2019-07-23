using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Contains functions that are used for the buttons on the main menu.
/// </summary>
public class StartMenu : MonoBehaviourPun
{
    void Start()
    {
        transform.Find("MP/InputField").gameObject.GetComponent<InputField>().text = Environment.UserName;
    }
    
    public void startMultiplayerGame()
    {
        Debug.Log(RpcTarget.All);
        PhotonView.Get(this).RPC("startGame", RpcTarget.All);
    }

    /// <summary>
    /// Starts the game by loading the main game scene.
    /// </summary>
    [PunRPC]
    public void startGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            List<int> selectedTeams = new List<int>();
            for(int i = 1; i <= 6; i++)
            {
                selectedTeams.Add(GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value);
            }
            if(selectedTeams.Count == selectedTeams.Distinct().Count())
            {
                setPlayerTeams();
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").SetActive(true);
                startDuplicatePanelFade();
            }
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
        
    }

    /// <summary>
    /// Starts the fade out of the objective panel
    /// </summary>
    public void startDuplicatePanelFade()
    {
        StartCoroutine(duplicatePanelTime());
    }

    /// <summary>
    /// Waits 2 seconds and then begins the fade of the objective text 
    /// </summary>
    /// <returns></returns>
    public IEnumerator duplicatePanelTime()
    {
        yield return new WaitForSeconds(2f);
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().CrossFadeAlpha(0, 2f, false);
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Text>().CrossFadeAlpha(0, 2f, false);
        StartCoroutine(setDuplicatePanelInactive());
    }

    /// <summary>
    /// Sets the objective panel inactive once the fade out is finished
    /// </summary>
    /// <returns></returns>
    public IEnumerator setDuplicatePanelInactive()
    {
        yield return new WaitForSeconds(2f);
        var tempColor = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().color;
        tempColor.a = 1.0f;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().color = tempColor;
        tempColor = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Image>().color;
        tempColor.a = 1.0f;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Image>().color = tempColor;
        GameObject.Find("OverlayCanvas/Objective").gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the teams that are currently assigned
    /// </summary>
    private void setPlayerTeams()
    {
        for (int i = 1; i <= 6; i++)
        {
            if(GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Name/Text").GetComponent<Text>().text != "Empty")
            {
                Debug.Log("set team " + i + " to be human");
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if(GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Name/Text").GetComponent<Text>().text == p.NickName)
                    {
                        ExitGames.Client.Photon.Hashtable ht = p.CustomProperties;
                        ht["TeamNum"] = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value;
                        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
                        break;
                    }
                }
                GameManager.teamTypes[i - 1] = (Team.Type)1;
            }
        }
        
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log((string)p.CustomProperties["TeamInt"]);
            }
        }
    }
}
