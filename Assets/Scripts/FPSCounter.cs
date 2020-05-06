using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{ 
    void Update()
    {
        float fps = 1f / Time.deltaTime;
        gameObject.GetComponent<Text>().text = ((int)fps).ToString();
    }
}
