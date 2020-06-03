using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;

public class PlayerLoadout : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public Weapon[] weapons;
    public Transform weaponParent;
    public GameObject bulletholePrefab;
    public LayerMask bulletCollidable;

    private GameObject currWeapon;
    private int currWeapID = -1;
    private float firerate;

    private bool isReloading;

    private void Start()
    {
        foreach (Weapon w in weapons) w.InitGun();
        photonView.RPC("Equip", RpcTarget.AllBuffered, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!photonView.IsMine) return;

        if (photonView.IsMine)
        {
            for (int x = 0; x < 3; ++x) //3 slots for loadout
            {
                if (Input.GetKeyDown((KeyCode)(49 + x)))
                {
                    if (x != currWeapID && x < weapons.Length)
                    {
                        photonView.RPC("Equip", RpcTarget.AllBuffered, x);
                        //Equip(x);
                    }
                }
            }

            if (currWeapID != -1)
            {
                //AimDownSight(Input.GetMouseButton(1));
                photonView.RPC("AimDownSight", RpcTarget.All, Input.GetMouseButton(1));
                GetComponent<Player>().SetADS(Input.GetMouseButton(1));

                if (Input.GetMouseButton(0) && firerate <= 0f)
                {
                    if (weapons[currWeapID].FireBullet())
                    { 
                        photonView.RPC("Shoot", RpcTarget.All, Input.GetMouseButton(1));
                    }
                    else
                    {
                        if (weapons[currWeapID].ifReloadable() && !isReloading)
                            StartCoroutine(Reload(weapons[currWeapID].reloadTime));
                    }
                    //Shoot();
                }

                if (Input.GetKeyDown(KeyCode.R))
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
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, hip.position, Time.deltaTime * weapons[currWeapID].adsSpeed);
        }
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
    }

    [PunRPC]
    private void Shoot(bool isAds)
    {
        Transform playerEye = transform.Find("Cameras/EyeCam");

        float rand = Random.Range(-weapons[currWeapID].bloom, weapons[currWeapID].bloom);
        float rand2 = Random.Range(-weapons[currWeapID].bloom, weapons[currWeapID].bloom);
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
                if (hit.collider.gameObject.layer == 11)
                {
                    hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, weapons[currWeapID].damage);
                }
            }    
        }

        float adsDamp = 1f;
        if (isAds)
            adsDamp = 0.5f;

        currWeapon.transform.Rotate(-weapons[currWeapID].recoil * adsDamp, 0, 0);
        currWeapon.transform.position -= currWeapon.transform.forward * weapons[currWeapID].kickback * adsDamp;

        firerate = weapons[currWeapID].firerate;
    }

    [PunRPC]
    private void TakeDamage(int damage)
    {
        GetComponent<Player>().TakeDamage(damage);
    }

    public void UpdateAmmoUI(Text theUI)
    {
        if (currWeapID != -1)
            theUI.text = weapons[currWeapID].GetMagAmmo().ToString() + "/" + weapons[currWeapID].GetAmmo().ToString();
    }
}
