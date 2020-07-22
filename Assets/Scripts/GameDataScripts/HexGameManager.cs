using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System;
using TMPro;
using System.Text.RegularExpressions;

public class PlayerInfo //Stats for in match
{
    public string name;
    public string team;
    public int actorNum;
    public int kills;
    public int deaths;
    public int assists;

    public PlayerInfo()
    {
    }

    public PlayerInfo(string teamColor, int actorNo, int _kills, int _deaths, int _assists, string _name)
    {
        team = teamColor;
        actorNum = actorNo;
        kills = _kills;
        deaths = _deaths;
        assists = _assists;
        name = _name;
    }
}

public class MatchStats //track stats
{
    public int wins, losses, draw, hits, misses, damage, captures, playtime;
    public MatchStats()
    {
        wins = losses = draw = hits = misses = damage = captures = playtime = 0;
    }
}

public class HexGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField]
    public Camera gameEnd;
    public string playerPrefab;
    //public Transform spawnPoint;
    public string mapPrefab;
    public bool isMapSpawned;
    //public Transform mapTransform;
    public int myInd;
    public List<PlayerInfo> playerInfo = new List<PlayerInfo>();
    public PlayerInfo myInfo;
    private List<GameObject> hexGrids = new List<GameObject>();
    private bool firstSpawn;
    private float loadTime, matchTime;
    public TextMeshProUGUI gameTimer;
    private bool countdownStart, gameOver;
    private double loadStartTime, gameStartTime;
    private MatchStats matchStats = new MatchStats();
    private PlayfabHandler pf;

    public enum EventCodes : byte
    {
        NewPlayer,
        UpdatePlayers,
        StartMatch,
        UpdateStats,
        UpdateKillfeed
    }

    public void Start()
    {
        loadStartTime = gameStartTime = -1.0;
        loadTime = 1f;
        matchTime = 1f;
        gameTimer.text = fToS(loadTime);
        myInd = -1;
        firstSpawn = true;
        isMapSpawned = false;
        gameStart = countdownStart = gameOver = false;
        SendNewPlayer();
        Spawn();
        pf = GameObject.Find("Authentication").GetComponent<PlayfabHandler>();
    }

    public void Update()
    {
        if (!gameStart)
        {
            if (countdownStart)
            {
                if (loadStartTime < 0.0)
                {
                    loadStartTime = PhotonNetwork.Time;
                }
                if (loadTime > 0f)
                {
                    loadTime -= Time.deltaTime;
                    if (loadTime < 0f)
                    {
                        loadTime = 0f;
                        gameStart = true;
                    }
                    gameTimer.text = fToS(loadTime);
                }
            }
        }
        else
        {
            if (gameStartTime < 0.0)
            {
                gameStartTime = PhotonNetwork.Time;
            }
            if (matchTime > 0f)
            {
                matchTime -= Time.deltaTime;
                if (matchTime < 0f)
                {
                    matchTime = 0f;
                }
                gameTimer.text = fToS(matchTime);
            }
        }

        if (matchTime <= 0f && !gameOver)
        {
            gameOver = true;
            EndGame();
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void Spawn()
    {
        if (PhotonNetwork.IsMasterClient && !isMapSpawned)
        {
            PhotonNetwork.InstantiateSceneObject(mapPrefab, Vector3.zero, Quaternion.identity);
            isMapSpawned = true;
        }
        if (!firstSpawn)
        {
            Vector3 spawn = GetRandomSpawn(myInfo.team);
            string stringValue = Enum.GetName(typeof(GameData.Division), GameData.GetDivision());
            GameObject thePlayer = PhotonNetwork.Instantiate("PlayerDivisions/" + stringValue, spawn, /*spawnPoint.rotation*/Quaternion.identity);
            Vector3 pos = thePlayer.transform.position;
            pos.y += thePlayer.transform.localScale.y;
            thePlayer.transform.position = pos;
        }
    }

    private string AssignTeam()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount % 2 == 0)
        {
            return "Blue";
        }
        else
        {
            return "Red";
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;

        EventCodes evCode = (EventCodes)photonEvent.Code;
        object[] obj = (object[])photonEvent.CustomData;

        switch (evCode)
        {
            case EventCodes.NewPlayer:
                ReceiveNewPlayer(obj);
                break;
            case EventCodes.UpdatePlayers:
                ReceiveUpdatePlayers(obj);
                break;
            case EventCodes.StartMatch:
                ReceiveStartMatch();
                break;
            case EventCodes.UpdateStats:
                ReceiveUpdatedPlayerStats(obj);
                break;
            case EventCodes.UpdateKillfeed:
                ReceiveKillfeedInfo(obj);
                break;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0); //bring back to mainmenu
    }

    public void SendNewPlayer() //Only sends the new player info to masterclient
    {
        object[] package = new object[6];

        myInfo = new PlayerInfo();

        package[0] = myInfo.team = AssignTeam();
        package[1] = myInfo.actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = myInfo.kills = 0;
        package[3] = myInfo.deaths = 0;
        package[4] = myInfo.assists = 0;
        package[5] = myInfo.name = PlayerPrefs.GetString("username");

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }

    public void ReceiveNewPlayer(object[] data) //Only masterclient receives new player
    {
        PlayerInfo newPlayer = new PlayerInfo(
            (string)data[0],
            (int)data[1],
            (int)data[2],
            (int)data[3],
            (int)data[4],
            (string)data[5]
        );

        playerInfo.Add(newPlayer);

        SendUpdatePlayers(playerInfo);
    }

    public void SendUpdatePlayers(List<PlayerInfo> info) //Only masterclient updates everyone with the new info
    {
        object[] package = new object[info.Count];

        for (int i = 0; i < info.Count; i++)
        {
            object[] packet = new object[6];

            packet[0] = info[i].team;
            packet[1] = info[i].actorNum;
            packet[2] = info[i].kills;
            packet[3] = info[i].deaths;
            packet[4] = info[i].assists;
            packet[5] = info[i].name;

            package[i] = packet;
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdatePlayers,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void ReceiveUpdatePlayers(object[] data)
    {
        playerInfo = new List<PlayerInfo>();

        for (int i = 0; i < data.Length; i++)
        {
            object[] packet = (object[])data[i];

            PlayerInfo p = new PlayerInfo(
                (string)packet[0],
                (int)packet[1],
                (int)packet[2],
                (int)packet[3],
                (int)packet[4],
                (string)packet[5]
            );

            playerInfo.Add(p);

            if (PhotonNetwork.LocalPlayer.ActorNumber == p.actorNum)
            {
                myInd = i;
            }
        }

        //If master client receives the last player data, send a start match to everyone
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //Raise event, send to master
            SendStartMatch();
        }
    }

    public void SendStartMatch()
    {
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.StartMatch,
            null,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void ReceiveStartMatch()
    {
        countdownStart = true;
    }

    public void SendUpdatedPlayerStats(int actorNum, int stat, int count)
    {
        object[] package = new object[] { actorNum, stat, count };

        PhotonNetwork.RaiseEvent(
                (byte)EventCodes.UpdateStats,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
    }

    public void ReceiveUpdatedPlayerStats(object[] data)
    {
        int actor = (int)data[0];
        int stat = (int)data[1];
        int count = (int)data[2];

        for (int i = 0; i < playerInfo.Count; i++)
        {
            if (playerInfo[i].actorNum == actor)
            {
                switch (stat)
                {
                    case 0:
                        playerInfo[i].kills += count;
                        break;
                    case 1:
                        playerInfo[i].deaths += count;
                        break;
                    case 2:
                        playerInfo[i].assists += count;
                        break;
                }

                if (actor == myInfo.actorNum)
                {
                    myInfo.kills = playerInfo[i].kills;
                    myInfo.deaths = playerInfo[i].deaths;
                    myInfo.assists = playerInfo[i].assists;
                }

                break;
            }
        }
    }

    public void SendKillfeedInfo(int killerActor, int killedActor)
    {
        object[] package = new object[] { killerActor, killedActor };

        PhotonNetwork.RaiseEvent(
                (byte)EventCodes.UpdateKillfeed,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
    }

    public void ReceiveKillfeedInfo(object[] data)
    {
        int killerActor = (int)data[0];
        int killedActor = (int)data[1];

        string killer, killed;
        killer = killed = "";
        for (int i = 0; i < playerInfo.Count; i++)
        {
            if (playerInfo[i].actorNum == killerActor)
            {
                killer = playerInfo[i].name;
            }

            if (playerInfo[i].actorNum == killedActor)
            {
                killed = playerInfo[i].name;
            }

            if (killed != "" && killer != "")
            {
                break;
            }
        }

        GameObject killfeed = GameObject.Find("Killfeed");
        Transform content = killfeed.transform.Find("Scroll View/Viewport/Content");
        content.GetComponent<KillfeedEdit>().AddtoKillfeed(killer, killed);
    }

    private void EndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.DestroyAll();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer); //Destroy localplayer
        GameObject.Find("HUD").SetActive(false); //Off the HUD
        gameEnd.gameObject.SetActive(true); //On the camera
        Vector2 results = CheckWinLose();
        AddStats();
        GameObject.Find("EndgameUI").GetComponent<EndgameUI>().StartAnim(results.x, results.y);
        StartCoroutine(End(5f));
    }

    public MatchStats GetStatTracker()
    {
        return matchStats;
    }

    private void AddStats()
    {
        matchStats.playtime = 120;
        pf.SetStats(myInfo.kills, myInfo.deaths, myInfo.assists, matchStats.wins, matchStats.losses, matchStats.draw, matchStats.hits, matchStats.misses, matchStats.damage, matchStats.captures, matchStats.playtime);
    }

    private IEnumerator End(float p_wait)
    {
        yield return new WaitForSeconds(p_wait);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public string GetLocalPlayerTeam()
    {
        return myInfo.team;
    }

    public void UpdateGridInfo(List<GameObject> hGrids)
    {
        hexGrids = hGrids;
        if (firstSpawn)
        {
            int index = -1;
            if (PhotonNetwork.CurrentRoom.PlayerCount % 2 == 1)
                index = (PhotonNetwork.CurrentRoom.PlayerCount - 1) / 2;
            else
                index = hexGrids.Count - PhotonNetwork.CurrentRoom.PlayerCount / 2;

            Vector3 spawn = IndexToHexVec3(index);
            string stringValue = Enum.GetName(typeof(GameData.Division), GameData.GetDivision());
            GameObject thePlayer = PhotonNetwork.Instantiate("PlayerDivisions/" + stringValue, spawn, /*spawnPoint.rotation*/Quaternion.identity);
            Vector3 pos = thePlayer.transform.position;
            pos.y = thePlayer.transform.localScale.y + 0.1f;
            thePlayer.transform.position = pos;
            firstSpawn = false;
        }
    }

    public Vector3 IndexToHexVec3(int index)
    {
        return hexGrids[index].transform.position;
    }

    public Vector3 GetRandomSpawn(string teamColor)
    {
        List<int> possInds = new List<int>();
        for (int x = 0; x < hexGrids.Count; ++x)
        {
            Transform myMat = hexGrids[x].transform.GetChild(0);
            Material myMatMesh = myMat.GetComponent<MeshRenderer>().material;
            if (!hexGrids[x].GetComponent<TileInfo>().active)
                continue;
            if (teamColor == "Red")
            {
                if (myMatMesh.name != "Blue (Instance)")
                {
                    possInds.Add(x);
                }
            }
            else if (teamColor == "Blue")
            {
                if (myMatMesh.name != "Red (Instance)")
                {
                    possInds.Add(x);
                }
            }
        }

        if (possInds.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, possInds.Count);
            Vector3 pos = hexGrids[possInds[rand]].transform.position;
            pos.y = hexGrids[possInds[rand]].transform.localScale.y / 15f * 1.1f + 0.1f;
            return pos;
        }

        return Vector3.zero;
    }

    private Vector2 CheckWinLose()
    {
        int red, blue;
        red = blue = 0;
        for (int x = 0; x < hexGrids.Count; ++x)
        {
            Transform myMat = hexGrids[x].transform.GetChild(0);
            Material myMatMesh = myMat.GetComponent<MeshRenderer>().material;
            
            if (myMatMesh.name == "Blue (Instance)")
            {
                blue++;
            }
            if (myMatMesh.name == "Red (Instance)")
            {
                red++;
            }
        }

        //if (red > blue)
        //{
        //    return "Red";
        //}
        //else if (blue > red)
        //{
        //    return "Blue";
        //}

        if (red > blue)
        {
            if (myInfo.team == "Red")
            {
                matchStats.wins = 1;
            }
            else
            {
                matchStats.losses = 1;
            }
        }
        else if (red == blue)
        {
            matchStats.draw = 1;
        }
        else if (blue > red)
        {
            if (myInfo.team == "Blue")
            {
                matchStats.wins = 1;
            }
            else
            {
                matchStats.losses = 1;
            }
        }

        //return "Draw";
        return new Vector2(red, blue);
    }

    public bool gameStart
    {
        get;
        set;
    }

    private string fToS(float time)
    {
        int front = (int)time / 60;
        int back = (int)time % 60;
        string tempB = back.ToString("F2");
        string newB = "";
        for (int x = 0; x < tempB.Length; ++x)
        {
            if (x > 1)
                newB += tempB[x];
        }
        return front + ":" + (back < 10 ? "0" : "") + back;
    }

    public List<PlayerInfo> GetPlayers()
    {
        return playerInfo;
    }
}
