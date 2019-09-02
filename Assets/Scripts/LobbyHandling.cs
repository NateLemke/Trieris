using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandling : MonoBehaviourPunCallbacks
{

    public GameObject roomItem;
    GameObject privatePanel;
    public List<RoomInfo> curRoomList;

    public GameObject showPrivate;
    public GameObject showFullRoom;
    public GameObject showInProgress;

    //private bool showPrivate;
    //public bool ShowPrivate
    //{
    //    get
    //    {
    //        return showPrivate;
    //    }
    //    set
    //    {
    //        showPrivate = value;
    //        displayRoomsInLobby();
    //    }
    //}
    //private bool showFullRoom;
    //public bool ShowFullRoom
    //{
    //    get
    //    {
    //        return showFullRoom;
    //    }
    //    set
    //    {
    //        showFullRoom = value;
    //        displayRoomsInLobby();
    //    }
    //}
    //private bool showInProgress;
    //public bool ShowInProgress
    //{
    //    get
    //    {
    //        return showInProgress;
    //    }
    //    set
    //    {
    //        showInProgress = value;
    //        displayRoomsInLobby();
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        privatePanel = GameObject.Find("Canvas/MultiplayerPanel/Lobby").gameObject;
        privatePanel = privatePanel.transform.Find("PrivateRoomPanel").gameObject;
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (PhotonNetwork.InLobby)
        {
            curRoomList = roomList;
            displayRoomsInLobby();
        }
    }

    public void displayRoomsInLobby()
    {
        foreach (Transform child in transform.Find("Lobby/RoomList/ScrollView/Viewport/Content").transform)
        {
            Destroy(child.gameObject);
        }
        foreach (RoomInfo r in curRoomList)
        {
            if (r.CustomProperties["Privacy"] != null)
            {
                Debug.Log((showPrivate.GetComponent<Toggle>().isOn || (!showPrivate.GetComponent<Toggle>().isOn && !(bool)r.CustomProperties["Privacy"])));
                Debug.Log(((showFullRoom.GetComponent<Toggle>().isOn && (r.PlayerCount <= r.MaxPlayers)) || (!showFullRoom.GetComponent<Toggle>().isOn && (r.PlayerCount >= r.MaxPlayers))));
                Debug.Log((showInProgress.GetComponent<Toggle>().isOn || (!showInProgress.GetComponent<Toggle>().isOn && !(bool)r.CustomProperties["InProgress"])));
                if ((showPrivate.GetComponent<Toggle>().isOn || (!showPrivate.GetComponent<Toggle>().isOn && !(bool)r.CustomProperties["Privacy"]))
                    && ((showFullRoom.GetComponent<Toggle>().isOn && (r.PlayerCount <= r.MaxPlayers)) || (!showFullRoom.GetComponent<Toggle>().isOn && (r.PlayerCount >= r.MaxPlayers))) 
                    && (showInProgress.GetComponent<Toggle>().isOn ||(!showInProgress.GetComponent<Toggle>().isOn && !(bool)r.CustomProperties["InProgress"]) )){
                    instantiateRoomItem(r);
                }
            }
        }
    }

    private void instantiateRoomItem(RoomInfo input)
    {
        GameObject roomItem = Instantiate(this.roomItem, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        roomItem.transform.SetParent(transform.Find("Lobby/RoomList/ScrollView/Viewport/Content").transform, false);

        roomItem.GetComponent<Button>().onClick.RemoveAllListeners();

        if (!(bool)input.CustomProperties["InProgress"])
        {
            if (!(bool)input.CustomProperties["Privacy"])
                roomItem.GetComponent<Button>().onClick.AddListener(() => OnClickConnectToRoom((string)input.Name));
            else
                roomItem.GetComponent<Button>().onClick.AddListener(() => AttemptPrivateGame((string)input.Name, (string)input.CustomProperties["MasterName"], (string)input.CustomProperties["Password"]));
        }
        
        roomItem.GetComponent<RoomListing>().currentRoom = input;
        roomItem.GetComponent<RoomListing>().setRoomName((string)input.CustomProperties["RoomName"]);
        Debug.Log("Master Name: " + (string)input.CustomProperties["MasterName"]);
    }

    public void AttemptPrivateGame(string roomName, string masterName, string password)
    {
        privatePanel.SetActive(true);
        privatePanel.transform.Find("Window/MasterNameLabel").gameObject.GetComponent<Text>().text = masterName;
        privatePanel.transform.Find("Window/JoinRoomBtn").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        privatePanel.transform.Find("Window/JoinRoomBtn").gameObject.GetComponent<Button>().onClick.AddListener(() => CheckPassword(roomName, password));
    }

    public void CheckPassword(string roomName, string password)
    {
        if (privatePanel.transform.Find("Window/InputField").gameObject.GetComponent<InputField>().text == password)
        {
            privatePanel.transform.Find("Window/Invalid").gameObject.SetActive(false);
            privatePanel.SetActive(false);
            OnClickConnectToRoom(roomName);
        }else
        {
            privatePanel.transform.Find("Window/Invalid").gameObject.SetActive(true);
        }

    }

    public void CancelPrivateRoom()
    {
        privatePanel.SetActive(false);
    }

    public void OnClickConnectToRoom(string input)
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        
        PhotonNetwork.JoinRoom(input);
    }
}
