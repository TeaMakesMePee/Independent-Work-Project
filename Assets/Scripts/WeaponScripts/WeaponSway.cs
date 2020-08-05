using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponSway : MonoBehaviourPunCallbacks
{
    public float intensity, smooth;
    private Quaternion initRot;
    private GameObject player;

    private void Start()
    {
        player = transform.root.gameObject;
        initRot = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameData.pauseGame) return;
            Sway();
    }

    private void Sway()
    {
        float mouseInputX = Input.GetAxis("Mouse X");
        float mouseInputY = Input.GetAxis("Mouse Y");

        if (!player.GetPhotonView().IsMine)
        {
            mouseInputX = mouseInputY = 0;
        }

        //calculate target rotation
        Quaternion rotDeltaX = Quaternion.AngleAxis(-intensity * mouseInputX, Vector3.up);
        Quaternion rotDeltaY = Quaternion.AngleAxis(intensity * mouseInputY, Vector3.right);
        Quaternion nextRot = initRot * rotDeltaX * rotDeltaY;

        //rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, nextRot, Time.deltaTime * smooth);
    }
}
