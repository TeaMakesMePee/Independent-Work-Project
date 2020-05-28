using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class HexGameLauncher : MonoBehaviourPunCallbacks
{
    public void Awake()
    {
        //On awake, connects
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    public override void OnConnectedToMaster()
    {
        //Once connected, join
        //Join();

        base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom()
    {
        //Start game once joined
        StartGame();

        base.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create();

        base.OnJoinRandomFailed(returnCode, message);
    }

    public void Connect()
    {
        PhotonNetwork.GameVersion = "0.0.0";
        //Connects
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Join()
    {
        //Join random room
        PhotonNetwork.JoinRandomRoom();
    }

    public void Create()
    {
        PhotonNetwork.CreateRoom("");
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
