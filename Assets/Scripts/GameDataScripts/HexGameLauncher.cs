using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class HexGameLauncher : MonoBehaviourPunCallbacks
{
    public GameObject MainMenuTab, RoomsTab, RoomsButton, CreateTab, DivisionsTab, theTitle;
    private List<RoomInfo> roomList;

    public TextMeshProUGUI roomName;
    private string selectedRoom = null;

    public void Awake()
    {
        //On awake, connects
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("MenuTheme");
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
        options.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomName.text, options);
        FindObjectOfType<AudioManager>().Play("ButtonClick");
    }

    private void CloseAllTabs()
    {
        ResetMainMenu();
        MainMenuTab.SetActive(false);
        CreateTab.SetActive(false);
        RoomsTab.SetActive(false);
        DivisionsTab.SetActive(false);
        theTitle.SetActive(false);
        FindObjectOfType<AudioManager>().Play("ButtonClick");
    }

    public void OpenRoomsTab()
    {
        CloseAllTabs();
        RoomsTab.SetActive(true);
    }

    public void OpenMainMenuTab()
    {
        CloseAllTabs();
        theTitle.SetActive(true);
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

    public void ResetMainMenu()
    {
        GameObject theAnchor = MainMenuTab.transform.GetChild(0).gameObject;
        for (int x = 0; x < theAnchor.transform.childCount; ++x)
            theAnchor.transform.GetChild(x).GetComponent<ButtonText>().ResetButton();
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

            newRoomButton.transform.Find("RoomName").GetComponent<TextMeshProUGUI>().text = room.Name;
            newRoomButton.transform.Find("RoomCount").GetComponent<TextMeshProUGUI>().text = room.PlayerCount + " / " + room.MaxPlayers;

            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { SetRoom(newRoomButton.transform); });
        }

        base.OnRoomListUpdate(roomList);
    }

    public void SetRoom(Transform button)
    {
        selectedRoom = button.transform.Find("RoomName").GetComponent<TextMeshProUGUI>().text;
    }

    public void JoinRoom()
    {
        if (selectedRoom != null)
        {
            PhotonNetwork.JoinRoom(selectedRoom);
            FindObjectOfType<AudioManager>().Play("ButtonClick");
        }
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
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
