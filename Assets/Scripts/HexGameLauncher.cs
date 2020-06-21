using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class HexGameLauncher : MonoBehaviourPunCallbacks
{
    public GameObject MainMenuTab, RoomsTab, RoomsButton, CreateTab, DivisionsTab;
    private List<RoomInfo> roomList;

    public InputField roomName;

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
        PhotonNetwork.JoinLobby();
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
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 1;

        PhotonNetwork.CreateRoom(roomName.text, options);
    }

    private void CloseAllTabs()
    {
        MainMenuTab.SetActive(false);
        CreateTab.SetActive(false);
        RoomsTab.SetActive(false);
        DivisionsTab.SetActive(false);
    }

    public void OpenRoomsTab()
    {
        CloseAllTabs();
        RoomsTab.SetActive(true);
    }

    public void OpenMainMenuTab()
    {
        CloseAllTabs();
        MainMenuTab.SetActive(true);
    }

    public void OpenCreateTab()
    {
        CloseAllTabs();
        CreateTab.SetActive(true);
    }

    public void OpenDivisionsTab()
    {
        CloseAllTabs();
        DivisionsTab.SetActive(true);
    }

    private void ClearRoomList() //Clears all the room list in prep for updating the list
    {
        Transform content = RoomsTab.transform.Find("Scroll View/Viewport/Content");
        foreach (Transform roomButt in content) Destroy(roomButt.gameObject);
    }

    public override void OnRoomListUpdate(List<RoomInfo> list)
    {
        roomList = list;
        ClearRoomList();

        Transform content = RoomsTab.transform.Find("Scroll View/Viewport/Content");

        foreach (RoomInfo room in roomList)
        {
            GameObject newRoomButton = Instantiate(RoomsButton, content) as GameObject;

            newRoomButton.transform.Find("RoomName").GetComponent<Text>().text = room.Name;
            newRoomButton.transform.Find("RoomCount").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;

            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton.transform); });
        }

        base.OnRoomListUpdate(roomList);
    }

    public void JoinRoom(Transform button)
    {
        PhotonNetwork.JoinRoom(button.transform.Find("RoomName").GetComponent<Text>().text);
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void SetTank()
    {
        GameData.SetDivision(GameData.Division.P_Tank);
        OpenMainMenuTab();
    }

    public void SetDamage()
    {
        GameData.SetDivision(GameData.Division.P_Damage);
        OpenMainMenuTab();
    }

    public void SetFlank()
    {
        GameData.SetDivision(GameData.Division.P_Flank);
        OpenMainMenuTab();
    }

    private void Update()
    {
        
    }
}
