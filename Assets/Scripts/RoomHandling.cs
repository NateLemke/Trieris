using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomHandling : MonoBehaviourPunCallbacks
{
    public GameObject thisRoom;
    public GameObject privateGame;


    // Start is called before the first frame update
    void Awake()
    {
        thisRoom = GameObject.Find("Canvas").gameObject;
        thisRoom = thisRoom.transform.Find("MultiplayerPanel/RoomPanel").gameObject;
    }

    void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i <= 6; i++)
            {
                GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value = i - 1;
            }

            Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
            for(int i = 1; i<=6; i++)
            {
                Debug.Log("Team" + i + "Int set to " + (int)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value);
                ht["Team" + i + "Int"] = (int) GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value;
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }
        else
        {
            for (int i = 1; i <= 6; i++)
            {
                Debug.Log("Image set to " + (int)GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value + " for Team" + i);
                GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + i + "/TeamImage/Dropdown").GetComponent<Dropdown>().value = (int)PhotonNetwork.CurrentRoom.CustomProperties["Team" + i + "Int"];
            }
        }
        UpdatePlayerList();
        setRoomName();
        setLocalPlayerTeam();
        privateGame.GetComponent<Toggle>().isOn = (bool)PhotonNetwork.CurrentRoom.CustomProperties["Privacy"];
    }

    public void UpdatePlayerList()
    {
        for (int i = 1; i <= 6; i++)
        {
            thisRoom.transform.Find("Teams/Team" + i + "/InformationPanel/Name/Text").GetComponent<Text>().text = "Empty";
            thisRoom.transform.Find("Teams/Team" + i + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value = 0;
        }
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            thisRoom.transform.Find("Teams/Team" + getSlotPosition(p) + "/InformationPanel/Name/Text").GetComponent<Text>().text = p.NickName;
            thisRoom.transform.Find("Teams/Team" + getSlotPosition(p) + "/InformationPanel/Dropdown").GetComponent<Dropdown>().value = 1;
        }
    }

    public void setRoomPrivacy()
    {
        Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        ht["Privacy"] = privateGame.GetComponent<Toggle>().isOn;
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    }

    public void setRoomName()
    {
        thisRoom.transform.Find("RoomName/InputField");
        thisRoom.transform.Find("RoomName/InputField").gameObject.GetComponent<InputField>().text = PhotonNetwork.CurrentRoom.Name;
    }

    public int getSlotPosition(Player inPlayer)
    {
        return orderedActorNumbers().IndexOf(inPlayer.ActorNumber) + 1;
    }

    public List<int> orderedActorNumbers()
    {
        List<int> tempPlayerList = new List<int>();
        if (PhotonNetwork.IsConnected)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                tempPlayerList.Add(p.ActorNumber);
            }
        }
        tempPlayerList.Sort();
        return tempPlayerList;
    }

    public Player playerInSlot(int slotNumber)
    {
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            if (orderedActorNumbers()[slotNumber-1] == p.ActorNumber)
                return p;
        }
        return PhotonNetwork.LocalPlayer;
    }

    public void setPlayerTeam(int slotNumber)
    {
        Hashtable customProperties = playerInSlot(slotNumber).CustomProperties;
        Dropdown thisDropdown = GameObject.Find("Canvas/MultiplayerPanel/RoomPanel/Teams/Team" + slotNumber + "/TeamImage/Dropdown").GetComponent<Dropdown>();
        customProperties["TeamNum"] = thisDropdown.value;
        customProperties["LoadedGame"] = false;
        playerInSlot(slotNumber).SetCustomProperties(customProperties);
        Debug.Log("Player " + slotNumber + ", team changed to " + playerInSlot(slotNumber).CustomProperties["TeamNum"]);
    }

    [PunRPC]
    public void setLocalPlayerTeam()
    {
        Debug.Log(getSlotPosition(PhotonNetwork.LocalPlayer));
        setPlayerTeam(getSlotPosition(PhotonNetwork.LocalPlayer));
    }

}
