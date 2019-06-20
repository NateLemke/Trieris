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
            for(int i = 1; i < 6; i++)
            {
                thisRoom.transform.Find("Teams/Team" + i + "/InformationPanel/Name/Text").GetComponent<Text>().text = "Empty";
            }
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                thisRoom.transform.Find("Teams/Team" + getRealId(p) + "/InformationPanel/Name/Text").GetComponent<Text>().text = p.NickName;
            }
        }
    }

    private int getRealId(Player inPlayer)
    {
        List<int> tempPlayerList = new List<int>();
        if (PhotonNetwork.IsConnected)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                tempPlayerList.Add(p.ActorNumber);
            }
        }
        return tempPlayerList.IndexOf(inPlayer.ActorNumber) + 1;
    }
}
