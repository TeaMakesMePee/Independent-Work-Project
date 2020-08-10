using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEngine.Audio;

/*
 * This script manages all the tabs within the main menu and the transition to the game scene
 * It also handles the connection to server and lobby here
*/

public class HexGameLauncher : MonoBehaviourPunCallbacks
{
    public GameObject MainMenuTab, RoomsTab, RoomsButton, CreateTab, DivisionsTab, theTitle, videoTab, statTab, quitTab, settingsTab;
    public List<GameObject> divButtons;
    private List<RoomInfo> roomList;
    public AudioMixer am;
    public Sprite tank, dmg, flank;

    public TextMeshProUGUI roomName;
    private string selectedRoom = null;

    public GameObject errorBox, authBox;
    public GameObject joinError, roomBox;
    public GameObject createError, createBox;
    public Slider teamSizeSlider;

    //public Animator anim;

    public void Awake()
    {
        //On awake, connects
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sfx"))
        {
            am.SetFloat("volume", PlayerPrefs.GetFloat("Sfx"));
        }
        FindObjectOfType<AudioManager>().Play("MenuTheme");
        //if (PlayerPrefs.HasKey("username"))
        //MainMenuTab.transform.Find("Profile/DisplayName").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("username");
        MainMenuTab.transform.Find("Profile/DisplayName").GetComponent<TextMeshProUGUI>().text = GameData.playerName;
    }

