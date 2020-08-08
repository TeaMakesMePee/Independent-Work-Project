using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

/*
 * This scripts handles the information and functions of the player in terms of movement, kills, taking damage, etc.
 * Also handles UI related to player
*/

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

    public float maxHealth;
    private float currHealth;

    private Transform hpBar;
    private float currHpScale;

    private TextMeshProUGUI ammoUI, currAmmo, baseAmmo;

    private GameObject divisionUIParent;

    private HexGameManager manager;
    private PlayerLoadout loadout;

    private int playerKills, playerDeaths;

    private float lookRotation;

    private GenerateHexGrid theHexGrids = new GenerateHexGrid();

    //private bool isADS;
    //private float adsDamp;

    private bool isMoving;

    private Weapon thePlayerWeap;

    private GameObject crosshair;
    private GameObject sceneCam;

    private Division p_Division;

    private TextMeshProUGUI t_currHP;

    private List<int> assistList;

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
            divisionUIParent = GameObject.Find("DivisionUI"); //Set the division script to the player GameObject
            switch (GameData.GetDivision())
            {
                case GameData.Division.P_Damage:
                    p_Division = gameObject.AddComponent<Damage>();
                    divisionUIParent.transform.Find("DamageUI").gameObject.SetActive(true);
                    p_Division.Init(7.5f, 8.5f, 450f);
                    GameObject.Find("WeaponScrollUI").GetComponent<WeaponUI>().SetRifleSprite("Damage");
                    break;
                case GameData.Division.P_Flank:
                    p_Division = gameObject.AddComponent<Flank>();
                    divisionUIParent.transform.Find("FlankUI").gameObject.SetActive(true);
                    p_Division.Init(10f, 5f, 500f);
                    GameObject.Find("WeaponScrollUI").GetComponent<WeaponUI>().SetRifleSprite("Flank");
                    break;
                case GameData.Division.P_Tank:
                    p_Division = gameObject.AddComponent<Tank>();
                    divisionUIParent.transform.Find("TankUI").gameObject.SetActive(true);
                    p_Division.Init(5f, 8f, 400f);
                    GameObject.Find("WeaponScrollUI").GetComponent<WeaponUI>().SetRifleSprite("Tank");
                    break;
            }

            sceneCam = GameObject.Find("Cameras").transform.Find("GameEnd").gameObject;
            if (sceneCam != null)
                sceneCam.SetActive(false);

            t_currHP = GameObject.Find("CurrHP").GetComponent<TextMeshProUGUI>();
            GameObject.Find("BaseHP").GetComponent<TextMeshProUGUI>().text = t_currHP.text = ((int)maxHealth).ToString();
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
            TextMeshProUGUI teamText = GameObject.Find("Team").GetComponent<TextMeshProUGUI>();
            teamText.text = teamName + " Team";
            teamText.color = teamName == "Blue" ? new Color(0f, 1f, 1f) : new Color(1f, 0f, 0f);
        }

        hpBar = GameObject.Find("HpBar").transform;
        currAmmo = GameObject.Find("CurrAmmo").GetComponent<TextMeshProUGUI>();
        baseAmmo = GameObject.Find("BaseAmmo").GetComponent<TextMeshProUGUI>();
        currHpScale = 1f;

        crosshair = GameObject.Find("Crosshair");

        isMoving = false;
        assistList = new List<int>();
    }

    private void Update()
    {
        if (!manager.gameStart) return;


        if (!photonView.IsMine) 
        {
            UpdateNonClientPlayers();
            return; 
        }

        if (GameData.pauseGame) return;

        //Get player weapon in that frame
        thePlayerWeap = GetComponent<PlayerLoadout>().GetWeapon();

        //Set active of crosshair
        crosshair.SetActive(!thePlayerWeap.isAds);

        float horiMove = Input.GetAxisRaw("Horizontal");
        float vertMove = Input.GetAxisRaw("Vertical");
        float bobLerp = 2f;

        if ((horiMove == 0 && vertMove == 0)) //if not moving
        {
            HeadBob(idleCount, thePlayerWeap.staticBobX * thePlayerWeap.adsDampVal, thePlayerWeap.staticBobY * thePlayerWeap.adsDampVal);
            idleCount += Time.deltaTime;
        }
        else if (!isSlowWalk && Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + 0.1f, ground)) //if walking
        {
            HeadBob(movingCount, thePlayerWeap.walkBobX * thePlayerWeap.adsDampVal, thePlayerWeap.walkBobY * thePlayerWeap.adsDampVal);
            movingCount += Time.deltaTime * 4f;
            bobLerp = 14f;
        }
        else if (isSlowWalk || !Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + 0.1f, ground)) //if slow walking
        {
            HeadBob(movingCount, thePlayerWeap.sWalkBobX * thePlayerWeap.adsDampVal, thePlayerWeap.sWalkBobY * thePlayerWeap.adsDampVal);
            movingCount += Time.deltaTime * 2f;
            bobLerp = 8f;
        }

        //Lerps the weapon smoothly to show bob effect
        weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, newWeaponBobPos, Time.deltaTime * bobLerp);

        //Lerp HP bar scale
        currHpScale = Mathf.Lerp(currHpScale, currHealth / maxHealth, Time.deltaTime * 10f);
        hpBar.localScale = new Vector3(currHpScale, 1f, 1f);
        loadout.UpdateAmmoUI(currAmmo, baseAmmo);

        t_currHP.text = ((int)currHealth).ToString();

        //Use division ability
        if (Input.GetKey(KeyCode.Q))
        {
            p_Division.UseAbility();
        }
        //Update division
        p_Division.UpdateDivisionStats();

        if (transform.position.y <= -2.5f)
            InstantDeath();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (!manager.gameStart) return;

        if (GameData.pauseGame) return;

        float horiMove = Input.GetAxisRaw("Horizontal");
        float vertMove = Input.GetAxisRaw("Vertical");

        float newSpeed = p_Division.GetMoveSpeed();
        isSlowWalk = false;
        inAir = true;

        //In air check
        if (Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + 0.1f, ground))
        {
            inAir = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            p_Division.Jump(inAir);
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

    public void TakeDamage(float damage, int actor, string weapName)
    {
        if (photonView.IsMine)
        {
            p_Division.TakeDamage(damage);
            AddToAssists(actor);
            if (currHealth <= 0f)
            {
                GetComponent<PlayerLoadout>().StopReloading();
                manager.Spawn();
                manager.SendUpdatedPlayerStats(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1); //add death to myself
                manager.SendUpdatedPlayerStats(actor, 0, 1); //add kill to killer
                manager.SendKillfeedInfo(actor, PhotonNetwork.LocalPlayer.ActorNumber, weapName); //add killfeed info
                for (int x = 0; x < assistList.Count; ++x)
                {
                    if (assistList[x] != actor)
                    {
                        manager.SendUpdatedPlayerStats(assistList[x], 2, 1); //add assists to assistant
                    }
                }
                crosshair.SetActive(true);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void InstantDeath()
    {
        GetComponent<PlayerLoadout>().StopReloading();
        manager.Spawn();
        manager.SendUpdatedPlayerStats(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
        for (int x = 0; x < assistList.Count; ++x)
        {
            manager.SendUpdatedPlayerStats(assistList[x], 2, 1); //add assists to assistant
        }
        crosshair.SetActive(true);
        PhotonNetwork.Destroy(gameObject);
    }

    public float GetHealth()
    {
        return currHealth;
    }

    public void SetHealth(float health)
    {
        currHealth = health;
    }

    [PunRPC]
    private void SyncTeamName(string tN)
    {
        teamName = tN;
        GetComponent<MeshRenderer>().material.color = teamName == "Blue" ? new Color(0f, 1f, 1f) : new Color(1f, 0f, 0f);
    }

    public bool GetMoving()
    {
        return isMoving;
    }

    //returns whether the game has started
    public bool GetGameStart()
    {
        return manager.gameStart;
    }

    public MatchStats GetMatchStats()
    {
        return manager.GetStatTracker();
    }

    public string teamName
    {
        get;
        set;
    }

    private void AddToAssists(int actor) //adds to the list of people who shot the player in current life
    {
        bool exists = false;

        for (int x = 0; x < assistList.Count; ++x)
        {
            if (assistList[x] == actor)
                exists = true;
        }

        if (!exists)
            assistList.Add(actor);
    }
}
