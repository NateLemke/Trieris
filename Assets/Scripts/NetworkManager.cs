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

    GameObject thisLobby;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ConnectingToMaster = false;
        ConnectingToRoom = false;
        thisLobby = GameObject.Find("Canvas").gameObject;
        thisLobby = thisLobby.transform.Find("MultiplayerPanel/Lobby").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //if (BtnConnectMaster != null)
        //    BtnConnectMaster.gameObject.SetActive(!PhotonNetwork.IsConnected && !ConnectingToMaster);
        //if (BtnConnectRoom != null)
        //    BtnConnectRoom.gameObject.SetActive(PhotonNetwork.IsConnected && !ConnectingToMaster && !ConnectingToRoom);
    }

    public void OnClickConnectToMaster()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.NickName = GameObject.Find("Canvas/MenuPanel/Menu/MP/InputField").gameObject.GetComponent<InputField>().text;
        //PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "v1";

        ConnectingToMaster = true;

        GameObject.Find("Canvas/MenuPanel/ConnectionPanel").gameObject.SetActive(true);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ConnectingToMaster = false;
        ConnectingToRoom = false;
        Debug.Log(cause);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        ConnectingToMaster = false;

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
        PhotonNetwork.JoinRandomRoom();
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

        Hashtable roominfo = new Hashtable();
        roominfo.Add("MasterName", PhotonNetwork.CurrentRoom.GetPlayer(1).NickName);
        roominfo.Add("RoomName", PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.CurrentRoom.SetCustomProperties(roominfo);

        listAllPlayersInRoom();
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
        options.IsOpen = privacy == 0;
        options.MaxPlayers = 6;
        string[] customProps = { "MasterName", "RoomName" };
        options.CustomRoomPropertiesForLobby = customProps;

        setPrivacyToggle(privacy);

        PhotonNetwork.CreateRoom(null, options);
    }

    private void setPrivacyToggle(int privacy)
    {
        GameObject thisRoom = GameObject.Find("Canvas").gameObject;
        thisRoom = thisRoom.transform.Find("MultiplayerPanel/RoomPanel").gameObject;
        thisRoom.GetComponent<RoomHandling>().privateGame = thisRoom.transform.Find("FilterPanel/PrivateGameFilter").gameObject.GetComponent<Toggle>();
        thisRoom.GetComponent<RoomHandling>().privateGame.isOn = privacy == 1;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        ConnectingToRoom = false;
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
