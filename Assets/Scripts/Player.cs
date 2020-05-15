using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Player : MonoBehaviourPunCallbacks
{
    public float speed, sprintMultiplier;
    public Camera playerEye;
    public float baseFOV, sprintFOV;
    public float jumpForce;
    public LayerMask ground;
    public Transform weaponParent;

    private float targetFOV;
    private Rigidbody playerRig;
    private bool isSprint, inAir;
    private Vector3 weaponOrigin, newWeaponBobPos;
    private float idleCount, movingCount;

    public GameObject camParent;

    public int maxHealth;
    private int currHealth;

    private Transform hpBar;
    private float currHpScale;

    private Text ammoUI;

    private HexGameManager manager;
    private PlayerLoadout loadout;

    private int playerKills, playerDeaths;

    private void Start()
    {
        //All players except yourself will be assigned the 'Player' layer, so that they can be shot, except you.
        if (!photonView.IsMine) gameObject.layer = 11;

        camParent.SetActive(photonView.IsMine);

        //Camera.main.gameObject.SetActive(false);
        playerRig = GetComponent<Rigidbody>();
        weaponOrigin = weaponParent.localPosition;

        currHealth = maxHealth;

        manager = GameObject.Find("GameManager").GetComponent<HexGameManager>();
        loadout = GetComponent<PlayerLoadout>();

        hpBar = GameObject.Find("Bar").transform;
        ammoUI = GameObject.Find("AmmoCount").GetComponent<Text>();
        currHpScale = 1f;
    }

    private void Update()
    {
        Debug.LogError(photonView.ViewID);
        if (!photonView.IsMine) return;

        float horiMove = Input.GetAxisRaw("Horizontal");
        float vertMove = Input.GetAxisRaw("Vertical");
        float bobLerp = 2f;

        if (horiMove == 0 && vertMove == 0)
        {
            HeadBob(idleCount, 0.01f, 0.01f);
            idleCount += Time.deltaTime;
        }
        else if (!isSprint)
        { 
            HeadBob(movingCount, 0.035f, 0.035f);
            movingCount += Time.deltaTime * 2f;
            bobLerp = 8f;
        }
        else if (isSprint)
        {
            HeadBob(movingCount, 0.07f, 0.05f);
            movingCount += Time.deltaTime * 4f;
            bobLerp = 14f;
        }
        weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, newWeaponBobPos, Time.deltaTime * bobLerp);

        //Lerp HP bar scale
        currHpScale = Mathf.Lerp(currHpScale, (float)(currHealth) / (float)(maxHealth), Time.deltaTime * 10f);
        hpBar.localScale = new Vector3(currHpScale, 1f, 1f);
        loadout.UpdateAmmoUI(ammoUI);
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        float horiMove = Input.GetAxisRaw("Horizontal");
        float vertMove = Input.GetAxisRaw("Vertical");

        float newSpeed = speed;
        targetFOV = baseFOV;
        isSprint = false;
        inAir = true;

        //In air check
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f, ground))
        {
            inAir = false;
        }

        //Jump check
        if (Input.GetKey(KeyCode.Space) && !inAir)
        {
            playerRig.AddForce(Vector3.up * jumpForce);
        }

        //Sprint check
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && vertMove > 0 && !inAir)
        {
            newSpeed *= sprintMultiplier;
            targetFOV = sprintFOV;
            isSprint = true;
        }

        //FOV Adjustment
        playerEye.fieldOfView = Mathf.Lerp(playerEye.fieldOfView, targetFOV, Time.deltaTime * 8f);

        Vector3 dir = new Vector3(horiMove, 0, vertMove);
        dir.Normalize();

        Vector3 newVel = transform.TransformDirection(dir) * newSpeed * Time.deltaTime;
        newVel.y = playerRig.velocity.y;
        playerRig.velocity = newVel;
    }

    private void HeadBob(float timeFrame, float xIntens, float yIntens)
    {
        newWeaponBobPos = new Vector3(Mathf.Cos(timeFrame) * xIntens, Mathf.Sin(timeFrame * 2f) * yIntens, 0) + weaponOrigin;
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            currHealth -= damage;
            Debug.LogError("HP: " + currHealth);

            if (currHealth <= 0)
            {
                manager.Spawn();
                PhotonNetwork.Destroy(gameObject);
                playerDeaths++;
            }
        }
    }
}
