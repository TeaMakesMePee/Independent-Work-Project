using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public HexGameLauncher launcher;

    public void JoinMatch()
    {
        launcher.Join();
    }

    public void CreateMatch()
    {
        launcher.Create();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //Removes username and password information to prevent auto login the next time the player tries to login
    public void SignOutExit()
    {
        if (PlayerPrefs.HasKey("username"))
            PlayerPrefs.DeleteKey("username");
        if (PlayerPrefs.HasKey("password"))
            PlayerPrefs.DeleteKey("password");
        Application.Quit();
    }
}
