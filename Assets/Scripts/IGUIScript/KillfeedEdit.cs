using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillfeedEdit : MonoBehaviour
{
    public GameObject killBar;
    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 4)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void AddtoKillfeed(string killer, string killed)
    {
        GameObject _killBar = Instantiate(killBar, transform) as GameObject;
        _killBar.transform.Find("Killer").GetComponent<TextMeshProUGUI>().text = killer;
        _killBar.transform.Find("Killed").GetComponent<TextMeshProUGUI>().text = killed;
    }
}
