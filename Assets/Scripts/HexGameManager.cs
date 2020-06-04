using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Threading;

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

public class HexGameManager : MonoBehaviour, IOnEventCallback
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

    public enum EventCodes : byte
    {
        NewPlayer,
        UpdatePlayers
    }

    public void Start()
    {
        myInd = -1;
        firstSpawn = true;
        isMapSpawned = false;
        SendNewPlayer();
        Spawn();
        Debug.LogError(PhotonNetwork.LocalPlayer.ActorNumber);
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
            spawn.y = spawnPoint.position.y;
            PhotonNetwork.Instantiate(playerPrefab, spawn, spawnPoint.rotation);
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
        }
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
            spawn.y = spawnPoint.position.y;
            PhotonNetwork.Instantiate(playerPrefab, spawn, spawnPoint.rotation);
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
}
