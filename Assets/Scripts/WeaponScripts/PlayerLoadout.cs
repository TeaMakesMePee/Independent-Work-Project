using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;
using TMPro;
using System.Xml.Schema;

public class PlayerLoadout : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public Weapon[] weapons;
    public Transform weaponParent;
    public GameObject bulletholePrefab;
    public LayerMask bulletCollidable;

    private GameObject currWeapon;
    private int currWeapID = 0;
    private float firerate;

    private bool isReloading;
    private float mouseScroll;

    public AudioSource audioSource3D, audioSource2D;
    public AudioClip reloadClip;
    public AudioClip hitmarkerClip;

    [SerializeField]
    public GameObject damageIndicator;

    private Image hitmarker;
    private Color teamColor;
    private float hitmarkerCd;

    private void Start()
    {
        isReloading = false;
        if (photonView.IsMine)
        {
            foreach (Weapon w in weapons) w.InitGun();
            photonView.RPC("Equip", RpcTarget.AllBuffered, currWeapID);
            hitmarker = GameObject.Find("HUD/Crosshair/InnerRing").GetComponent<Image>();
            hitmarker.color = GetComponent<Player>().teamName == "Red" ? new Color(1f, 0f, 0f, 0f) : new Color(0f, 1f, 1f, 0f);
            teamColor = GetComponent<Player>().teamName == "Red" ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 1f, 1f, 1f);
            hitmarkerCd = 0f;
        }
        mouseScroll = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameData.pauseGame) return;
        //if (!photonView.IsMine) return;
        if (photonView.IsMine)
        {
            #region weapon wheel
            if (mouseScroll > 0f && Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                mouseScroll += Input.GetAxis("Mouse ScrollWheel");
            }
            else if (mouseScroll < 0f && Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                mouseScroll += Input.GetAxis("Mouse ScrollWheel");
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                mouseScroll += Input.GetAxis("Mouse ScrollWheel");
            }

            if (mouseScroll <= -0.3f)
            {
                if (currWeapID < weapons.Length - 1)
                {
                    currWeapID += 1;
                }
                else if (currWeapID == weapons.Length - 1)
                {
                    currWeapID = 0;
                }
                photonView.RPC("Equip", RpcTarget.AllBuffered, currWeapID);
                mouseScroll = 0f;
            }
            else if (mouseScroll >= 0.3f)
            {
                if (currWeapID > 0)
                {
                    currWeapID -= 1;
                }
                else if (currWeapID == 0)
                {
                    currWeapID = weapons.Length - 1;
                }
                photonView.RPC("Equip", RpcTarget.AllBuffered, currWeapID);
                mouseScroll = 0f;
            }
            #endregion

            #region weapon keys
            for (int x = 0; x < weapons.Length; ++x) //3 slots for loadout
            {
                if (Input.GetKeyDown((KeyCode)(49 + x)))
                {
                    if (x != currWeapID/* && x < weapons.Length*/)
                    {
                        currWeapID = x;
                        photonView.RPC("Equip", RpcTarget.AllBuffered, currWeapID);
                        //Equip(x);
                    }
                }
            }
            #endregion

            if (currWeapID != -1 && GetComponent<Player>().GetGameStart())
            {
                //AimDownSight(Input.GetMouseButton(1));
                if (weapons[currWeapID].weapName == "Pistol") //Only pistols can ads
                {
                    photonView.RPC("AimDownSight", RpcTarget.All, Input.GetMouseButton(1) && !isReloading);
                    //GetComponent<Player>().SetADS(Input.GetMouseButton(1));
                }

                #region old shoot call
                //if (Input.GetMouseButton(0) && firerate <= 0f)
                //{
                //    if (weapons[currWeapID].FireBullet())
                //    { 
                //        photonView.RPC("Shoot", RpcTarget.All);
                //    }
                //    else
                //    {
                //        if (weapons[currWeapID].ifReloadable() && !isReloading)
                //            StartCoroutine(Reload(weapons[currWeapID].reloadTime));
                //    }
                //}
                #endregion

                //Apply increasing bloom to all weapons except pistol
                if (weapons[currWeapID].weapName != "Pistol")
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (weapons[currWeapID].bloom < weapons[currWeapID].maxBloom)
                        {
                            float multiply = 0.015f;
                            if (GetComponent<Player>().GetMoving()) //if moving, increase bloom even more
                            {
                                multiply = 0.025f;
                            }
                            weapons[currWeapID].bloom += Time.deltaTime * multiply;
                            if (weapons[currWeapID].bloom > weapons[currWeapID].maxBloom)
                                weapons[currWeapID].bloom = weapons[currWeapID].maxBloom;
                        }
                    }
                    else
                    {
                        if (weapons[currWeapID].bloom > 0f)
                        {
                            weapons[currWeapID].bloom -= Time.deltaTime * 0.15f;
                            if (weapons[currWeapID].bloom < 0f)
                                weapons[currWeapID].bloom = 0f;
                        }
                    }

                    //Clamp
                    weapons[currWeapID].bloom = Mathf.Clamp(weapons[currWeapID].bloom, 0f, weapons[currWeapID].maxBloom);
                }

                if (Input.GetKeyDown(KeyCode.R)) //Reload
                {
                    if (weapons[currWeapID].ifReloadable() && !isReloading)
                        StartCoroutine(Reload(weapons[currWeapID].reloadTime));
                }

                firerate -= Time.deltaTime;
            }

            Transform theAnchor = currWeapon.transform.Find("Anchor");
            GameObject theDesign = theAnchor.Find("Design").gameObject;

            if (hitmarkerCd > 0f)
            {
                hitmarkerCd -= Time.deltaTime;
            }
            else
            {
                float a = Mathf.Lerp(hitmarker.color.a, 0f, Time.deltaTime * 2f);
                hitmarker.color = new Color(teamColor.r, teamColor.g, teamColor.b, a);
            }
        }
        
        //Lerp weapon back to default position, resets kickback, for all players
        if (currWeapID != -1)
            currWeapon.transform.localPosition = Vector3.Lerp(currWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
    }
    
    IEnumerator Reload(float reloadTime)
    {
        //Debug.LogError("reloading");
        isReloading = true;
        //currWeapon.SetActive(false);
        Transform theAnchor = currWeapon.transform.Find("Anchor");
        GameObject theDesign = null;
        if (theAnchor != null)
        {
            theDesign = theAnchor.Find("Design").gameObject;
        }

        float rotateAmount = 0f;
        while (rotateAmount < 359f)
        {
            float start = rotateAmount;
            rotateAmount = Mathf.Lerp(rotateAmount, 360f, Time.deltaTime * 5f);
            //Debug.LogError(theDesign.transform.childCount);
            if (theDesign != null)
            {
                for (int x = 0; x < theDesign.transform.childCount; ++x)
                {
                    if (theDesign.transform.GetChild(x).gameObject.activeSelf)
                        theDesign.transform.GetChild(x).Rotate(weapons[currWeapID].rotateDir, rotateAmount - start);
                }
                yield return null;
            }
            else
            {
                break;
            }
        }

        if (theDesign != null)
        {
            for (int x = 0; x < theDesign.transform.childCount; ++x)
            {
                if (theDesign.transform.GetChild(x).gameObject.activeSelf)
                    theDesign.transform.GetChild(x).Rotate(weapons[currWeapID].rotateDir, 360f - rotateAmount);
            }

            yield return new WaitForSeconds(0f);

            //Play reload sound
            audioSource3D.clip = reloadClip;
            audioSource3D.pitch = 1.2f;
            audioSource3D.volume = 1f;
            audioSource3D.Play();
        }

        //currWeapon.SetActive(true);
        weapons[currWeapID].Reload();
        isReloading = false;
    }

    [PunRPC]
    private void AimDownSight(bool isAim)
    {
        Transform anchor = currWeapon.transform.Find("Anchor");
        Transform hip = currWeapon.transform.Find("States/Hip");
        Transform ads = currWeapon.transform.Find("States/Ads");

        if (isAim)
        {
            anchor.position = Vector3.Lerp(anchor.position, ads.position, Time.deltaTime * weapons[currWeapID].adsSpeed);
            weapons[currWeapID].adsDampVal = weapons[currWeapID].adsDamp;
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, hip.position, Time.deltaTime * weapons[currWeapID].adsSpeed);
            weapons[currWeapID].adsDampVal = weapons[currWeapID].initAdsDamp;
        }

        if (photonView.IsMine)
            weapons[currWeapID].isAds = isAim;
    }

    [PunRPC]
    void Equip(int weaponID)
    {
        if (currWeapon != null)
        {
            if (isReloading) StopCoroutine("Reload");
            Destroy(currWeapon); 
        }

        GameObject newWeapon = Instantiate(weapons[weaponID].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localEulerAngles = Vector3.zero;

        currWeapon = newWeapon;
        currWeapID = weaponID;
        if (photonView.IsMine)
        {
            GameObject.Find("WeaponScrollUI").GetComponent<WeaponUI>().Equip(weaponID);
        }

        audioSource3D.clip = reloadClip;
        audioSource3D.pitch = 1.8f;
        audioSource3D.volume = 0.5f;
        audioSource3D.Play();
        //currWeapID = weaponID;
    }

    [PunRPC]
    private void Shoot(float _damage, float _firerate)
    {
        //Temp code
        float recDamp = 1f;
        float kbDamp = 1f;
        float bDamp = 1f;
        if (weapons[currWeapID].isAds)
        {
            recDamp = 0.5f;
            kbDamp = 0.1f;
            bDamp = 0.2f;
        }

        Transform playerEye = transform.Find("Cameras/EyeCam");

        float rand = Random.Range(-weapons[currWeapID].bloom * bDamp, weapons[currWeapID].bloom * bDamp/*-0.1f, 0.1f*/);
        float rand2 = Random.Range(-weapons[currWeapID].bloom * bDamp, weapons[currWeapID].bloom * bDamp/*-0.1f, 0.1f*/);
        Vector3 bloom = playerEye.forward + rand * playerEye.up
            + rand2 * playerEye.right;
        bloom.Normalize();

        //Debug.LogError("player eye pos: " + playerEye.position + ", forward: " + playerEye.forward
        //+ ", rand: " + rand + ", up: " + playerEye.up + ", rand2: " + rand2 + ", rigt: " + playerEye.right);
        bool hitSomething = false;

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(playerEye.position, bloom, out hit, 1000f, bulletCollidable))
        {
            GameObject newBullethole = Instantiate(bulletholePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            newBullethole.transform.SetParent(hit.transform);
            newBullethole.transform.LookAt(hit.point + hit.normal);
            Destroy(newBullethole, 5f);
            if (photonView.IsMine)
            {
                GameObject theHit = hit.collider.gameObject;
                if (theHit.layer == 11 && theHit.GetComponent<Player>().teamName != GetComponent<Player>().teamName)
                {
                    theHit.GetPhotonView().RPC("TakeDamage", RpcTarget.All, _damage, transform.position.x, transform.position.z, PhotonNetwork.LocalPlayer.ActorNumber, weapons[currWeapID].weapName);
                    hitmarker.color = teamColor;
                    hitmarkerCd = 1f;
                    audioSource2D.PlayOneShot(hitmarkerClip);
                    GetComponent<Player>().GetMatchStats().hits += 1;
                    GetComponent<Player>().GetMatchStats().damage += (int)_damage;
                    hitSomething = true;
                }
            }    
        }

        if (photonView.IsMine && !hitSomething)
        {
            GetComponent<Player>().GetMatchStats().misses += 1;
        }

        currWeapon.transform.Rotate(-weapons[currWeapID].recoil * recDamp, 0, 0);
        currWeapon.transform.position -= currWeapon.transform.forward * weapons[currWeapID].kickback * kbDamp;

        firerate = /*weapons[currWeapID].firerate*/_firerate;

        audioSource3D.Stop();
        audioSource3D.clip = weapons[currWeapID].audioClip;
        audioSource3D.pitch = weapons[currWeapID].audioPitch + Random.Range(-0.075f, 0.075f);
        audioSource3D.volume = 1f;
        audioSource3D.Play();
    }

    [PunRPC]
    private void TakeDamage(float damage, float x, float z, int actor, string weapname)
    {
        GetComponent<Player>().TakeDamage(damage, actor, weapname);
        if (photonView.IsMine)
        {
            GameObject dI = Instantiate(damageIndicator, GameObject.Find("Indicators").transform);
            dI.GetComponent<DamagedIndicator>().Init(new Vector3(x, 0f, z));
        }
    }

    public void StopReloading()
    {
        if (isReloading) StopCoroutine("Reload");
    }

    public void UpdateAmmoUI(TextMeshProUGUI currAmmo, TextMeshProUGUI baseAmmo)
    {
        if (currWeapID != -1)
        {
            currAmmo.text = weapons[currWeapID].GetMagAmmo().ToString();
            baseAmmo.text = weapons[currWeapID].GetAmmo().ToString();
        }
        //theUI.text = weapons[currWeapID].GetMagAmmo().ToString() + "/" + weapons[currWeapID].GetAmmo().ToString();
    }

    public Weapon GetWeapon()
    {
        return weapons[currWeapID];
    }

    public bool readyFire()
    {
        //Debug.LogError("firerate: " + firerate);
        //Debug.LogError("Reloading: " + isReloading);
        return firerate <= 0f && !isReloading;
    }

    public void CheckReload()
    {
        if (weapons[currWeapID].ifReloadable() && !isReloading)
        {
            //Debug.LogError("reloadable");
            StartCoroutine(Reload(weapons[currWeapID].reloadTime));
        }
    }
}
