using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillfeedEdit : MonoBehaviour
{
    public GameObject killBar;
    public List<Sprite> weapImages;
    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 4)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void AddtoKillfeed(string killer, string killed, string weapName)
    {
        GameObject _killBar = Instantiate(killBar, transform) as GameObject;
        _killBar.transform.Find("Killer").GetComponent<TextMeshProUGUI>().text = killer;
        _killBar.transform.Find("Killed").GetComponent<TextMeshProUGUI>().text = killed;

        Sprite theSprite = null;
        switch (weapName)
        {
            case "Ak47":
                theSprite = weapImages[0];
                break;
            case "Mac":
                theSprite = weapImages[1];
                break;
            case "Pistol":
                theSprite = weapImages[3];
                break;
            case "Scar":
                theSprite = weapImages[2];
                break;
        }

        _killBar.transform.Find("Image").GetComponent<Image>().sprite = theSprite;
    }
}
