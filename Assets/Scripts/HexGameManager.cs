using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System;

public struct PlayerInfo
{
    public string team;
    public int actorNum;

    public PlayerInfo(string teamColor, int actorNo)
    {
        team = teamColor;
        actorNum = actorNo;
    }
}

public class HexGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public string playerPrefab;
    public Transform spawnPoint;
    public string mapPrefab;
    public bool isMapSpawned;
    //public Transform mapTransform;
    public int myInd;
    public List<PlayerInfo> playerInfo = new List<PlayerInfo>();
    public PlayerInfo myInfo;
    private List<GameObject> hexGrids = new List<GameObject>();
    private bool firstSpawn;
    private float loadTime, matchTime;
    public Text gameTimer;
    private bool countdownStart, gameOver;
    private double loadStartTime, gameStartTime;

    public enum EventCodes : byte
    {
        NewPlayer,
        UpdatePlayers,
        StartMatch,
    }

    public void Start()
    {
        loadStartTime = gameStartTime = -1.0;
        loadTime = 5f;
        matchTime = 120f;
        gameTimer.text = fToS(loadTime);
        myInd = -1;
        firstSpawn = true;
        isMapSpawned = false;
        gameStart = countdownStart = gameOver = false;
        SendNewPlayer();
        Spawn();
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
            GameObject thePlayer = PhotonNetwork.Instantiate("PlayerDivisions/" + stringValue, spawn, spawnPoint.rotation);
            Vector3 pos = thePlayer.transform.position;
            pos.y = thePlayer.transform.localScale.y + 0.1f;
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
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0); //bring back to mainmenu
    }

    public void SendNewPlayer()
    {
        object[] package = new object[2];

        myInfo = new PlayerInfo();

        package[0] = myInfo.team = AssignTeam();
        package[1] = myInfo.actorNum = PhotonNetwork.LocalPlayer.ActorNumber;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }

    public void ReceiveNewPlayer(object[] data)
    {
        PlayerInfo newPlayer = new PlayerInfo(
            (string)data[0],
            (int)data[1]
        );

        playerInfo.Add(newPlayer);

        SendUpdatePlayers(playerInfo);
    }

    public void SendUpdatePlayers(List<PlayerInfo> info)
    {
        object[] package = new object[info.Count];

        for (int i = 0; i < info.Count; i++)
        {
            object[] packet = new object[2];

            packet[0] = info[i].team;
            packet[1] = info[i].actorNum;

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
                (int)packet[1]
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

    private void EndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        //mapcam.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        Debug.LogError("GameOver. " + CheckWinLose());
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
            GameObject thePlayer = PhotonNetwork.Instantiate("PlayerDivisions/" + stringValue, spawn, spawnPoint.rotation);
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
            return hexGrids[possInds[rand]].transform.position;
        }

        return Vector3.zero;
    }

    private string CheckWinLose()
    {
        int red, blue;
        red = blue = 0;
        for (int x = 0; x < hexGrids.Count; ++x)
        {
            Transform myMat = hexGrids[x].transform.GetChild(0);
            Material myMatMesh = myMat.GetComponent<MeshRenderer>().material;
            
            if (myMatMesh.name != "Blue (Instance)")
            {
                blue++;
            }
            else if (myMatMesh.name != "Red (Instance)")
            {
                red++;
            }
        }
        
        if (red > blue)
        {
            return "Red";
        }
        else if (blue > red)
        {
            return "Blue";
        }

        return "Draw";
    }

    public bool gameStart
    {
        get;
        set;
    }

    private string fToS(float time)
    {
        int front = (int)time;
        float back = time - front;
        string tempB = back.ToString("F2");
        string newB = "";
        for (int x = 0; x < tempB.Length; ++x)
        {
            if (x > 1)
                newB += tempB[x];
        }
        return front + ":" + newB;
    }
}
