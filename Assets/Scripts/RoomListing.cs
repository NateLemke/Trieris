using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviourPunCallbacks
{
    //Text info;

    //Text name;
    Text creator;
    Text slots;
    Text privacy;
    Text inProgress;

    public string roomName;

    public RoomInfo currentRoom;
    // Start is called before the first frame update
    void Start()
    {
        //info = transform.Find("RoomInfo").GetComponent<Text>();
        //name = transform.Find("Name/Text").GetComponent<Text>();
        creator = transform.Find("Creator/Text").GetComponent<Text>();
        slots = transform.Find("Slots/Text").GetComponent<Text>();
        privacy = transform.Find("Privacy/Text").GetComponent<Text>();
        inProgress = transform.Find("InProgress/Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentRoom != null)
        {
            //name.text = currentRoom.Name;
            creator.text = (string)currentRoom.CustomProperties["MasterName"];
            slots.text = currentRoom.PlayerCount + " / " + currentRoom.MaxPlayers;
            privacy.text = (bool) currentRoom.CustomProperties["Privacy"] ? "Private" : "Public";
            inProgress.text = (bool) currentRoom.CustomProperties["InProgress"] ? "In Game" : "In Lobby";
        }
        else
        {
            Debug.Log("room not set");
        }
    }

    public void setRoomName(string input)
    {
        roomName = input;
        Debug.Log("name set");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo r in roomList)
        {
            if(r.Name == roomName)
            {
                Debug.Log("room found");
                currentRoom = r;
                break;
            }
            else
            {
                Debug.Log("room not found");
            }
        }
    }
}
