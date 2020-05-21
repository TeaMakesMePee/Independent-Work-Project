using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class HexGameManager : MonoBehaviour
{
    public string playerPrefab;
    public Transform spawnPoint;
    public string mapPrefab;
    public bool isMapSpawned;
    //public Transform mapTransform;

    public void Start()
    {
        isMapSpawned = false;
        Spawn();
    }

    public void Spawn()
    {
        if (PhotonNetwork.IsMasterClient && !isMapSpawned)
        {
            PhotonNetwork.InstantiateSceneObject(mapPrefab, Vector3.zero, Quaternion.identity);
            isMapSpawned = true;
        }
        PhotonNetwork.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
