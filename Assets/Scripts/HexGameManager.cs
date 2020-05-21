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
    //public Transform mapTransform;

    public void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        GameObject MapContainer = GameObject.Find("Map");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject(mapPrefab, Vector3.zero, Quaternion.identity);
        }
        PhotonNetwork.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
