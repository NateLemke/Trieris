using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomHandling : MonoBehaviour
{
    GameObject thisRoom;
    // Start is called before the first frame update
    void Start()
    {
        thisRoom = GameObject.Find("Canvas").gameObject;
        thisRoom = thisRoom.transform.Find("MultiplayerPanel/RoomPanel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                thisRoom.transform.Find("Teams/Team" + p.ActorNumber + "/InformationPanel/Name/Text").GetComponent<Text>().text = p.NickName;
            }
        }
    }
}
