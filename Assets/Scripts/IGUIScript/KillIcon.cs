using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class manages the kill pop up when a player gets a kill
 * A kill icon is visible and it fades over time
*/

public class KillIcon : MonoBehaviour
{
    private float coolDown;
    private float alpha;
    void Start()
    {
        coolDown = 0f;
        alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDown <= 0f)
        {
            alpha = Mathf.Lerp(alpha, 0f, Time.deltaTime * 8f);
            GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        }
        else
        {
            alpha = Mathf.Lerp(alpha, 1f, Time.deltaTime * 16f);
            GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
        }

        coolDown -= Time.deltaTime;
    }

    public void TriggerKillIcon()
    {
        coolDown = 1f;
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        alpha = 1f;
        GetComponent<AudioSource>().Play();
    }
}
