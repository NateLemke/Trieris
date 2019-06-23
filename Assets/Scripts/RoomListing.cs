using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    Text info;
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("RoomInfo").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //r.Name;
        //r.PlayerCount;
        //r.MaxPlayers;
        ////Room Master
        //r.GetPlayer(1);
    }
}
