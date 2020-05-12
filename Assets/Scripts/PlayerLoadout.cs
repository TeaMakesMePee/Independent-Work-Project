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
        if (!photonView.IsMine) return;

        for (int x = 0; x < 3; ++x) //3 slots for loadout
        {
            if (Input.GetKeyDown((KeyCode)(49 + x)))
            {
                if (x != currWeapID && x < weapons.Length)
                    Equip(x);
            }
        }

        if (currWeapID != -1)
        { 
            AimDownSight(Input.GetMouseButton(1)); 
            
            if (Input.GetMouseButton(0) && firerate <= 0f)
            {
                Shoot();
            }

            currWeapon.transform.localPosition = Vector3.Lerp(currWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            firerate -= Time.deltaTime;
        }
    }

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

    private void Shoot()
    {
        Transform playerEye = transform.Find("Cameras/EyeCam");

        Vector3 bloom = playerEye.forward + Random.Range(-weapons[currWeapID].bloom, weapons[currWeapID].bloom) * playerEye.up
            + Random.Range(-weapons[currWeapID].bloom, weapons[currWeapID].bloom) * playerEye.right;
        bloom.Normalize();

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(playerEye.position, bloom, out hit, 1000f, bulletCollidable))
        {
            GameObject newBullethole = Instantiate(bulletholePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            newBullethole.transform.LookAt(hit.point + hit.normal);
            Destroy(newBullethole, 5f);
        }

        currWeapon.transform.Rotate(-weapons[currWeapID].recoil, 0, 0);
        currWeapon.transform.position -= currWeapon.transform.forward * weapons[currWeapID].kickback;

        firerate = weapons[currWeapID].firerate;
    }
}
