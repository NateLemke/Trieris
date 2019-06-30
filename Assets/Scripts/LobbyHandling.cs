using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandling : MonoBehaviourPunCallbacks
{

    public GameObject roomItem;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (PhotonNetwork.InLobby)
        {
            foreach (Transform child in transform.Find("Lobby/RoomList/ScrollView/Viewport/Content").transform)
            {
                Destroy(child.gameObject);
            }
            foreach (RoomInfo r in roomList)
            {
                GameObject roomItem = Instantiate(this.roomItem, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                roomItem.transform.SetParent(transform.Find("Lobby/RoomList/ScrollView/Viewport/Content").transform, false);
                roomItem.GetComponent<Button>().onClick.AddListener(() => OnClickConnectToRoom((string) r.Name));
                roomItem.GetComponent<RoomListing>().currentRoom = r;
                roomItem.GetComponent<RoomListing>().setRoomName((string)r.CustomProperties["RoomName"]);
                Debug.Log("Master Name: " + (string)r.CustomProperties["MasterName"]);
            }
        }
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
