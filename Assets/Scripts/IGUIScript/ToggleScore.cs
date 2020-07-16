using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScore : MonoBehaviour
{
    public GameObject scoreboard;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        else
        {
            scoreboard.SetActive(false);
        }
    }
}
