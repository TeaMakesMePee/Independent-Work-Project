using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthTrigger : MonoBehaviour
{
    public PlayfabHandler playfab;
    // Start is called before the first frame update
    void Start()
    {
        if (GameData.GetAuth())
        {
            playfab.CloseTabs();
        }
    }
}
