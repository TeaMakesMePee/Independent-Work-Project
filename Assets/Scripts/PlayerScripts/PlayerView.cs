using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerView : MonoBehaviourPunCallbacks
{
    #region vars

    public Transform player, cam, weapon;
    public float xSens, ySens, maxAngle;
    // Start is called before the first frame update
    private Quaternion initCamRot;

    public static bool cursorLocked = true;

    #endregion

    void Start()
    {
        initCamRot = cam.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        LookAt();
        UpdateCursor();
    }

    void LookAt()
    {
        //Y axis 
        float sens = 250f;
        if (PlayerPrefs.HasKey("Sens"))
        {
            sens = PlayerPrefs.GetFloat("Sens");
        }
        float mouseInputY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        Quaternion offset = Quaternion.AngleAxis(mouseInputY, -Vector3.right);
        Quaternion newCamRot = cam.localRotation * offset; //Add the offset to the current rotation

        //Clamp
        if (Quaternion.Angle(initCamRot, newCamRot) <= maxAngle)
        {
            cam.localRotation = newCamRot;
        }

        weapon.rotation = cam.rotation;

        //X axis
        float mouseInputX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        offset = Quaternion.AngleAxis(mouseInputX, Vector3.up);
        player.localRotation *= offset; //Rotate player body instead, cam will follow
    }

    void UpdateCursor()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = true;
            }
        }
    }
}
