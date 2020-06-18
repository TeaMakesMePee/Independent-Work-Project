using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed, sprintMultiplier;
    public Camera playerEye;
    public float baseFOV, sprintFOV;
    public float jumpForce;
    public LayerMask ground;
    public Transform weaponParent;

    private float targetFOV;
    private Rigidbody playerRig;
    private bool isSlowWalk, inAir;
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

    private float lookRotation;

    private GenerateHexGrid theHexGrids = new GenerateHexGrid();

    private bool isADS;
    private float adsDamp;

    private bool isMoving;

    private Weapon thePlayerWeap;

    private GameObject crosshair;
    private GameObject sceneCam;

    private Division p_Division;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo message) //Sends data if your photonview, receives data if it isnt yours
    {
        //Example: 
        //Set up: You are player A and player B needs your rotation
        //You send to all players who observe this player A script's your rotation
        //Players who have player A script on them but isnt player A will read the data and update accordingly
        
        //Note: Not to send data that needs to be precise, as it is unreliable
        if (stream.IsWriting)
        {
            stream.SendNext((int)(weaponParent.transform.eulerAngles.x * 100f));
        }
        else
        {
            lookRotation = (int)stream.ReceiveNext() / 100f;
        }
    }

    private void Start()
    {
        //All players except yourself will be assigned the 'Player' layer, so that they can be shot, except you.
        if (!photonView.IsMine)
        {
            gameObject.layer = 11;
            gameObject.tag = "Player";
        }
        else
        {
            switch (GameData.GetDivision())
            {
                case GameData.Division.P_Damage:
                    p_Division = gameObject.AddComponent<Damage>();
                    p_Division.Init(7.5f, 10f);
                    break;
                case GameData.Division.P_Flank:
                    p_Division = gameObject.AddComponent<Flank>();
                    p_Division.Init(10f, 5f);
                    break;
                case GameData.Division.P_Tank:
                    p_Division = gameObject.AddComponent<Tank>();
                    p_Division.Init(5f, 8f);
                    break;
            }

            sceneCam = GameObject.Find("SceneCam");
            if (sceneCam != null)
                sceneCam.SetActive(false);
        }

        //Set my player camera to true
        camParent.SetActive(photonView.IsMine);

        //Camera.main.gameObject.SetActive(false);
        playerRig = GetComponent<Rigidbody>();
        weaponOrigin = weaponParent.localPosition;

        //Set health
        currHealth = maxHealth;

        manager = GameObject.Find("GameManager").GetComponent<HexGameManager>();
        loadout = GetComponent<PlayerLoadout>();

        if (photonView.IsMine)
        {
            teamName = manager.GetLocalPlayerTeam();
            photonView.RPC("SyncTeamName", RpcTarget.AllBuffered, teamName);
        }

        hpBar = GameObject.Find("Bar").transform;
        ammoUI = GameObject.Find("AmmoCount").GetComponent<Text>();
        currHpScale = 1f;

        crosshair = GameObject.Find("Crosshair");

        isADS = false;
        adsDamp = 1f;
        isMoving = false;
    }

    private void Update()
    {
        //Debug.LogError(photonView.ViewID);
        if (!manager.gameStart) return;

        if (!photonView.IsMine) 
        {
            UpdateNonClientPlayers();
            return; 
        }

        //Get player weapon in that frame
        thePlayerWeap = GetComponent<PlayerLoadout>().GetWeapon();

        //Set active of crosshair
        crosshair.SetActive(!thePlayerWeap.isAds);

        //Debug.LogError(PhotonNetwork.LocalPlayer.ActorNumber);

        float horiMove = Input.GetAxisRaw("Horizontal");
        float vertMove = Input.GetAxisRaw("Vertical");
        float bobLerp = 2f;

        if (horiMove == 0 && vertMove == 0) //if not moving
        {
            HeadBob(idleCount, thePlayerWeap.staticBobX * thePlayerWeap.adsDampVal, thePlayerWeap.staticBobY * thePlayerWeap.adsDampVal);
            idleCount += Time.deltaTime;
        }
        else if (!isSlowWalk) //if walking
        {
            HeadBob(movingCount, thePlayerWeap.walkBobX * thePlayerWeap.adsDampVal, thePlayerWeap.walkBobY * thePlayerWeap.adsDampVal);
            movingCount += Time.deltaTime * 4f;
            bobLerp = 14f;
        }
        else if (isSlowWalk) //if slow walking
        {
            HeadBob(movingCount, thePlayerWeap.sWalkBobX * thePlayerWeap.adsDampVal, thePlayerWeap.sWalkBobY * thePlayerWeap.adsDampVal);
            movingCount += Time.deltaTime * 2f;
            bobLerp = 8f;
        }

        //Lerps the weapon smoothly to show bob effect
        weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, newWeaponBobPos, Time.deltaTime * bobLerp);

        //Lerp HP bar scale
        currHpScale = Mathf.Lerp(currHpScale, (float)(currHealth) / (float)(maxHealth), Time.deltaTime * 10f);
        hpBar.localScale = new Vector3(currHpScale, 1f, 1f);
        loadout.UpdateAmmoUI(ammoUI);

        //Update division
        p_Division.UpdateDivisionStats();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (!manager.gameStart) return;

        float horiMove = Input.GetAxisRaw("Horizontal");
        float vertMove = Input.GetAxisRaw("Vertical");

        float newSpeed = speed;
        isSlowWalk = false;
        inAir = true;

        //In air check
        if (Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + 0.1f, ground))
        {
            inAir = false;
        }

        //Jump check
        //if (Input.GetKey(KeyCode.Space) && !inAir)
        //{
        //    playerRig.AddForce(Vector3.up * jumpForce);
        //}
        if (Input.GetKey(KeyCode.Space))
        {
            p_Division.Jump(inAir);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            p_Division.UseAbility();
        }

        //Sprint check
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            newSpeed *= sprintMultiplier;
            isSlowWalk = true;
        }

        Vector3 dir = new Vector3(horiMove, 0, vertMove);
        dir.Normalize();
        if (dir == Vector3.zero)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        Vector3 newVel = transform.TransformDirection(dir) * newSpeed * Time.deltaTime;
        newVel.y = playerRig.velocity.y;
        playerRig.velocity = newVel;

        //TEMP
        if (!inAir)
        {
            //theHexGrids.ChangeHexColor(transform.position);
        }
    }

    private void UpdateNonClientPlayers()
    {
        float temp = weaponParent.localEulerAngles.y;
        Quaternion targetRot = Quaternion.identity * Quaternion.AngleAxis(lookRotation, Vector3.right);
        weaponParent.rotation = Quaternion.Slerp(weaponParent.rotation, targetRot, Time.deltaTime * 10f);

        Vector3 newRot = weaponParent.eulerAngles;
        newRot.y = temp;
        weaponParent.localEulerAngles = newRot;
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

            if (currHealth <= 0)
            {
                manager.Spawn();
                PhotonNetwork.Destroy(gameObject);
                playerDeaths++;
            }
        }
    }

    [PunRPC]
    private void SyncTeamName(string tN)
    {
        teamName = tN;
    }

    public void SetADS(bool adsState)
    {
        isADS = adsState;
    }

    public bool GetMoving()
    {
        return isMoving;
    }

    public bool GetGameStart()
    {
        return manager.gameStart;
    }

    public string teamName
    {
        get;
        set;
    }
}
