using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public int damage;
    public string weapName;
    public float firerate, adsSpeed, recoil, kickback;
    public float reloadTime;
    public GameObject prefab;

    private int ammo, currMagSize;
    [SerializeField]
    public int staticAmmo;
    [SerializeField]
    public int maxMagSize;

    public float bloom, maxBloom, initBloom;

    public void InitGun() //Init weapon if it hasnt yet been initted
    {
        ammo = staticAmmo;
        currMagSize = maxMagSize;
        bloom = initBloom;
        isAds = false;
    }

    public bool FireBullet()
    {
        if (currMagSize > 0)
        {
            currMagSize -= 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ifReloadable()
    {
        int emptySlots = maxMagSize - currMagSize; //check the amount of free space in mag

        if (ammo <= 0 || emptySlots == 0) //If no ammo, or if no empty slots, don't reload
            return false;

        return true;
    }

    public void Reload()
    {
        int emptySlots = maxMagSize - currMagSize;

        if (ammo >= emptySlots) //check if ammo left is > free bullet space in mag
        {
            ammo -= emptySlots; //subtract from ammo
            currMagSize = maxMagSize; //refill mag
        }
        else
        {
            currMagSize += ammo; //refill mag with whatevers left
            ammo = 0; //set to 0
        }
    }

    public int GetAmmo() { return ammo; }

    public int GetMagAmmo() { return currMagSize; }

    public bool isAds { get; set; }
}
