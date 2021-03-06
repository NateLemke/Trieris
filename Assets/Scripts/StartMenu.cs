﻿using Photon.Pun;
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
        if (!PhotonNetwork.IsMasterClient)
        {
            //GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/NotMasterClient").SetActive(true);
            //startNotMasterFade();
            RoomHandling rh = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel").GetComponent<RoomHandling>();
            rh.SendReadyStatus(rh.getSlotPosition(PhotonNetwork.LocalPlayer));
        }
        else
        {
            Debug.Log(RpcTarget.All);
            bool allPlayersReady = true;
            for(int i=1; i<=6; i++){
                if(!(bool) PhotonNetwork.CurrentRoom.CustomProperties["Team" + i + "Ready"]){
                    allPlayersReady = false;
                    break;
                }
            }
            if(allPlayersReady){
                startGame();
            }else{
                GameObject notReadyPanel = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel");
                notReadyPanel = notReadyPanel.transform.Find("PlayersNotReady").gameObject;
                notReadyPanel.SetActive(true);
            }
            //PhotonView.Get(this).RPC("startGame", RpcTarget.All);
        }
    }

    public void closePlayersNotReady(){
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/PlayersNotReady").SetActive(false);
    }


    [PunRPC]
    public void SelectionControlsActivity(bool input){
        for(int i = 1; i <= 6; i++){
            GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().interactable = input;
            GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Dropdown").GetComponent<Dropdown>().interactable = input;
        }
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/FilterPanel/PrivateGameFilter").GetComponent<Toggle>().interactable = input;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/LeaveRoomBtn").GetComponent<Button>().interactable = input;
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
            
            if (selectedTeams.Count == selectedTeams.Distinct().Count())
            {
                PhotonView.Get(this).RPC("SelectionControlsActivity", RpcTarget.Others, false);
                RoomHandling rh = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel").GetComponent<RoomHandling>();
                if (PhotonNetwork.IsMasterClient)
                    setPlayerTeams();

                for (int i = 1; i <= PhotonNetwork.PlayerList.Length; i++)
                {
                    GameManager.teamTypes[GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value] = (Team.Type)Team.Type.player;
                }
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.Log("Duplicates");
                GameObject rp = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel");
                rp.transform.Find("DuplicateTeamPanel").gameObject.SetActive(true);
                startDuplicatePanelFade();
            }
            
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
        
    }
    
    public void startDuplicatePanelFade()
    {
        StartCoroutine(duplicatePanelTime());
    }

    public IEnumerator duplicatePanelTime()
    {
        yield return new WaitForSeconds(1f);
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().CrossFadeAlpha(0, 2f, false);
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Text>().CrossFadeAlpha(0, 2f, false);
        StartCoroutine(setDuplicatePanelInactive());
    }
    
    public IEnumerator setDuplicatePanelInactive()
    {
        yield return new WaitForSeconds(2f);
        var tempColor = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().color;
        tempColor.a = 1.0f;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().color = tempColor;
        tempColor = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Text>().color;
        tempColor.a = 1.0f;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Text>().color = tempColor;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").gameObject.SetActive(false);
    }

    public void startNotMasterFade()
    {
        StartCoroutine(duplicatePanelTime());
    }

    public IEnumerator notMasterTime()
    {
        yield return new WaitForSeconds(2f);
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().CrossFadeAlpha(0, 2f, false);
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Text>().CrossFadeAlpha(0, 2f, false);
        StartCoroutine(setDuplicatePanelInactive());
    }

    public IEnumerator setNotMasterInactive()
    {
        yield return new WaitForSeconds(2f);
        var tempColor = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().color;
        tempColor.a = 1.0f;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").GetComponent<Image>().color = tempColor;
        tempColor = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Text>().color;
        tempColor.a = 1.0f;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel/Text").GetComponent<Text>().color = tempColor;
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/DuplicateTeamPanel").gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the teams that are currently assigned
    /// </summary>
    public void setPlayerTeams()
    {
        for (int i = 1; i <= 6; i++)
        {
            Debug.Log("set team " + (Team.Faction)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value + " to be " + (Team.Type)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value);
            GameManager.teamTypes[GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value] = (Team.Type)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value;
            //if(GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value == 0)
            //{
            //    Debug.Log("set team " + (Team.Faction)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value + " to be ai");
            //    GameManager.teamTypes[GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value] = (Team.Type)0;
            //}
            //else if (GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value == 1)
            //{
                //Debug.Log("set team " + (Team.Faction)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value + " to be human");
                //foreach (Player p in PhotonNetwork.PlayerList)
                //{
                //    if (GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/InformationPanel/Name/Text").GetComponent<Text>().text == p.NickName)
                //    {
                //        ExitGames.Client.Photon.Hashtable ht = p.CustomProperties;
                //        ht["TeamNum"] = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value;
                //        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
                //        Debug.Log("Set " + p.NickName + " to " + p.CustomProperties["TeamNum"]);
                //        break;
                //    }
                //}
                //GameManager.teamTypes[GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value] = (Team.Type)1;
            //}
            //else
            //{
                //Debug.Log("set team " + (Team.Faction)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value + " to be off");
                //GameManager.teamTypes[GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value] = (Team.Type)2;
            //}
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
    
}