    public override void OnConnectedToMaster()
    {
        //Once connected, join
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
        if (GameData.GetDivision() == GameData.Division.P_None)
        {
            createBox.SetActive(false);
            createError.SetActive(true);
        }
        else
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = /*1*/(byte)((int)teamSizeSlider.value * 2);

            PhotonNetwork.CreateRoom(roomName.text, options);
            FindObjectOfType<AudioManager>().Play("ButtonClick");
        }
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
        quitTab.SetActive(false);
        settingsTab.SetActive(false);
        FindObjectOfType<AudioManager>().Play("ButtonClick");
    }

    public void OpenQuitTab()
    {
        CloseAllTabs();
        quitTab.SetActive(true);
    }

    public void OpenSettingsTab()
    {
        CloseAllTabs();
        settingsTab.SetActive(true);
    }

    public void OpenStatsTab()
    {
        CloseAllTabs();
        UpdateStats();
        statTab.SetActive(true);
        TriggerExpBar();
    }

    public void OpenRoomsTab()
    {
        CloseAllTabs();
        RoomsTab.SetActive(true);
        //RoomsTab.transform.Find("SelectBackground/ErrorMessage").gameObject.SetActive(GameData.GetDivision() == GameData.Division.P_None ? true : false);
    }

    public void OpenMainMenuTab()
    {
        CloseAllTabs();
        theTitle.SetActive(true);
        MainMenuTab.SetActive(true);
    }

    public void CloseStatsTab()
    {
        CloseAllTabs();
        statTab.transform.Find("TopContainer/Experience/ExperienceBar").GetComponent<ExpBarScript>().ResetExpStat();
        theTitle.SetActive(true);
        MainMenuTab.SetActive(true);
    }

    public void OpenCreateTab()
    {
        CloseAllTabs();
        CreateTab.SetActive(true);
        //CreateTab.transform.Find("ErrorMessage").gameObject.SetActive(GameData.GetDivision() == GameData.Division.P_None ? true : false);
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
        GameObject theAnchor2 = MainMenuTab.transform.GetChild(1).gameObject;
        for (int x = 0; x < theAnchor2.transform.childCount; ++x)
            theAnchor2.transform.GetChild(x).GetComponent<ButtonText>().ResetButton();
    }

    private void ClearRoomList() //Clears all the room list in prep for updating the list
    {
        Transform content = RoomsTab.transform.Find("List/Scroll View/Viewport/Content");
        foreach (Transform roomButt in content) Destroy(roomButt.gameObject);
    }

    public override void OnRoomListUpdate(List<RoomInfo> list)
    {
        roomList = list;
        ClearRoomList();

        Transform content = RoomsTab.transform.Find("List/Scroll View/Viewport/Content");

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
        if (GameData.GetDivision() == GameData.Division.P_None)
        {
            roomBox.SetActive(false);
            joinError.SetActive(true);
        }
        else
        {
            if (selectedRoom != null)
            {
                PhotonNetwork.JoinRoom(selectedRoom);
                FindObjectOfType<AudioManager>().Play("ButtonClick");
            }
        }
    }

    public void CloseJoinError()
    {
        roomBox.SetActive(true);
        joinError.SetActive(false);
    }

    public void CloseCreateError()
    {
        createBox.SetActive(true);
        createError.SetActive(false);
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
        MainMenuTab.transform.Find("Profile/ProfIcon").GetComponent<Image>().sprite = tank;
        OpenMainMenuTab();
    }

    public void SetDamage()
    {
        GameData.SetDivision(GameData.Division.P_Damage);
        MainMenuTab.transform.Find("Profile/ProfIcon").GetComponent<Image>().sprite = dmg;
        OpenMainMenuTab();
    }

    public void SetFlank()
    {
        GameData.SetDivision(GameData.Division.P_Flank);
        MainMenuTab.transform.Find("Profile/ProfIcon").GetComponent<Image>().sprite = flank;
        OpenMainMenuTab();
    }

    public void ToggleAuthError(string error)
    {
        authBox.SetActive(false);
        errorBox.SetActive(true);
        errorBox.transform.Find("ErrorMessage").GetComponent<TextMeshProUGUI>().text = error;
    }

    public void ToggleAuthBox()
    {
        authBox.SetActive(true);
        authBox.transform.Find("Auth_User").GetComponent<TMP_InputField>().text = "";
        authBox.transform.Find("Auth_Pass").GetComponent<TMP_InputField>().text = "";
        errorBox.SetActive(false);
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

    public void UpdateStats()
    {
        Transform generalStat = statTab.transform.Find("TopContainer/GeneralStat");
        Transform levelStat = statTab.transform.Find("TopContainer/LevelStat");
        Transform expStat = statTab.transform.Find("TopContainer/Experience");

        string timeText = ((float)(GameData.pStat.playtime / (60 * 60)) >= 1f ? "hours" : "mins");
        int minutes = GameData.pStat.playtime / 60;
        int timeCount = minutes;
        if (minutes > 999)
            timeCount = minutes / 60;

        int level = 1;
        int currExp = 10000;
        int remainingExp = GameData.pStat.experience;
        for (int x = GameData.pStat.experience; x > currExp;)
        {
            level++;
            x -= currExp;
            if (currExp < 100000)
                currExp += 10000;
            remainingExp = x;
        }

        expStat.transform.Find("ExpToLevel").GetComponent<TextMeshProUGUI>().text = remainingExp.ToString() + "/" + currExp.ToString();

        levelStat.transform.Find("LevelImg").transform.Find("TheLevel").GetComponent<TextMeshProUGUI>().text = level.ToString();

        levelStat.transform.Find("DisplayName").GetComponent<TextMeshProUGUI>().text = /*PlayerPrefs.GetString("username")*/GameData.playerName;

        generalStat.transform.Find("TimePlayed/Time").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", timeCount) + " " + timeText;

        generalStat.transform.Find("TotalWins/Wins").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", GameData.pStat.totalWins);

        statTab.transform.Find("BottomContainer/theStats/Kills/MostKills").GetComponent<TextMeshProUGUI>().text = string.Format("{0:n0}", GameData.pStat.mostKills);

        float atKills = GameData.pStat.totalKills;
        float atWins = GameData.pStat.totalWins;
        float atLosses = GameData.pStat.totalLosses;
        float atDraws = GameData.pStat.totalDraws;
        float avgKills = (atKills > 0 ? atKills / (atWins + atLosses + atDraws) : 0f);
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

        float tHits = GameData.pStat.totalHits;
        float tMisses = GameData.pStat.totalMisses;
        float pAcc = tHits > 0f ? (tHits / (tHits + tMisses)) : 0f;
        int acc = (int)(pAcc * 100f);
        statTab.transform.Find("BottomContainer/theStats/Accuracy/AccuracyPercent").GetComponent<TextMeshProUGUI>().text = acc.ToString() + "%";

        float tDeaths = GameData.pStat.totalDeaths;
        float kda = (atKills > 0 ? atKills / tDeaths : 0f);
        statTab.transform.Find("BottomContainer/theStats/KDA/KillDeathRatio").GetComponent<TextMeshProUGUI>().text = kda.ToString("0.0");

        float pWinr = (atWins > 0 ? atWins / (atWins + atLosses + atDraws) : 0f);
        int winr = (int)(pWinr * 100f);
        statTab.transform.Find("BottomContainer/theStats/Winrate/WinratePercent").GetComponent<TextMeshProUGUI>().text = winr.ToString() + "%";        
    }

    private void TriggerExpBar()
    {
        int currExp = 10000;
        int remainingExp = GameData.pStat.experience;
        for (int x = GameData.pStat.experience; x > currExp;)
        {
            x -= currExp;
            if (currExp < 100000)
                currExp += 10000;
            remainingExp = x;
        }
        statTab.transform.Find("TopContainer/Experience/ExperienceBar").GetComponent<ExpBarScript>().SetExpStat(remainingExp, currExp);
    }
}
