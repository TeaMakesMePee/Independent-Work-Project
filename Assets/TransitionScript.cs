using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScript : MonoBehaviour
{
    public void OnFadeComplete()
    {
        GameObject.Find("MenuManager").GetComponent<HexGameLauncher>().StartGame();
    }
}
