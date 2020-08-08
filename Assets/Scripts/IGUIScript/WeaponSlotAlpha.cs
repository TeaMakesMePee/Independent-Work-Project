using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class manages the visibility of the weapon UI in game (bottom right of screen when you equip a weapon)
 * Mainly affects the alpha values to show the weapon equipped and fades to 0 overtime to not obstruct the view
*/

public class WeaponSlotAlpha : MonoBehaviour
{
    // Start is called before the first frame update
    private float currAlpha, nextAlpha;
    private Color theColor = Color.white;
    private Image theImage;
    private TextMeshProUGUI theText;
    private float offAlpha;
    private void Awake()
    {
        offAlpha = 0f;
        currAlpha = nextAlpha = 0f;
        theImage = transform.Find("Image").GetComponent<Image>();
        theText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        currAlpha = Mathf.Lerp(currAlpha, nextAlpha, Time.deltaTime * 12f);
        theImage.color = new Color(theColor.r, theColor.g, theColor.b, currAlpha / 255f);
        theText.color = new Color(theColor.r, theColor.g, theColor.b, currAlpha / 255f);
        if (offAlpha > 0f)
        {
            offAlpha -= Time.deltaTime;
            if (offAlpha < 0f)
            {
                offAlpha = 0f;
                nextAlpha = 0f;
            }
        }
    }

    public void Equip()
    {
        nextAlpha = 200f;
        offAlpha = 3f;
    }

    public void EquipOther()
    {
        nextAlpha = 80f;
        offAlpha = 3f;
    }
}
