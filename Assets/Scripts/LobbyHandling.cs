using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandling : MonoBehaviourPunCallbacks
{

    public GameObject roomItem;
    List<RoomInfo> curRoomList;
    GameObject privatePanel;
    // Start is called before the first frame update
    void Start()
    {
        privatePanel = GameObject.Find("Canvas/MultiplayerPanel/Lobby").gameObject;
        privatePanel = privatePanel.transform.Find("PrivateRoomPanel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InLobby)
        {
            foreach (Transform child in transform.Find("Lobby/RoomList/ScrollView/Viewport/Content").transform)
            {
                Destroy(child.gameObject);
            }
            foreach (RoomInfo r in curRoomList)
            {
                GameObject roomItem = Instantiate(this.roomItem, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                roomItem.transform.SetParent(transform.Find("Lobby/RoomList/ScrollView/Viewport/Content").transform, false);

                roomItem.GetComponent<Button>().onClick.RemoveAllListeners();
                if (!(bool)r.CustomProperties["Privacy"])
                    roomItem.GetComponent<Button>().onClick.AddListener(() => OnClickConnectToRoom((string)r.Name));
                else
                {
                    roomItem.GetComponent<Button>().onClick.AddListener(() => AttemptPrivateGame((string)r.Name, (string)r.CustomProperties["MasterName"], (string)r.CustomProperties["Password"]));
                }

                roomItem.GetComponent<RoomListing>().currentRoom = r;
                roomItem.GetComponent<RoomListing>().setRoomName((string)r.CustomProperties["RoomName"]);
                Debug.Log("Master Name: " + (string)r.CustomProperties["MasterName"]);
            }
        }
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        curRoomList = roomList;
        
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
            OnClickConnectToRoom(roomName);
        else
        {
            privatePanel.transform.Find("Window/InvalidPassPanel").gameObject.SetActive(true);
        }

    }

    public void CancelPrivateRoom()
    {
        privatePanel.SetActive(false);
    }

    public void CloseInvalidPassPanel()
    {
        privatePanel.transform.Find("Window/InvalidPassPanel").gameObject.SetActive(false);
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
