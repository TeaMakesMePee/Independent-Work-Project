using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class HexGameManager : MonoBehaviour
{
    public string playerPrefab;
    public Transform playerTransform;

    public void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        PhotonNetwork.Instantiate(playerPrefab, playerTransform.position, playerTransform.rotation);
    }
}
