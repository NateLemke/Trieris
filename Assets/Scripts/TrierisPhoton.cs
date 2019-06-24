using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrierisPhoton : MonoBehaviour, IPunObservable {

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void PhotonMessage() {
        PhotonView pv = PhotonView.Get(this);
        pv.RPC("LogMessage",RpcTarget.MasterClient);
    }

    public void LogMessage() {
        Debug.Log("Photon Message Received!");
    }

    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info) {
        if (stream.IsWriting) {

        } else {

        }
    }
}