using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //public Button BtnConnectMaster;
    //public Button BtnConnectRoom;

    public bool ConnectingToMaster;
    public bool ConnectingToRoom;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ConnectingToMaster = false;
        ConnectingToRoom = false;
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
        PhotonNetwork.NickName = Environment.UserName;
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

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        ConnectingToRoom = false;
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | Name: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("Network");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 6 });
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
}
