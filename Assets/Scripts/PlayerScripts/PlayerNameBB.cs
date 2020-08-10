using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class updates the rotation of the player's name above the character
 * This is to let the player know who their enemy is
*/

public class PlayerNameBB : MonoBehaviour
{
    GameObject thePlayer;
    Vector3 initForward;
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        initForward = thePlayer.transform.forward;
        initForward.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        if (thePlayer.transform == transform.root)
            return;
        Vector3 look = transform.root.forward;
        look.y = 0f;
        look.Normalize();
        Vector3 dir = thePlayer.transform.position - transform.root.position;
        dir.y = 0f;
        dir.Normalize();
        float angle = Vector3.Angle(dir, look);
        if (Vector3.Cross(dir, look).y > 0f)
            angle *= -1f;

        float offsetAngle = Vector3.Angle(initForward, look);
        if (Vector3.Cross(initForward, look).y > 0f)
            offsetAngle *= -1f;

        Debug.LogError(angle);
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, angle + 180f - offsetAngle, 0f);
    }
}
