using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{ 
    void Update() //Fps counter, used for frame rate check
    {
        float fps = 1f / Time.deltaTime;
        gameObject.GetComponent<TextMeshProUGUI>().text = ((int)fps).ToString();
    }
}
