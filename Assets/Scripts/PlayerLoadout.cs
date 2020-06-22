using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    private void Start()
    {
        foreach (Weapon w in weapons) w.InitGun();
        photonView.RPC("Equip", RpcTarget.AllBuffered, currWeapID);
        mouseScroll = 0f;
    }

    // Update is called once per frame
    void Update()
    {
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
                    photonView.RPC("AimDownSight", RpcTarget.All, Input.GetMouseButton(1));
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
                            float multiply = 0.03f;
                            if (GetComponent<Player>().GetMoving()) //if moving, increase bloom even more
                            {
                                multiply = 0.05f;
                            }
                            weapons[currWeapID].bloom += Time.deltaTime * multiply;
                        }
                    }
                    else
                    {
                        if (weapons[currWeapID].bloom > 0f)
                            weapons[currWeapID].bloom -= Time.deltaTime * 0.1f;
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
        }
        
        //Lerp weapon back to default position, resets kickback, for all players
        if (currWeapID != -1)
            currWeapon.transform.localPosition = Vector3.Lerp(currWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
    }

    IEnumerator Reload(float reloadTime)
    {
        isReloading = true;
        currWeapon.SetActive(false);
        yield return new WaitForSeconds(reloadTime);
        weapons[currWeapID].Reload();
        currWeapon.SetActive(true);
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
                    theHit.GetPhotonView().RPC("TakeDamage", RpcTarget.All, /*weapons[currWeapID].damage*/_damage);
                }
            }    
        }

        currWeapon.transform.Rotate(-weapons[currWeapID].recoil * recDamp, 0, 0);
        currWeapon.transform.position -= currWeapon.transform.forward * weapons[currWeapID].kickback * kbDamp;

        firerate = /*weapons[currWeapID].firerate*/_firerate;
    }

    [PunRPC]
    private void TakeDamage(float damage)
    {
        GetComponent<Player>().TakeDamage(damage);
    }

    public void UpdateAmmoUI(Text theUI)
    {
        if (currWeapID != -1)
            theUI.text = weapons[currWeapID].GetMagAmmo().ToString() + "/" + weapons[currWeapID].GetAmmo().ToString();
    }

    public Weapon GetWeapon()
    {
        return weapons[currWeapID];
    }

    public bool readyFire()
    {
        return firerate <= 0f;
    }

    public void CheckReload()
    {
        if (weapons[currWeapID].ifReloadable() && !isReloading)
            StartCoroutine(Reload(weapons[currWeapID].reloadTime));
    }
}
