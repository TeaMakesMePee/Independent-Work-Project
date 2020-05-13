using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;

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

    void Start()
    {

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

                if (Input.GetMouseButton(0) && firerate <= 0f)
                {
                    photonView.RPC("Shoot", RpcTarget.All);
                    //Shoot();
                }

                firerate -= Time.deltaTime;
            }
        }
        
        if (currWeapID != -1)
            currWeapon.transform.localPosition = Vector3.Lerp(currWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
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
            Destroy(currWeapon);

        GameObject newWeapon = Instantiate(weapons[weaponID].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localEulerAngles = Vector3.zero;

        currWeapon = newWeapon;
        currWeapID = weaponID;
    }

    [PunRPC]
    private void Shoot()
    {
        Transform playerEye = transform.Find("Cameras/EyeCam");

        float rand = Random.Range(-weapons[currWeapID].bloom, weapons[currWeapID].bloom);
        float rand2 = Random.Range(-weapons[currWeapID].bloom, weapons[currWeapID].bloom);
        Vector3 bloom = playerEye.forward + rand * playerEye.up
            + rand2 * playerEye.right;
        bloom.Normalize();

        Debug.LogError("player eye pos: " + playerEye.position + ", forward: " + playerEye.forward
            + ", rand: " + rand + ", up: " + playerEye.up + ", rand2: " + rand2 + ", rigt: " + playerEye.right);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(playerEye.position, bloom, out hit, 1000f, bulletCollidable))
        {
            GameObject newBullethole = Instantiate(bulletholePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            newBullethole.transform.LookAt(hit.point + hit.normal);
            Destroy(newBullethole, 5f);
            Debug.LogError("Soot");
            if (photonView.IsMine)
            {
                if (hit.collider.gameObject.layer == 11)
                {
                    hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, weapons[currWeapID].damage);
                }
            }    
        }

        currWeapon.transform.Rotate(-weapons[currWeapID].recoil, 0, 0);
        currWeapon.transform.position -= currWeapon.transform.forward * weapons[currWeapID].kickback;

        firerate = weapons[currWeapID].firerate;
    }

    [PunRPC]
    private void TakeDamage(int damage)
    {
        GetComponent<PlayerMovement>().TakeDamage(damage);
    }
}
