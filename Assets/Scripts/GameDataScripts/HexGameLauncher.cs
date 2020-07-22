﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class HexGameLauncher : MonoBehaviourPunCallbacks
{
    public GameObject MainMenuTab, RoomsTab, RoomsButton, CreateTab, DivisionsTab, theTitle, videoTab, statTab;
    public List<GameObject> divButtons;
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
        options.MaxPlayers = 1;

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
        statTab.SetActive(false);
        FindObjectOfType<AudioManager>().Play("ButtonClick");
    }

    public void OpenStatsTab()
    {
        CloseAllTabs();
        UpdateStats();
        statTab.SetActive(true);
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

    public void ToggleVideo()
    {
        videoTab.SetActive(!videoTab.activeSelf);
    }

    public void ToggleDivisionButtons()
    {
        for (int d = 0; d < divButtons.Count; ++d)
        {
            divButtons[d].SetActive(!divButtons[d].activeSelf);
        }
    }

    public void TankPreview()
    {
        ToggleDivisionButtons();
        ToggleVideo();
        videoTab.transform.Find("DivisionVideo").GetComponent<VideoScript>().PlayVideo(GameData.Division.P_Tank);
    }

    public void DamagePreview()
    {
        ToggleDivisionButtons();
        ToggleVideo();
        videoTab.transform.Find("DivisionVideo").GetComponent<VideoScript>().PlayVideo(GameData.Division.P_Damage);
    }

    public void FlankPreview()
    {
        ToggleDivisionButtons();
        ToggleVideo();
        videoTab.transform.Find("DivisionVideo").GetComponent<VideoScript>().PlayVideo(GameData.Division.P_Flank);
    }

    public void ClosePreview()
    {
        videoTab.transform.Find("DivisionVideo").GetComponent<VideoScript>().StopVideo();
        ToggleVideo();
        ToggleDivisionButtons();
    }

    private void UpdateStats()
    {
        Transform generalStat = statTab.transform.Find("TopContainer/GeneralStat");
        Transform levelStat = statTab.transform.Find("TopContainer/LevelStat");

        string timeText = ((float)(GameData.pStat.playtime / (60 * 60)) >= 1f ? "hours" : "mins");
        int minutes = GameData.pStat.playtime / 60;
        int timeCount = minutes;
        if (minutes > 999)
            timeCount = minutes / 60;

        generalStat.transform.Find("TimePlayed/Time").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", timeCount) + " " + timeText;

        generalStat.transform.Find("TotalWins/Wins").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", GameData.pStat.totalWins);

        levelStat.transform.Find("DisplayName").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("username");

        statTab.transform.Find("BottomContainer/theStats/Kills/MostKills").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", GameData.pStat.mostKills);

        float avgKills = (GameData.pStat.totalKills > 0 ? GameData.pStat.totalKills / (GameData.pStat.totalWins + GameData.pStat.totalLosses + GameData.pStat.totalDraws) : 0f);
        statTab.transform.Find("BottomContainer/theStats/Kills/AverageKills").GetComponent<TextMeshProUGUI>().text = "average: " + string.Format("{0:#,##0.##}", avgKills);

        statTab.transform.Find("BottomContainer/theStats/Kills/TotalKills").GetComponent<TextMeshProUGUI>().text = "total: " + string.Format("{0:n0}", GameData.pStat.totalKills);

        statTab.transform.Find("BottomContainer/theStats/Damage/MostDamage").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", GameData.pStat.mostDamage);

        int avgDmg = (GameData.pStat.totalDamage > 0 ? GameData.pStat.totalDamage / (GameData.pStat.totalWins + GameData.pStat.totalLosses + GameData.pStat.totalDraws) : 0);
        statTab.transform.Find("BottomContainer/theStats/Damage/AverageDamage").GetComponent<TextMeshProUGUI>().text = "average: " + string.Format("{0:n0}", avgDmg);

        statTab.transform.Find("BottomContainer/theStats/Damage/TotalDamage").GetComponent<TextMeshProUGUI>().text = "total: " + string.Format("{0:n0}", GameData.pStat.totalDamage);

        statTab.transform.Find("BottomContainer/theStats/Captured/MostCaptured").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", GameData.pStat.mostCaptures);

        int avgCapture = (GameData.pStat.totalCaptures > 0 ? GameData.pStat.totalCaptures / (GameData.pStat.totalWins + GameData.pStat.totalLosses + GameData.pStat.totalDraws) : 0);
        statTab.transform.Find("BottomContainer/theStats/Captured/AverageCapture").GetComponent<TextMeshProUGUI>().text = "average: " + string.Format("{0:n0}", avgCapture);

        statTab.transform.Find("BottomContainer/theStats/Captured/TotalCapture").GetComponent<TextMeshProUGUI>().text = "total: " + string.Format("{0:n0}", GameData.pStat.totalCaptures);

        float pAcc = (GameData.pStat.totalHits > 0 ? GameData.pStat.totalHits / (GameData.pStat.totalHits + GameData.pStat.totalMisses) : 0f);
        int acc = (int)(pAcc * 100f);
        statTab.transform.Find("BottomContainer/theStats/Accuracy/AccuracyPercent").GetComponent<TextMeshProUGUI>().text = acc.ToString() + "%";

        float kda = (GameData.pStat.totalKills > 0 ? GameData.pStat.totalKills / GameData.pStat.totalDeaths : 0f);
        statTab.transform.Find("BottomContainer/theStats/KDA/KillDeathRatio").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", kda);

        float pWinr = (GameData.pStat.totalWins > 0 ? GameData.pStat.totalWins / (GameData.pStat.totalWins + GameData.pStat.totalLosses + GameData.pStat.totalDraws) : 0f);
        int winr = (int)(pWinr * 100f);
        statTab.transform.Find("BottomContainer/theStats/Winrate/WinratePercent").GetComponent<TextMeshProUGUI>().text = winr.ToString() + "%";
    }
}
