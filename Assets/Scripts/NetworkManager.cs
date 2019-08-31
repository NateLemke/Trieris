using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //public Button BtnConnectMaster;
    //public Button BtnConnectRoom;

    public bool ConnectingToMaster;
    public bool ConnectingToRoom;

    private bool recentRoomPrivacy;

    GameObject thisLobby;
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        ConnectingToMaster = false;
        ConnectingToRoom = false;
        thisLobby = GameObject.Find("Canvas").gameObject;
        thisLobby = thisLobby.transform.Find("MultiplayerPanel/Lobby").gameObject;

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Entered Room");
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (BtnConnectMaster != null)
        //    BtnConnectMaster.gameObject.SetActive(!PhotonNetwork.IsConnected && !ConnectingToMaster);
        //if (BtnConnectRoom != null)
        //    BtnConnectRoom.gameObject.SetActive(PhotonNetwork.IsConnected && !ConnectingToMaster && !ConnectingToRoom);
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Connection: " + PhotonNetwork.IsConnected);
            Debug.Log("OfflineMode: " + PhotonNetwork.OfflineMode);
            Debug.Log("In Lobby: " + PhotonNetwork.InLobby);
            Debug.Log("In Room: " + PhotonNetwork.InRoom);
        }
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    PhotonNetwork.Disconnect();
        //}
    }

    public void OnClickConnectToMaster()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.NickName = GameObject.Find("Canvas/MenuPanel/Menu/MP/InputField").gameObject.GetComponent<InputField>().text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "v1";

        ConnectingToMaster = true;

        GameObject.Find("Canvas/MenuPanel/ConnectionPanel").gameObject.SetActive(true);

        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.ConnectToRegion("usw");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ConnectingToMaster = false;
        ConnectingToRoom = false;
        if(GameObject.Find("Canvas/MultiplayerPanel") != null){
            GameObject.Find("Canvas/MultiplayerPanel").SetActive(false);
        }
        Debug.Log(cause);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        ConnectingToMaster = false;
        Debug.Log("Region: " + PhotonNetwork.CloudRegion);
        GameObject.Find("Canvas/MenuPanel/ConnectionPanel").gameObject.SetActive(false);
        GameObject.Find("Canvas/MenuPanel/Menu").gameObject.GetComponent<StartMenu>().multiplayerGame();
        PhotonNetwork.LocalPlayer.NickName = GameObject.Find("Canvas/MenuPanel/Menu/MP/InputField/Text").GetComponent<Text>().text;
        PhotonNetwork.JoinLobby();
        Debug.Log("Connected to server");
    }

    public void OnClickConnectToRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        ConnectingToRoom = true;

        Hashtable privacyHash = new Hashtable() { { "Privacy" , false } };
        PhotonNetwork.JoinRandomRoom(privacyHash, 0);
    }

    public override void OnLeftRoom(){
        base.OnLeftRoom();
        Debug.Log("Left Room");
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        ConnectingToRoom = false;
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | Name: " + PhotonNetwork.CurrentRoom.Name);
        GameObject.Find("Canvas/MenuPanel/Menu").gameObject.GetComponent<StartMenu>().OpenRoom();

        //Hashtable roominfo = new Hashtable();
        //roominfo.Add("MasterName", PhotonNetwork.CurrentRoom.GetPlayer(1).NickName);
        //roominfo.Add("RoomName", PhotonNetwork.CurrentRoom.Name);
        //PhotonNetwork.CurrentRoom.SetCustomProperties(roominfo);

        Debug.Log("Privacy? " + PhotonNetwork.CurrentRoom.CustomProperties["Privacy"]);

        //setPrivacyToggle();

        listAllPlayersInRoom();
    }
    
    public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
    {
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel").GetComponent<RoomHandling>().UpdatePlayerList();
        Debug.Log(newPlayer.NickName + " has left the room");
        
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel").GetComponent<RoomHandling>().UpdatePlayerList();
        Debug.Log(newPlayer.NickName + " has entered the room");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        OnClickCreateRoom(0);
    }

    /// <summary>
    /// Creates a room
    /// public room: privacy = 0
    /// private room: privacy = 1
    /// </summary>
    /// <param name="privacy"></param>
    public void OnClickCreateRoom(int privacy)
    {
        RoomOptions options = new RoomOptions();
        options.IsVisible = true;
        options.IsOpen = true;
        options.MaxPlayers = 6;
        string[] customProps = { "MasterName", "RoomName", "Privacy", "Password", "InProgress" };
        options.CustomRoomPropertiesForLobby = customProps;

        //setPrivacyToggle(privacy);
        recentRoomPrivacy = privacy == 1;

        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        
        Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        ht["MasterName"] = newMasterClient.NickName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " Master is now " + newMasterClient.NickName);

        PhotonView.Get(GameObject.Find("Canvas/MultiplayerPanel/RoomPanel").gameObject).RPC("setLocalPlayerTeam", RpcTarget.All);
    }

    public override void OnCreatedRoom()
    {
        Hashtable roominfo = new Hashtable();
        roominfo.Add("MasterName", PhotonNetwork.CurrentRoom.GetPlayer(1).NickName);
        roominfo.Add("RoomName", PhotonNetwork.CurrentRoom.Name);
        roominfo.Add("Privacy", recentRoomPrivacy);
        roominfo.Add("Password", PhotonNetwork.CurrentRoom.Name.Substring(0, 5));
        roominfo.Add("InProgress", false);
        PhotonNetwork.CurrentRoom.SetCustomProperties(roominfo);
    }

    //private void setPrivacyToggle()
    //{
    //    GameObject thisRoom = GameObject.Find("Canvas").gameObject;
    //    thisRoom = thisRoom.transform.Find("MultiplayerPanel/RoomPanel").gameObject;
    //    thisRoom.GetComponent<RoomHandling>().privateGame = thisRoom.transform.Find("FilterPanel/PrivateGameFilter").gameObject.GetComponent<Toggle>();
    //    thisRoom.GetComponent<RoomHandling>().privateGame.isOn = (bool)PhotonNetwork.CurrentRoom.CustomProperties["Privacy"];
    //    //thisRoom.GetComponent<RoomHandling>().setRoomName();
    //}

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        ConnectingToRoom = false;
    }

    public void ChangeTeamImage(int slot)
    {
        int inputTeamNum = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/TeamImage/Dropdown").GetComponent<Dropdown>().value;
        Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        ht["Team" + slot + "Int"] = (int)inputTeamNum;
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        PhotonView.Get(this).RPC("UpdateTeamImage", RpcTarget.Others);
    }

    [PunRPC]
    //public void SendTeamImage(int slot, int inputTeamNum)
    public void UpdateTeamImage()
    {
        Debug.Log("Updating Team Image");
        for(int i = 1; i <= 6; i++){
            Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
            GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value = (int)ht["Team" + i + "Int"];
        }
        //GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/TeamImage/Dropdown").GetComponent<Dropdown>().value = inputTeamNum;
        //Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        //ht["Team" + slot + "Int"] = (int)inputTeamNum;
        //PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    }

    public void ChangeTeamTypeDropdown(int slot)
    {
        //if (GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value == 2)
        //    GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value = 0;
        //else if (GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value == 1 && slot > PhotonNetwork.CountOfPlayers)
        //    GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value = 0;
        //int inputTeamTypeNum = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value;
        //PhotonView.Get(this).RPC("SendTeamTypeDropdown", RpcTarget.Others, slot, inputTeamTypeNum);
    }

    [PunRPC]
    public void SendTeamTypeDropdown(int slot, int inputTeamTypeNum)
    {
        GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slot + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value = inputTeamTypeNum;
        Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        ht["Team" + slot + "Type"] = (int)inputTeamTypeNum;
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    }

    //public override void OnCreatedRoom()
    //{
    //    base.OnCreatedRoom();
    //}
    public void OnClickDisconnect()
    {
        PhotonNetwork.Disconnect();
    }
    
    public void listAllPlayersInRoom()
    {
        Debug.Log("Players: ");
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i].ActorNumber);
        }
        Debug.Log("List End.");
    }
}
