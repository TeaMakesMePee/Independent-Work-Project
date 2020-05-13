using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class HexGameManager : MonoBehaviour
{
    public string playerPrefab;
    public Transform spawnPoint;

    public void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        PhotonNetwork.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
