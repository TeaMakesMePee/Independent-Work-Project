using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField]
    public Sprite tank, flank, damage;

    public void Equip(int slot)
    {
        switch (slot)
        {
            case 0: //pistol
            {
                transform.Find("SlotTwo").GetComponent<WeaponSlotAlpha>().Equip();
                transform.Find("SlotOne").GetComponent<WeaponSlotAlpha>().EquipOther();
            }
            break;
            case 1: //rifle
            {
                transform.Find("SlotOne").GetComponent<WeaponSlotAlpha>().Equip();
                transform.Find("SlotTwo").GetComponent<WeaponSlotAlpha>().EquipOther();
            }
            break;
        }
    }

    public void SetRifleSprite(string division)
    {
        transform.Find("SlotOne").gameObject.SetActive(true);
        transform.Find("SlotTwo").gameObject.SetActive(true);
        switch (division)
        {
            case "Tank":
            {
                transform.Find("SlotOne").transform.Find("Image").GetComponent<Image>().sprite = tank;
                transform.Find("SlotOne").transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "SCAR-H";
            }
            break;
            case "Damage":
            {
                transform.Find("SlotOne").transform.Find("Image").GetComponent<Image>().sprite = damage;
                transform.Find("SlotOne").transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "AK-47";
            }
            break;
            case "Flank":
            {
                transform.Find("SlotOne").transform.Find("Image").GetComponent<Image>().sprite = flank;
                transform.Find("SlotOne").transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "MAC-10";
            }
            break;
        }
    }
}
