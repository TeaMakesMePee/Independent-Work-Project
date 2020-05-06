using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string name;
    public float firerate, adsSpeed, bloom, recoil, kickback;
    public GameObject prefab;
}
