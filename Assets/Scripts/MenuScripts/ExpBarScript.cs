using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This scripts handles the animation of the experience bar in the player statistics page in main menu
 * Lerps the % exp acquired for the level
*/

public class ExpBarScript : MonoBehaviour
{
    // Start is called before the first frame update
    float maxFill, currFill, relFill;
    int currBar;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float change = currFill;
        currFill = Mathf.Lerp(currFill, maxFill, Time.deltaTime * 4f);
        change = currFill - change;
        relFill += change;
        transform.GetChild(currBar).GetComponent<Image>().fillAmount = relFill;
        if (relFill >= 1f)
        {
            currBar++;
            float leftOver = relFill - 1f;
            relFill = leftOver;
            transform.GetChild(currBar).GetComponent<Image>().fillAmount = leftOver;
        }
    }

    public void SetExpStat(int curr, int remain)
    {
        currBar = 0;
        currFill = relFill = 0f;
        maxFill = ((float)curr / (float)remain) * 10f;
    }

    public void ResetExpStat()
    {
        for (int x = 0; x < transform.childCount; ++x)
            transform.GetChild(x).GetComponent<Image>().fillAmount = 0f;
    }
}
