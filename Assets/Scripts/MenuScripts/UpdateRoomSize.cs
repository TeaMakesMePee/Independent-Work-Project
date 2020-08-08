using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * This script edits the room size text when the slider changes value in the create room page
*/

public class UpdateRoomSize : MonoBehaviour
{
    // Start is called before the first frame update
    private int val;
    public TextMeshProUGUI text;
    void Start()
    {
        val = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)GetComponent<Slider>().value != val)
        {
            val = (int)GetComponent<Slider>().value;
            text.text = "Team Size: " + val.ToString();
        }
    }
}
